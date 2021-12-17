using System;
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
        [SerializeField] private Button _backButton = null;
        
        [Title("Panel")]
        [SerializeField] private InputSettingsViewBehaviour _mouseKeyboardInputSettingsView = null;
        [SerializeField] private InputSettingsViewBehaviour _controllerInputSettingsView = null;
        private SetUsernameViewBehaviour _setUsernameView = null;
        private LeaderboardPanelBehaviour _leaderboardView = null;
        private TabbedPanelViewBehaviour _tabbedPanelViews = null;
        private GameplaySettingsViewBehaviour _gameplaySettingsView = null;
        private ILockInput _lockInput = null;
        
        public void Initialize(
            IThirdPerson thirdPersonSettings,
            IInputSettings inputSettings, 
            ILockInput lockInput)
        {
            _lockInput = lockInput;
            if (_mouseKeyboardInputSettingsView != null)
            {
                _mouseKeyboardInputSettingsView.Initialize(new MouseKeyboardInputSettingsViewModel(inputSettings));
            }
            _setUsernameView?.Initialize(lockInput);
            _tabbedPanelViews?.Initialize();
            _gameplaySettingsView?.Initialize(thirdPersonSettings);
        }

        public void OpenPanel()
        {
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
                Debug.LogWarning("Mouse and Keyboard Input Settings View Is Missing From Settings View Behaviour ");
            }
            
            if (_controllerInputSettingsView == null)
            {
                Debug.LogWarning("Controller Input Settings View Is Missing From Settings View Behaviour ");
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
            _setUsernameView =  GetComponentInChildren<SetUsernameViewBehaviour>();
            _leaderboardView = GetComponentInChildren<LeaderboardPanelBehaviour>();
            _tabbedPanelViews = GetComponentInChildren<TabbedPanelViewBehaviour>();
            _gameplaySettingsView = GetComponentInChildren<GameplaySettingsViewBehaviour>();
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
        }
    }
}