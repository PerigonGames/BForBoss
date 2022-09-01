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

        public void Initialize(IInputConfiguration inputConfiguration, Action onBackPressed)
        {
            base.Initialize();
            _mouseKeyboardInputSettingsView.Initialize(new MouseKeyboardInputSettingsViewModel(inputConfiguration));
            _controllerInputSettingsView.Initialize(new ControllerInputSettingsViewModel(inputConfiguration));
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

            if (_gameplaySettingsView == null)
            {
                PanicHelper.Panic(new Exception("Gameplay Settings View Missing from SettingsViewBehaviour"));
            }
            
            if (_audioSettingsView == null)
            {
                PanicHelper.Panic(new Exception("Audio Settings View Missing from SettingsViewBehaviour"));
            }
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
        }
    }
}