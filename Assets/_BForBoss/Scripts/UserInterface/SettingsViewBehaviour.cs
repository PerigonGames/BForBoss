using PerigonGames;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BForBoss
{
    public class SettingsViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Button _backButton = null;
        
        private InputSettingsViewBehaviour InputSettingsView => GetComponentInChildren<InputSettingsViewBehaviour>();
        private InputUsernameViewBehaviour InputUsernameView => GetComponentInChildren<InputUsernameViewBehaviour>();
        
        public void Initialize(IInputSettings inputSettings)
        {
            InputSettingsView.Initialize(new InputSettingsViewModel(inputSettings));
            InputUsernameView.Initialize();
            GetComponentInChildren<TabbedPanelViewBehaviour>().Initialize();
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
            transform.localScale = Vector3.zero;
            _backButton.onClick.AddListener(() =>
            {
                LockMouseUtility.Instance.LockMouse();
                transform.localScale = Vector3.zero;
            });
        }
    }
}