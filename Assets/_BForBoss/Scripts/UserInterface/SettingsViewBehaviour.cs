using PerigonGames;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class SettingsViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Button _backButton = null;

        private InputSettingsViewBehaviour _inputSettingsView = null;
        private SetUsernameViewBehaviour _setUsernameView = null;
        private LeaderboardPanelBehaviour _leaderboardView = null;
        private TabbedPanelViewBehaviour _tabbedPanelViews = null;
        
        public void Initialize(IInputSettings inputSettings, ILockInput lockInput)
        {
            _inputSettingsView.Initialize(new InputSettingsViewModel(inputSettings));
            _setUsernameView.Initialize(lockInput);
            _tabbedPanelViews.Initialize();
        }

        public void OpenPanel()
        {
            transform.ResetScale();
        }

        public void OpenLeaderboard(int time, string input)
        {
            OpenPanel();
            _tabbedPanelViews.TurnOffAllContent();
            _leaderboardView.gameObject.SetActive(true);
            _leaderboardView.SetUserScore(time, input);
        }

        private void Awake()
        {
            transform.localScale = Vector3.zero;
            _backButton.onClick.AddListener(() =>
            {
                transform.localScale = Vector3.zero;
            });
            SetupViews();
        }

        private void SetupViews()
        {
            _inputSettingsView = GetComponentInChildren<InputSettingsViewBehaviour>();
            _setUsernameView =  GetComponentInChildren<SetUsernameViewBehaviour>();
            _leaderboardView = GetComponentInChildren<LeaderboardPanelBehaviour>();
            _tabbedPanelViews = GetComponentInChildren<TabbedPanelViewBehaviour>();

        }
    }
}