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
    private const int BRUSH_SIZE_OFFSET = 10;
    private const float TOOLBAR_HEIGHT = 30f;
    
    public Action<Texture2D> OnWindowClosed;
    
    private Texture2D _originalScreenShot = null;
    private Texture2D _editedScreenShot = null;
    private FeedbackScreenShotEditContent _window;
    
    private bool _shouldCloseWindow = false;
    private bool _isBufferFrame = true;
    private bool _drawThisFrame = false;

    private Rect _screenShotRect;
    private Color _brushColor = Color.black;
    private int _currentBrushIndex = 0;
    private readonly string[] _brushSelections = new string[3] {"Small", "Medium", "Large"};

    public void OpenWindow(Texture2D screenShot)
    {
        _originalScreenShot = screenShot;
        _editedScreenShot = new Texture2D(screenShot.width, screenShot.height, TextureFormat.ARGB32, false);
        _editedScreenShot.SetPixels(screenShot.GetPixels());
        _editedScreenShot.Apply();

        _window = GetWindow<FeedbackScreenShotEditContent>();
        _window.minSize = new Vector2(screenShot.width, screenShot.height + TOOLBAR_HEIGHT);
        _window.maxSize = _window.minSize;
        _window.titleContent = new GUIContent("Edit ScreenShot");

        _window.wantsMouseMove = true;
        _window.wantsMouseEnterLeaveWindow = true;
        
        _screenShotRect = new Rect(0,0,screenShot.width, screenShot.height);
        
        _window.ShowPopup();
    }

    private void OnGUI()
    {
        if (_originalScreenShot == null)
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

        using (new EditorGUILayout.VerticalScope())
        {
            DrawScreenShotToEdit();
            EditorGUILayout.Space(_editedScreenShot.height + 5f);
            DrawToolbar();
        }
    }

    private void DrawScreenShotToEdit()
    {
        EditorGUI.DrawPreviewTexture(_screenShotRect, _editedScreenShot);

        if (mouseOverWindow != this)
        {
            return;
        }
        
        Event evt = Event.current;

        if (evt.isMouse && evt.button == 0)
        {
            if (!_screenShotRect.Contains(evt.mousePosition))
            {
                return;
            }
            
            switch (evt.type)
            {
                case EventType.MouseDown:
                {
                    _drawThisFrame = true;
                    break;
                }
                case EventType.MouseUp:
                {
                    _drawThisFrame = false;
                    break;
                }
            }
        }

        if (!_drawThisFrame)
        {
            return;
        }
        
        Debug.Log(evt.mousePosition);
        Vector2 mousePosition = evt.mousePosition;
        Vector2Int relativeMousePosition = new Vector2Int((int)mousePosition.x, (int)(_editedScreenShot.height - mousePosition.y));

        for (int i = -BRUSH_SIZE_OFFSET; i <= BRUSH_SIZE_OFFSET; i++)
        {
            if (i < 0 || i >= _editedScreenShot.width)
            {
                continue;
            }
                
            for (int j = -BRUSH_SIZE_OFFSET; j <= BRUSH_SIZE_OFFSET; j++)
            {
                if (j < 0 || j >= _editedScreenShot.height)
                {
                    continue;
                }
                    
                _editedScreenShot.SetPixel(relativeMousePosition.x + i, relativeMousePosition.y + j, _brushColor);
            }
        }
        
        _editedScreenShot.Apply();
        Repaint();
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 10f;
            _brushColor = EditorGUILayout.ColorField("Brush Color", _brushColor);
            _currentBrushIndex = EditorGUILayout.Popup("Brush Size", _currentBrushIndex, _brushSelections);
            EditorGUIUtility.labelWidth = originalLabelWidth;
            
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save Changes"))
            {
                SaveEditedChanges();
            }

            if (GUILayout.Button("Reset"))
            {
                Reset();
            }
        }
    }

    private void SaveEditedChanges()
    {
        _originalScreenShot = _editedScreenShot;
        Close();
    }

    private void Reset()
    {
        _editedScreenShot = _originalScreenShot;
    }
    
    private void OnLostFocus()
    {
        if (!position.Contains(Event.current.mousePosition))
        {
            _shouldCloseWindow = true;
        }
    }

    private void OnDestroy()
    {
        OnWindowClosed?.Invoke(_originalScreenShot);
    }
}