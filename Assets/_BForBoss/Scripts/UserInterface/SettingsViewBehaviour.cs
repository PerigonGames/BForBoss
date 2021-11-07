using PerigonGames;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BForBoss
{
    public class SettingsViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Button _backButton = null;
        
        private InputSettingsViewBehaviour _inputSettingsView = null;

        private InputSettingsViewBehaviour InputSettingsView
        {
            get
            {
                if (_inputSettingsView == null)
                {
                    _inputSettingsView = GetComponentInChildren<InputSettingsViewBehaviour>();
                }

                return _inputSettingsView;
            }   
        }

        public void Initialize(IInputSettings inputSettings)
        {
            InputSettingsView.Initialize(new InputSettingsViewModel(inputSettings));
        }

        private void Update()
        {
            if (Keyboard.current.digit0Key.isPressed)
            {
                LockMouseUtility.Instance.UnlockMouse();
                transform.ResetScale();
            }
        }

        private void Awake()
        {
            _backButton.onClick.AddListener(() =>
            {
                LockMouseUtility.Instance.LockMouse();
                transform.localScale = Vector3.zero;
            });
        }
    }
}