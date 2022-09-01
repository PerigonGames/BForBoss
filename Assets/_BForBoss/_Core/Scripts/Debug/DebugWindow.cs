using System;
using System.Collections.Generic;
using System.Reflection;
using Perigon.Character;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BForBoss
{
    public class DebugWindow : MonoBehaviour
    {
        private const Key KEYCODE_CHARACTER = Key.Backquote;

        private const float CANVAS_WIDTH_MULTIPLIER = 0.15f;
        private const float CANVAS_HEIGHT_MULTIPLIER = 0.5f;
        
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private FreeRoamCamera _freeRoamCamera = null;

        //State Handling
        private StateManager _stateManager = StateManager.Instance;
        private State _currentState;
        
        private bool _isPanelShowing = false;
        
        private List<DebugView> _debugViews = new List<DebugView>();
        private DebugView _currentDebugView;

        private Rect _windowRect;
        
        private void Awake()
        {
#if (!UNITY_EDITOR && !DEVELOPMENT_BUILD)
            Destroy(gameObject);
#endif
            foreach (Type viewType in Assembly.GetAssembly(typeof(DebugView)).GetTypes())
            {
                if (viewType.IsClass && !viewType.IsAbstract && viewType.IsSubclassOf(typeof(DebugView)))
                {
                    ConstructorInfo constructorInfo = viewType.GetConstructors()[0];
                    DebugView debugView = constructorInfo.Invoke(GetDebugViewConstructorParameters(viewType)) as DebugView;
                    _debugViews.Add(debugView);
                }
            }
            
            _freeRoamCamera.Initialize(Camera.main.transform, onExit: OnFreeCameraDebugViewExited);
        }

        private void Update()
        {
            if (Keyboard.current[KEYCODE_CHARACTER].wasReleasedThisFrame)
            {
                if (_isPanelShowing)
                {
                    ClosePanel();
                }
                else
                {
                    OpenPanel();
                }
            }
        }

        private void OnGUI()
        {
            if (!_isPanelShowing)
            {
                return;
            }

            if (_currentDebugView != null && _currentDebugView.IsInitialized)
            {
                _currentDebugView.DrawGUI();
            }
            else
            {
                CreateDebugViewList();
            }
        }

        private void CreateDebugViewList()
        {
            using (new GUILayout.AreaScope(_windowRect, "Debug Options", GUI.skin.window))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Space(0.15f * _windowRect.height);

                        foreach (var view in _debugViews)
                        {
                            if (GUILayout.Button(view.PrettyName))
                            {
                                _currentDebugView = view;
                                OnViewOpened(view);
                            }
                        }
                    }
                    
                    GUILayout.FlexibleSpace();
                }
                
                GUILayout.FlexibleSpace();
            }
        }

        private void OnViewOpened(DebugView view)
        {
            if (view is FreeRoamCameraDebugView)
            {
                _freeRoamCamera.gameObject.SetActive(true);
            }
        }

        private void ResetView()
        {
            if (_currentDebugView != null)
            {
                _currentDebugView.ResetData();
                _currentDebugView = null;
            }
        }

        private void OpenPanel()
        {
            _currentState = _stateManager.GetState();
            _stateManager.SetState(State.Debug);
            GetCanvasRect();
            transform.localScale = Vector3.one;
            _isPanelShowing = !_isPanelShowing;
        }

        private void ClosePanel()
        {
            ResetView();
            transform.localScale = Vector3.zero;
            _stateManager.SetState(_currentState);
            _isPanelShowing = !_isPanelShowing;
        }
        
        private void OnFreeCameraOptionsChanged(bool isMouseRotationYInverted)
        {
            _freeRoamCamera.ShouldInvertMouseYAxis = isMouseRotationYInverted;
        }

        private void OnFreeCameraDebugViewExited()
        {
            _freeRoamCamera.gameObject.SetActive(false);
        }

        private object[] GetDebugViewConstructorParameters(Type debugType)
        {
            List<object> parameters = new List<object> {_windowRect};

            if (debugType == typeof(FreeRoamCameraDebugView))
            {
                Rect freeCameraRect = new Rect(_windowRect)
                {
                    width = _windowRect.width + 20f
                };
                parameters = new List<object>{freeCameraRect,
                    (Action<bool>) OnFreeCameraOptionsChanged,
                    (Action) OnFreeCameraDebugViewExited};
            }

            return parameters.ToArray();
        }

        private void GetCanvasRect()
        {
            if (_rectTransform != null)
            {
                Vector2 anchorMin = _rectTransform.anchorMin;
                
                _windowRect = new Rect(anchorMin.x * Screen.width,
                    anchorMin.y * Screen.height, Screen.width * CANVAS_WIDTH_MULTIPLIER,
                    Screen.height * CANVAS_HEIGHT_MULTIPLIER);
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            GetCanvasRect();

            if (_currentDebugView != null)
            {
                _currentDebugView.MasterRect = _windowRect;
            }
        }
    }
}
