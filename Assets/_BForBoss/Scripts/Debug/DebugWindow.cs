using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BForBoss
{
    public class DebugWindow : MonoBehaviour
    {

        private const Key _keyCodeCharacter = Key.F1;
        private const Key _keyCodeModifier = Key.LeftCtrl;

        private const float _canvasWidthMultiplier = 0.15f;
        private const float _canvasHeightMultiplier = 0.5f;
        
        //[SerializeField] private List<DebugOptions> _debugOptions;
        [SerializeField] private RectTransform _rectTransform;

        //State Handling
        private StateManager _stateManager = StateManager.Instance;
        private State _currentState;
        
        private bool _isPanelShowing = false;
        
        private List<Type> _debugOptions = new List<Type>();
        private DebugView _currentDebugView;

        private Rect _windowRect;
        
        private void Awake()
        {
            foreach (Type viewType in Assembly.GetAssembly(typeof(DebugView)).GetTypes())
            {
                if (viewType.IsClass && !viewType.IsAbstract && viewType.IsSubclassOf(typeof(DebugView)))
                {
                    _debugOptions.Add(viewType);
                }
            }
        }

        private void Update()
        {
            if (Keyboard.current[_keyCodeModifier].isPressed &&
                Keyboard.current[_keyCodeCharacter].wasPressedThisFrame)
            {
                if (_isPanelShowing)
                {
                    ClosePanel();
                }
                else
                {
                    OpenPanel();
                }

                _isPanelShowing = !_isPanelShowing;
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

                        foreach (Type debugType in _debugOptions)
                        {
                            if (GUILayout.Button(debugType.Name))
                            {
                                ConstructorInfo constructorInfo = debugType.GetConstructor(new Type[] {typeof(Rect)});
                                object[] parameters = new object[] {_windowRect};
                                _currentDebugView = constructorInfo.Invoke(parameters) as DebugView;
                            }
                        }
                    }
                    
                    GUILayout.FlexibleSpace();
                }
                
                GUILayout.FlexibleSpace();
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
            _stateManager.SetState(State.Pause);
            FirstPersonPlayer.IsDebugWindowOpen = true;
            GetCanvasRect();
            transform.localScale = Vector3.one;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void ClosePanel()
        {
            FirstPersonPlayer.IsDebugWindowOpen = false;
            ResetView();
            transform.localScale = Vector3.zero;
            _stateManager.SetState(_currentState);
        }

        private bool IsPanelShowing()
        {
            return _isPanelShowing;
        }

        private void GetCanvasRect()
        {
            if (_rectTransform != null)
            {
                Vector2 anchorMin = _rectTransform.anchorMin;
                
                _windowRect = new Rect(anchorMin.x * Screen.width,
                    anchorMin.y * Screen.height, Screen.width * _canvasWidthMultiplier,
                    Screen.height * _canvasHeightMultiplier);
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
