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
            if (Keyboard.current[_keyCodeModifier].wasPressedThisFrame &&
                Keyboard.current[_keyCodeCharacter].wasPressedThisFrame)
            {
                Debug.Log("Hopefully this works");

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
            
        }

        private void ResetView()
        {
            
        }

        private void OpenPanel()
        {
            GetCanvasRect();
            transform.localScale = Vector3.one;
            SceneManager.sceneLoaded += OnSceneChanged;
        }

        private void ClosePanel()
        {
            ResetView();
            transform.localScale = Vector3.one;
            SceneManager.sceneLoaded -= OnSceneChanged;
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
                _windowRect = new Rect(_rectTransform.anchorMin.x * Screen.width,
                    _rectTransform.anchorMin.y * Screen.height, _rectTransform.rect.width,
                    _rectTransform.rect.height);
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            GetCanvasRect();
        }
    }
}
