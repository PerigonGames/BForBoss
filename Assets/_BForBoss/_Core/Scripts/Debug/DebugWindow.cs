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
        
        //[SerializeField] private List<DebugOptions> _debugOptions;
        [SerializeField] private RectTransform _rectTransform;

        //State Handling
        private StateManager _stateManager = StateManager.Instance;
        private State _currentState;
        
        private bool _isPanelShowing = false;
        
        private List<Type> _debugOptions = new List<Type>();
        private DebugView _currentDebugView;

        private Rect _windowRect;
        private static DebugWindow _instance = null;

        public static DebugWindow Instance => _instance;
        
        private void Awake()
        {
#if (!UNITY_EDITOR && !DEVELOPMENT_BUILD)
            Destroy(gameObject);
#endif
            
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
            
            DontDestroyOnLoad(this);
            
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
            if (Keyboard.current[KEYCODE_CHARACTER].wasPressedThisFrame)
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
            IInputSettings input = FindObjectOfType<FirstPersonPlayer>();
            input.SwapToUIActions();
            GetCanvasRect();
            transform.localScale = Vector3.one;
            LockMouseUtility.Instance.UnlockMouse();
        }

        private void ClosePanel()
        {
            IInputSettings input = FindObjectOfType<FirstPersonPlayer>();
            input.SwapToPlayerActions();
            ResetView();
            transform.localScale = Vector3.zero;
            _stateManager.SetState(_currentState);
            LockMouseUtility.Instance.LockMouse();
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
