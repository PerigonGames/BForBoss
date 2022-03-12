using System;
using UnityEditor;
using UnityEngine;

namespace BForBoss
{
    public class ImageEditorWindow : EditorWindow
    {
        private const float TOOLBAR_HEIGHT = 30f;
    
        public Action<Texture2D> OnWindowClosed;
        
        private Texture2D _originalScreenShot = null;
        private Texture2D _editedScreenShot = null;
        private ImageEditorWindow _window;
        
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
            
            _window = GetWindow<ImageEditorWindow>();
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
}
