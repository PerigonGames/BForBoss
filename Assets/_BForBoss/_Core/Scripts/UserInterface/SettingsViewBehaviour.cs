using System;
using Perigon.UserInterface;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class SettingsViewBehaviour : TabbedPanelViewBehaviour
    {
        [Title("Buttons")]
        [Resolve][SerializeField] private Button _backButton = null;
        
        [Title("Panel")]
        [SerializeField] private InputSettingsViewBehaviour _mouseKeyboardInputSettingsView = null;
        [SerializeField] private InputSettingsViewBehaviour _controllerInputSettingsView = null;
        private GameplaySettingsViewBehaviour _gameplaySettingsView = null;
        private AudioSettingsViewBehaviour _audioSettingsView = null;
        
        private Action OnBackPressed;

        public void Initialize(Action onBackPressed)
        {
            base.Initialize();
            _mouseKeyboardInputSettingsView.Initialize(new MouseKeyboardInputSettingsViewModel());
            _controllerInputSettingsView.Initialize(new ControllerInputSettingsViewModel());
            _gameplaySettingsView.Initialize();
            _audioSettingsView.Initialize();
            OnBackPressed = onBackPressed;
        }

        private void Awake()
        {
            if (_mouseKeyboardInputSettingsView == null)
            {
                Debug.LogWarning("MouseAndKeyboardInputSettingsView is missing from SettingsViewBehaviour");
            }
            
            if (_controllerInputSettingsView == null)
            {
                Debug.LogWarning("ControllerInputSettingsView is missing from SettingsViewBehaviour");
            }
            
            _backButton.onClick.AddListener(() =>
            {
                OnBackPressed();
            });
            
            _gameplaySettingsView = GetComponentInChildren<GameplaySettingsViewBehaviour>(true);
            _audioSettingsView = GetComponentInChildren<AudioSettingsViewBehaviour>(true);
            
            this.PanicIfNullObject(_gameplaySettingsView, nameof(_gameplaySettingsView));
            this.PanicIfNullObject(_audioSettingsView, nameof(_audioSettingsView));
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
        }
    }
}