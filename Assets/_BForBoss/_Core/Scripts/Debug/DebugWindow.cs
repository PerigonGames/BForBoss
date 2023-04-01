using System;
using System.Collections.Generic;
using System.Reflection;
using Perigon.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BForBoss
{
    public class DebugWindow : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private const Key KEYCODE_CHARACTER = Key.Backquote;

        private const float CANVAS_WIDTH_MULTIPLIER = 0.2f;
        private const float CANVAS_HEIGHT_MULTIPLIER = 0.5f;
        
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private FreeRoamCamera _freeRoamCamera = null;
        
        private PlayerLifeCycleBehaviour _playerLifeCycle;
        private EnergySystemBehaviour _energySystemBehaviour;
        private bool _isPlayerCurrentlyInvincible;
        
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
            _playerLifeCycle = FindObjectOfType<PlayerLifeCycleBehaviour>();
            _energySystemBehaviour = FindObjectOfType<EnergySystemBehaviour>();
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

            if (_currentDebugView?.IsShown ?? false)
            {
                _currentDebugView.DrawGUI();
            }
            else
            {
                using (new GUILayout.AreaScope(_windowRect, "Debug Options", GUI.skin.window))
                {
                    CreateDebugViewList();
                    CreateDebugActionList();
                }
            }
        }

        private void CreateDebugViewList()
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
        }

        private void CreateDebugActionList()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Space(0.15f * _windowRect.height);
                    _playerLifeCycle.IsInvincible = DrawDebugToggle(_playerLifeCycle.IsInvincible, new GUIContent("Player Invincibility"));
                    _energySystemBehaviour.UseDebugEnergySystemConfig = DrawDebugToggle(_energySystemBehaviour.UseDebugEnergySystemConfig, new GUIContent("Freeze Energy"));
                    if (GUILayout.Button("Give Max Energy"))
                    {
                        _energySystemBehaviour.SetMaxEnergy();
                    }
                }
                    
                GUILayout.FlexibleSpace();
            }
        }

        private void OnViewOpened(DebugView view)
        {
            view.IsShown = true;
            if (view is FreeRoamCameraDebugView)
            {
                _freeRoamCamera.gameObject.SetActive(true);
                _isPlayerCurrentlyInvincible = _playerLifeCycle.IsInvincible;
                _playerLifeCycle.IsInvincible = true;
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
            _playerLifeCycle.IsInvincible = _isPlayerCurrentlyInvincible;
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

        private bool DrawDebugToggle(bool toggle, GUIContent content)
        {
            Color currentColor = GUI.color;
            GUI.color = toggle ? Color.green : Color.red;
            toggle = GUILayout.Toggle(toggle, content, GUI.skin.button);
            GUI.color = currentColor;
            
            return toggle;
        }

        private void OnRectTransformDimensionsChange()
        {
            GetCanvasRect();

            if (_currentDebugView != null)
            {
                _currentDebugView.MasterRect = _windowRect;
            }
        }
#endif
    }
}
