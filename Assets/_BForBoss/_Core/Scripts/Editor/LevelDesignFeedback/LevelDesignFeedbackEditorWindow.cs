using System;
using UnityEditor;
using UnityEngine;

public class LevelDesignFeedbackEditorWindow : EditorWindow
{
    private const float ELEMENT_SPACING = 5f;
    private const int NUMBER_OF_FEEDBACK_LINES = 5;

    public Action OnWindowClosed;
    
    private static LevelDesignFeedbackEditorWindow _window = null;

    private static Texture2D _image;
    private string _title;
    private string _feedback;
    
    public static LevelDesignFeedbackEditorWindow OpenWindow(Texture2D screenshot)
    {
        _image = screenshot;
        _window = (LevelDesignFeedbackEditorWindow) GetWindow(typeof(LevelDesignFeedbackEditorWindow));
        _window.titleContent = new GUIContent("Add Level Design Feedback");
        _window.minSize = new Vector2(250, 500);
        _window.maxSize = new Vector2(400, 700);
        _window.Show();

        return _window;
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
                Debug.Log("How you Doing?");
                // new SceneViewCameraWindow(SceneView.currentDrawingSceneView)
                
                PopupWindow.Show(previewImageRect, new FeedbackScreenshotEditContent(_image));
                
            }
            
            GUILayout.FlexibleSpace();
        }


        EditorGUILayout.Space(previewImageRect.y + previewImageRect.height);
        EditorGUILayout.Space(ELEMENT_SPACING);
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
                Debug.Log("Feedback added");
            }

            GUI.enabled = true;

            EditorGUILayout.Space(ELEMENT_SPACING);
        }
    }

    private bool WasElementDoubleClicked(Rect elementRect)
    {
        Event evt = Event.current;
        bool hasMouseClick = evt.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive, elementRect)) == EventType.MouseDown;

        return hasMouseClick && evt.button == 0 && evt.clickCount == 2 && elementRect.Contains(evt.mousePosition);
    }

    private void OnDestroy()
    {
        OnWindowClosed?.Invoke();
    }
}

public class FeedbackScreenshotEditContent : PopupWindowContent
{
    private Texture2D _screenshot;
    
    private readonly Vector2 _windowSize;

    public FeedbackScreenshotEditContent(Texture2D screenShot)
    {
        _screenshot = screenShot;
        
        float popupWidth = ((float)Display.main.systemWidth / 2f) * EditorGUIUtility.pixelsPerPoint;
        float popupHeight = ((float)Display.main.systemHeight / 2f) * EditorGUIUtility.pixelsPerPoint;
        _windowSize = new Vector2(popupWidth, popupHeight);
    }

    public override void OnGUI(Rect rect)
    {
        
    }

    public override Vector2 GetWindowSize()
    {
        return _windowSize;
    }
}
