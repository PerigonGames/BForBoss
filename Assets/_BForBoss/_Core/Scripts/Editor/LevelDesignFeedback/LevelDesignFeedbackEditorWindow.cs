using System;
using System.Text;
using BForBoss;
using Trello;
using UnityEditor;
using UnityEngine;

public class LevelDesignFeedbackEditorWindow : EditorWindow
{
    private const float ELEMENT_SPACING = 5f;
    private const int NUMBER_OF_FEEDBACK_LINES = 5;

    public Action OnWindowClosed;
    
    private static LevelDesignFeedbackEditorWindow _window = null;

    private static Texture2D _originalImage;
    private static Texture2D _image;
    private string _title;
    private string _feedback;
    
    public static LevelDesignFeedbackEditorWindow OpenWindow(Texture2D screenshot)
    {
        _originalImage = screenshot;
        _image = CreateTextureCopy(_originalImage);
        _window = (LevelDesignFeedbackEditorWindow) GetWindow(typeof(LevelDesignFeedbackEditorWindow));
        _window.titleContent = new GUIContent("Add Level Design Feedback");
        _window.minSize = new Vector2(250, 500);
        _window.maxSize = new Vector2(400, 700);
        _window.Show();

        EditorApplication.isPaused = true;

        return _window;
    }
    
    private static Texture2D CreateTextureCopy(Texture2D sourceTexture)
    {
        Texture2D textureCopy = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, false);
        textureCopy.SetPixels(sourceTexture.GetPixels());
        textureCopy.Apply();

        return textureCopy;
    }

    private void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            DrawPreviewImage();

            EditorGUILayout.Space(ELEMENT_SPACING);

            DrawDescription();
            
            GUILayout.FlexibleSpace();

            DrawButtons();
            
            EditorGUILayout.Space(ELEMENT_SPACING);
        }
    }

    private void DrawPreviewImage()
    {
        Rect previewImageRect;
        EditorGUILayout.Space(ELEMENT_SPACING);

        using (new EditorGUILayout.VerticalScope())
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                previewImageRect = new Rect(position.width * 0.05f,position.height * 0.02f, position.width * 0.9f, position.height * 0.4f);
                
                if (_image == null)
                {
                    EditorGUI.DrawRect(previewImageRect, Color.gray);
                }
                else
                {
                    EditorGUI.DrawTextureTransparent(previewImageRect, _image, ScaleMode.ScaleToFit, 0);
                }

                if (_image != null && WasElementDoubleClicked(previewImageRect))
                {
                    // new SceneViewCameraWindow(SceneView.currentDrawingSceneView)

                    ImageEditorWindow imagePopupWindow = GetWindow<ImageEditorWindow>();
                    imagePopupWindow.OnWindowClosed += OnImageEdited;
                    imagePopupWindow.OpenWindow(_image);
                }
            
                GUILayout.FlexibleSpace();
            }
            
            EditorGUILayout.Space(previewImageRect.y + previewImageRect.height);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Reset"))
                {
                    _image = CreateTextureCopy(_originalImage);
                }
                
                EditorGUILayout.Space(position.width * 0.05f);
            }
        }
        
        EditorGUILayout.Space(ELEMENT_SPACING);
    }

    private void OnImageEdited(Texture2D editedScreenShot)
    {
        _image = editedScreenShot;
        Repaint();
    }

    private void DrawDescription()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 35f;
            EditorGUILayout.PrefixLabel("Title", GUI.skin.button, EditorStyles.boldLabel);
            _title = EditorGUILayout.TextField(_title, GUILayout.ExpandWidth(true));
            EditorGUIUtility.labelWidth = labelWidth;
        }
            
        EditorGUILayout.Space(ELEMENT_SPACING);
        EditorGUILayout.LabelField("Feedback",EditorStyles.boldLabel);
        _feedback = EditorGUILayout.TextArea(_feedback, GUI.skin.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * NUMBER_OF_FEEDBACK_LINES));
    }

    private void DrawButtons()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.Space(ELEMENT_SPACING);

            if (GUILayout.Button("Close"))
            {
                _window.Close();
            }
                
            GUILayout.FlexibleSpace();

            GUI.enabled = _image != null && !string.IsNullOrEmpty(_title) && !string.IsNullOrEmpty(_feedback);
                
            if (GUILayout.Button("Create Feedback"))
            {
                SendFeedback();
            }

            GUI.enabled = true;

            EditorGUILayout.Space(ELEMENT_SPACING);
        }
    }

    private void SendFeedback()
    {
        TrelloCard card = new TrelloCard
        {
            name = _title,
            desc = GenerateDescription(_feedback),
            attachment = new TrelloCard.Attachment(_image, "Attachment.jpg")
        };
        TrelloSend.SendNewCard(card, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, null, OnFeedbackSent);
    }

    private void OnFeedbackSent(bool success)
    {
        string message = $"The Trello Card {_title} was {(success ? "successfully" : "unsuccessfully")} created";
        EditorUtility.DisplayDialog("Level Design Feedback Window", message, "Sounds good");
    }

    private bool WasElementDoubleClicked(Rect elementRect)
    {
        Event evt = Event.current;
        bool hasMouseClick = evt.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive, elementRect)) == EventType.MouseDown;

        return hasMouseClick && evt.button == 0 && evt.clickCount == 2 && elementRect.Contains(evt.mousePosition);
    }

    private string GenerateDescription(string feedback)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("#Feedback");
        stringBuilder.AppendLine("___");
        stringBuilder.AppendLine("###User Description");
        stringBuilder.AppendLine("```");
        stringBuilder.AppendLine(feedback);
        stringBuilder.AppendLine("```");
        stringBuilder.AppendLine("___");
        
        if (Camera.main != null)
        {
            stringBuilder.AppendLine("###Additional Details");
            stringBuilder.AppendLine($"Main Camera Position : {Camera.main.transform.position}");
        }

        return stringBuilder.ToString();
    }

    private void OnDestroy()
    {
        OnWindowClosed?.Invoke();
    }
}