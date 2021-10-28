using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace BForBoss
{
    public class DebugWindow : MonoBehaviour
    {
        public static Action OnGUIUpdate;

        private const Key _keyCodeCharacter = Key.F1;
        private const Key _keyCodeModifier = Key.LeftCtrl;

        private const float _canvasWidthMultiplier = 0.15f;
        private const float _canvasHeightMultiplier = 0.5f;
        
        //[SerializeField] private List<DebugOptions> _debugOptions;
        [SerializeField] private RectTransform _rectTransform;

        private bool _isPanelShowing = false;
        private DebugView _currentDebugView;

        private Rect _windowRect;
        
        public void Initialize()
        {
            //Use this method to get Rect and get list of Options;
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

            if (OnGUIUpdate != null)
            {
                OnGUIUpdate.Invoke();
            }
            else
            {
                CreateDebugViewList();
            }
        }

        private void CreateDebugViewList()
        {
            using (new GUILayout.AreaScope(_windowRect, String.Empty, GUI.skin.window))
            {
                //GUILayout.FlexibleSpace();

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Space(0.15f * _windowRect.height);
                        
                        for (int i = 0; i < 5; i++)
                        {
                            if (GUILayout.Button($"Press This {i}"))
                            {
                                Debug.Log($"You Pressed this {i}");
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
            
        }

        private void OpenPanel()
        {
            FirstPersonPlayer.IsDebugWindowOpen += IsPanelShowing;
            GetCanvasRect();
            transform.localScale = Vector3.one;
            SceneManager.sceneLoaded += OnSceneChanged;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void ClosePanel()
        {
            FirstPersonPlayer.IsDebugWindowOpen -= IsPanelShowing;
            ResetView();
            transform.localScale = Vector3.one;
            SceneManager.sceneLoaded -= OnSceneChanged;
        }

        private bool IsPanelShowing()
        {
            return _isPanelShowing;
        }
        
        private void OnSceneChanged(Scene scene, LoadSceneMode mode)
        {
            ClosePanel();
            _isPanelShowing = false;
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
        }
    }
}
