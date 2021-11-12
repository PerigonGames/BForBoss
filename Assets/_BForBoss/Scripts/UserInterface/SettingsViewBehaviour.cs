using PerigonGames;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class SettingsViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Button _backButton = null;
        
        private InputSettingsViewBehaviour InputSettingsView => GetComponentInChildren<InputSettingsViewBehaviour>();
        private SetUsernameViewBehaviour SetUsernameView => GetComponentInChildren<SetUsernameViewBehaviour>();
        private TabbedPanelViewBehaviour TabbedPanelViews => GetComponentInChildren<TabbedPanelViewBehaviour>();
        
        public void Initialize(IInputSettings inputSettings)
        {
            InputSettingsView.Initialize(new InputSettingsViewModel(inputSettings));
            SetUsernameView.Initialize();
            TabbedPanelViews.Initialize();
        }

        public void OpenPanel()
        {
            transform.ResetScale();
        }

        private void Awake()
        {
            transform.localScale = Vector3.zero;
            _backButton.onClick.AddListener(() =>
            {
                transform.localScale = Vector3.zero;
            });
        }
    }
}