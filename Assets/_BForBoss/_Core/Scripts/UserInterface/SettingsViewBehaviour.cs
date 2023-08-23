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
        
        [Title("Views")]
        [Resolve][SerializeField] private InputSettingsViewBehaviour _settingsSensitivityView = null;
        [Resolve][SerializeField] private TelemetricsViewBehaviour _settingsTelemetryView = null;
        [Resolve][SerializeField] private AudioSettingsViewBehaviour _settingsAudioView = null;
        
        private Action OnBackPressed;

        public void Initialize(Action onBackPressed)
        {
            base.Initialize();
            _settingsSensitivityView.Initialize(new MouseKeyboardInputSettingsViewModel());
            _settingsTelemetryView.Initialize();
            _settingsAudioView.Initialize();
            OnBackPressed = onBackPressed;
        }

        private void Awake()
        {
            _backButton.onClick.AddListener(() =>
            {
                OnBackPressed();
            });

            this.PanicIfNullObject(_settingsSensitivityView, nameof(_settingsSensitivityView));
            this.PanicIfNullObject(_settingsTelemetryView, nameof(_settingsTelemetryView));
            this.PanicIfNullObject(_settingsAudioView, nameof(_settingsAudioView));
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
        }
    }
}