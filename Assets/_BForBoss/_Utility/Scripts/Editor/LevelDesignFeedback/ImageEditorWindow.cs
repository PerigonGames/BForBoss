using System;
using System.Collections.Generic;
using Sirenix.Utilities;
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
        private Color32 _brushColor = Color.black;
        private int _currentBrushIndex = 1;
        private int _brushSize;
        private readonly string[] _brushSelections = {"Small", "Medium", "Large"};
        private readonly int[] _brushSizes = {4, 7, 10};

        private bool _shouldDrawPaint = true;

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
            EditorGUI.DrawPreviewTexture(_screenShotRect, _editedScreenShot);

            if (_shouldDrawPaint)
            {
                OnPaintInputUpdate();
                DrawPreviewPaint();
            }
            else
            {
                OnInputUpdate();
                DrawPreviewSquareIfMousePressedDown();
            }


            EditorGUILayout.Space(_editedScreenShot.height + 5f);
            DrawToolbar();
        }

        private List<Vector3> _listOfPoints = new List<Vector3>();
        private void OnPaintInputUpdate()
        {
            Event inputEvent = Event.current;
            if (inputEvent == null || mouseOverWindow != this )
            {
                return;
            } 
            
            if (inputEvent.isMouse && inputEvent.button == 0)
            {
                if (!_screenShotRect.Contains(inputEvent.mousePosition))
                {
                    return;
                }
                
                switch (inputEvent.type)
                {
                    case EventType.MouseDown:
                        _listOfPoints.Add(inputEvent.mousePosition);
                        break;
                    case EventType.MouseDrag:
                        _listOfPoints.Add(inputEvent.mousePosition);
                        break;
                    case EventType.MouseUp:
                    {
                        DrawPaintIfNeeded();
                        break;
                    }
                }
            }
        }

        private void DrawPreviewPaint()
        {
            if (!_listOfPoints.IsNullOrEmpty())
            {
                Handles.BeginGUI();
                Handles.color = _brushColor;
                Vector3 lastPoint = Vector3.zero;
                foreach (var point in _listOfPoints)
                {
                    if (lastPoint != Vector3.zero)
                    {
                        Handles.DrawLine(lastPoint, point);
                    }

                    lastPoint = point;
                }

                Repaint();
                Handles.EndGUI();
            }
        }

        private void DrawPaintIfNeeded()
        {
            if (!_listOfPoints.IsNullOrEmpty())
            {
                Vector3 lastPoint = Vector3.zero;
                foreach (var point in _listOfPoints)
                {
                    if (lastPoint != Vector3.zero)
                    {
                        Vector2Int relativeLastPoint = new Vector2Int((int)lastPoint.x, (int)(_editedScreenShot.height - lastPoint.y));
                        Vector2Int relativePoint = new Vector2Int((int)point.x, (int)(_editedScreenShot.height - point.y));
                        DrawLine(_editedScreenShot, relativeLastPoint, relativePoint, _brushColor);
                    }

                    lastPoint = point;
                }
                _editedScreenShot.Apply();
                Repaint();
            }
            
            _listOfPoints.Clear();
        }
        
        
        private Vector3 mouseDownPosition = Vector3.zero;

        private void OnInputUpdate()
        {
            Event inputEvent = Event.current;
            if (inputEvent == null || mouseOverWindow != this )
            {
                return;
            } 
            
            if (inputEvent.isMouse && inputEvent.button == 0)
            {
                if (!_screenShotRect.Contains(inputEvent.mousePosition))
                {
                    return;
                }
                
                switch (inputEvent.type)
                {
                    case EventType.MouseDown:
                        mouseDownPosition = inputEvent.mousePosition;
                        break;
                    case EventType.MouseUp:
                    {
                        DrawSquareOnToScreenshot();
                        break;
                    }
                }
            }
        }

        private void DrawPreviewSquareIfMousePressedDown()
        {
            if (mouseDownPosition != Vector3.zero)
            {
                Handles.BeginGUI();
                Handles.color = _brushColor;
                var currentMousePosition = Event.current.mousePosition;
                var topRight = new Vector3(mouseDownPosition.x, currentMousePosition.y);
                var bottomLeft = new Vector3(currentMousePosition.x, mouseDownPosition.y);
                Handles.DrawLine(mouseDownPosition, topRight);
                Handles.DrawLine(topRight, currentMousePosition);
                Handles.DrawLine(currentMousePosition, bottomLeft);
                Handles.DrawLine(bottomLeft, mouseDownPosition);
                Repaint();
                Handles.EndGUI();
            }
            
        }

        private void DrawSquareOnToScreenshot()
        {
            if (mouseDownPosition == Vector3.zero)
            {
                return;
            }

            Vector2 mouseUpPosition = Event.current.mousePosition;
            Vector2Int relativeMouseDownPosition = new Vector2Int((int)mouseDownPosition.x, (int)(_editedScreenShot.height - mouseDownPosition.y));
            Vector2Int relativeMouseUpPosition = new Vector2Int((int)mouseUpPosition.x, (int)(_editedScreenShot.height - mouseUpPosition.y));

            var topRight = new Vector3(relativeMouseDownPosition.x, relativeMouseUpPosition.y);
            var bottomLeft = new Vector3(relativeMouseUpPosition.x, relativeMouseDownPosition.y);
            var thickness = _brushSizes[2];
            DrawLine(_editedScreenShot, relativeMouseDownPosition, topRight, _brushColor, thickness);
            DrawLine(_editedScreenShot, topRight, relativeMouseUpPosition, _brushColor, thickness);
            DrawLine(_editedScreenShot, relativeMouseUpPosition, bottomLeft, _brushColor, thickness);
            DrawLine(_editedScreenShot, bottomLeft, relativeMouseDownPosition, _brushColor, thickness);
            
            _editedScreenShot.Apply();
            Repaint();

            mouseDownPosition = Vector3.zero;
        }

        //https://answers.unity.com/questions/244417/create-line-on-a-texture.html
        private void DrawLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col, int thickness = 1)
        {
            Vector2 t = p1;
            float frac = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
            float ctr = 0;
     
            while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y) {
                t = Vector2.Lerp(p1, p2, ctr);
                ctr += frac;
                for (int i = 0; i < thickness; i++)
                {
                    tex.SetPixel((int)t.x + i, (int)t.y + i, col);
                    tex.SetPixel((int)t.x - i, (int)t.y + i, col);
                    tex.SetPixel((int)t.x - i, (int)t.y - i, col);
                    tex.SetPixel((int)t.x - i, (int)t.y - i, col);
                    
                    tex.SetPixel((int)t.x, (int)t.y - i, col);
                    tex.SetPixel((int)t.x, (int)t.y + i, col);
                    
                    tex.SetPixel((int)t.x - i, (int)t.y, col);
                    tex.SetPixel((int)t.x + i, (int)t.y, col);
                }
            }
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
                _shouldDrawPaint = EditorGUILayout.Toggle("Should use Paint", _shouldDrawPaint);
                
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
