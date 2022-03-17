using Perigon.Character;
using Perigon.Leaderboard;
using Perigon.UserInterface;
using Perigon.Utility;
using PerigonGames;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class SettingsViewBehaviour : MonoBehaviour
    {
        [Title("Buttons")]
        [Resolve][SerializeField] private Button _backButton = null;
        
        [Title("Panel")]
        [SerializeField] private InputSettingsViewBehaviour _mouseKeyboardInputSettingsView = null;
        [SerializeField] private InputSettingsViewBehaviour _controllerInputSettingsView = null;
        private SetUsernameViewBehaviour _setUsernameView = null;
        private LeaderboardPanelBehaviour _leaderboardView = null;
        private TabbedPanelViewBehaviour _tabbedPanelViews = null;
        private GameplaySettingsViewBehaviour _gameplaySettingsView = null;
        private ILockInput _lockInput = null;
        private AudioSettingsViewBehaviour _audioSettingsView = null;
        
        public void Initialize(
            IInputSettings inputSettings, 
            ILockInput lockInput)
        {
            _lockInput = lockInput;
            _mouseKeyboardInputSettingsView.Initialize(new MouseKeyboardInputSettingsViewModel(inputSettings));
            _controllerInputSettingsView?.Initialize(new ControllerInputSettingsViewModel(inputSettings));
            _setUsernameView?.Initialize(lockInput);
            _tabbedPanelViews?.Initialize();
            _gameplaySettingsView?.Initialize();
            _audioSettingsView?.Initialize();
        }

        public void OpenPanel()
        {
            _tabbedPanelViews.Reset();
            transform.ResetScale();
        }

        public void ClosePanel()
        {
            transform.localScale = Vector3.zero;
            UnlockInputIfEndRaceState();
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
            _backButton.onClick.AddListener(ClosePanel);
            SetupViews();
        }

        private void OnValidate()
        {
            if (_mouseKeyboardInputSettingsView == null)
            {
                Debug.LogWarning("MouseAndKeyboardInputSettingsView is missing from SettingsViewBehaviour ");
            }
            
            if (_controllerInputSettingsView == null)
            {
                Debug.LogWarning("ControllerInputSettingsView is missing from SettingsViewBehaviour ");
            }
        }

        private void UnlockInputIfEndRaceState()
        {
            if (StateManager.Instance.GetState() == State.EndRace)
            {
                _lockInput.UnlockInput();
            }
        }

        private void SetupViews()
        {
            _setUsernameView =  GetComponentInChildren<SetUsernameViewBehaviour>(true);
            _leaderboardView = GetComponentInChildren<LeaderboardPanelBehaviour>(true);
            _tabbedPanelViews = GetComponentInChildren<TabbedPanelViewBehaviour>(true);
            _gameplaySettingsView = GetComponentInChildren<GameplaySettingsViewBehaviour>(true);
            _audioSettingsView = GetComponentInChildren<AudioSettingsViewBehaviour>(true);
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
        }
    }
}