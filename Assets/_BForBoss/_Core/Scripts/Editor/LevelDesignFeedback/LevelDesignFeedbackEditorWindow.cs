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

        EditorApplication.isPaused = true;

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
                // new SceneViewCameraWindow(SceneView.currentDrawingSceneView)

                FeedbackScreenShotEditContent imagePopupWindow = GetWindow<FeedbackScreenShotEditContent>();
                imagePopupWindow.OnWindowClosed += OnImageEdited;
                imagePopupWindow.OpenWindow(_image);
            }
            
            GUILayout.FlexibleSpace();
        }


        EditorGUILayout.Space(previewImageRect.y + previewImageRect.height);
        EditorGUILayout.Space(ELEMENT_SPACING);
    }

    private void OnImageEdited(Texture2D editedScreenShot)
    {
        _image = editedScreenShot;
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

public class FeedbackScreenShotEditContent : EditorWindow
{
    public Action<Texture2D> OnWindowClosed;
    
    private Texture2D _screenShot = null;
    private FeedbackScreenShotEditContent _window;
    
    private bool _shouldCloseWindow = false;
    private bool _isBufferFrame = true;
    
    private Color _brushColor = Color.black;
    private int _brushSize = 4;

    public void OpenWindow(Texture2D screenShot)
    {
        _screenShot = screenShot;

        _window = GetWindow<FeedbackScreenShotEditContent>();
        _window.minSize = new Vector2(screenShot.width, screenShot.height);
        _window.maxSize = _window.minSize;
        _window.titleContent = new GUIContent("Edit ScreenShot");

        _window.wantsMouseMove = true;
        _window.wantsMouseEnterLeaveWindow = true;
        
        _window.ShowPopup();
    }

    private void OnGUI()
    {
        if (_screenShot == null)
        {
            return;
        }
        
        if (_isBufferFrame)
        {
            _isBufferFrame = false;
            return;
        }
        
        if (_shouldCloseWindow)
        {
            Close();
        }
        
        EditorGUI.DrawPreviewTexture(new Rect(0,0,position.width,position.height), _screenShot);

        if (mouseOverWindow != this)
        {
            return;
        }
        
        Event evt = Event.current;
        if (evt.isMouse && evt.button == 0 && evt.type == EventType.MouseDrag)
        {
            Debug.Log(evt.mousePosition);
            Vector2 mousePosition = evt.mousePosition;
            Vector2Int relativeMousePosition = new Vector2Int((int)mousePosition.x, (int)(_screenShot.height - mousePosition.y));

            for (int i = -2; i <= 2; i++)
            {
                if (i < 0 || i >= _screenShot.width)
                {
                    continue;
                }
                
                for (int j = -2; j <= 2; j++)
                {
                    if (j < 0 || j >= _screenShot.height)
                    {
                        continue;
                    }
                    
                    _screenShot.SetPixel(relativeMousePosition.x + i, relativeMousePosition.y + j, _brushColor);
                }
            }
            
            _screenShot.Apply();
            
            // _screenShot.SetPixel(relativeMousePosition.x, relativeMousePosition.y, _brushColor);
            // _screenShot.Apply();
        }
    }

    private void OnLostFocus()
    {
        _shouldCloseWindow = true;
    }

    private void OnDestroy()
    {
        OnWindowClosed?.Invoke(_screenShot);
    }
}