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
    private const float TOOLBAR_HEIGHT = 30f;
    
    public Action<Texture2D> OnWindowClosed;
    
    private Texture2D _originalScreenShot = null;
    private Texture2D _editedScreenShot = null;
    private FeedbackScreenShotEditContent _window;
    
    private bool _shouldCloseWindow = false;
    private bool _drawThisFrame = false;
    private bool _forceRepaint = false;

    private Rect _screenShotRect;
    private Color _brushColor = Color.black;
    private int _currentBrushIndex = 1;
    private int _brushSize;
    private readonly string[] _brushSelections = new string[3] {"Small", "Medium", "Large"};
    private readonly int[] _brushSizes = new int[3] {4, 7, 10};

    public void OpenWindow(Texture2D screenShot)
    {
        if (screenShot == null)
        {
            return;
        }
        
        _originalScreenShot = screenShot;
        _editedScreenShot = CreateTextureCopy(_originalScreenShot);

        Undo.undoRedoPerformed += OnUndoBrushStroke;
        
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
        if (_shouldCloseWindow)
        {
            Close();
        }
        
        DrawScreenShotToEdit();
        EditorGUILayout.Space(_editedScreenShot.height + 5f);
        DrawToolbar();
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
                case EventType.MouseDrag:
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
        
        Undo.RecordObject(_editedScreenShot, "ScreenShot Edit");
        
        _brushSize = _brushSizes[_currentBrushIndex];
        Color[] pixels = _editedScreenShot.GetPixels();
        Vector2 mousePosition = evt.mousePosition;
        Vector2Int relativeMousePosition = new Vector2Int((int)mousePosition.x, (int)(_editedScreenShot.height - mousePosition.y));
        
        for (int i = -_brushSize; i <= _brushSize; i++)
        {
            int centreXPoint = relativeMousePosition.x + i;
            if (centreXPoint < 0)
            {
                continue;
            }
        
            if (centreXPoint >= _editedScreenShot.width)
            {
                break;
            }
        
            for (int j = -_brushSize; j <= _brushSize; j++)
            {
                int centreYPoint = relativeMousePosition.y + j;
                if (centreYPoint < 0)
                {
                    continue;
                }
        
                if (centreYPoint >= _editedScreenShot.height)
                {
                    break;
                }

                pixels[centreXPoint + (centreYPoint * _editedScreenShot.width)] = _brushColor;
            }
        }
        
        _editedScreenShot.SetPixels(pixels);
        _editedScreenShot.Apply();

        _forceRepaint = true;
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 70f;
            _brushColor = EditorGUILayout.ColorField("Brush Color", _brushColor);
            EditorGUIUtility.labelWidth = 65f;
            _currentBrushIndex = EditorGUILayout.Popup("Brush Size", _currentBrushIndex, _brushSelections, GUILayout.ExpandWidth(true));
            EditorGUIUtility.labelWidth = originalLabelWidth;
            
            EditorGUILayout.Space();
        
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

    private void Update()
    {
        if (_forceRepaint)
        {
            _forceRepaint = false;
            Repaint();
        }
    }

    private Texture2D CreateTextureCopy(Texture2D sourceTexture)
    {
        Texture2D textureCopy = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, false);
        textureCopy.SetPixels(sourceTexture.GetPixels());
        textureCopy.Apply();

        return textureCopy;
    }

    private void SaveEditedChanges()
    {
        _originalScreenShot = _editedScreenShot;
        Close();
    }

    private void Reset()
    {
        _editedScreenShot = CreateTextureCopy(_originalScreenShot);
    }
    
    private void OnUndoBrushStroke()
    {
        _forceRepaint = true;
    }
    
    private void OnLostFocus()
    {
        Rect windowRect = new Rect(0,0, position.width, position.height);
        if (Event.current == null || !windowRect.Contains(Event.current.mousePosition))
        {
            _shouldCloseWindow = true;
        }
    }

    private void OnDestroy()
    {
        Undo.undoRedoPerformed -= OnUndoBrushStroke;
        OnWindowClosed?.Invoke(_originalScreenShot);
    }
}