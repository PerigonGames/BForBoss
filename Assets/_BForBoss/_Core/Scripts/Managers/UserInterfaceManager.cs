using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class UserInterfaceManager : MonoBehaviour
    {
        private enum UserInterfaceState
        {
            None,
            PauseView,
            SettingsView,
            TutorialView,
            EndGameView
        }
        
        [SerializeField] private SettingsViewBehaviour _settingsViewBehaviour;
        [SerializeField] private PauseViewBehaviour _pauseViewBehaviour;
        [SerializeField] private EndGameViewBehaviour _endGameViewBehaviour;
        [SerializeField] private TutorialCanvas _tutorialCanvas;
        
        private UserInterfaceState UIState
        {
            set
            {
                _state = value;
                OnUserInterfaceStateChanged();
            }
        }

        private UserInterfaceState _state = UserInterfaceState.None;
        
        public void Initialize()
        {
            StateManager.Instance.OnStateChanged += HandleOnStateChangedForUserInterface;
            var resetGameUseCase = new ResetGameUseCase(StateManager.Instance);
            _settingsViewBehaviour.Initialize(onBackPressed: () =>
            {
                UIState = UserInterfaceState.PauseView;
            });
            _tutorialCanvas.Initialize(onBackPressed: () =>
            {
                UIState = UserInterfaceState.PauseView;
            });
            
            _pauseViewBehaviour.Initialize(
                resetGameUseCase, 
                onSettingsPressed: () =>
            {
                UIState = UserInterfaceState.SettingsView;
            }, 
                onTutorialPressed: () =>
            {
                UIState = UserInterfaceState.TutorialView;
            });
            _endGameViewBehaviour.Initialize(resetGameUseCase);
        }

        private void HandleOnStateChangedForUserInterface(State gameState)
        {
            switch (gameState)
            {
                case State.Pause:
                    UIState = UserInterfaceState.PauseView;
                    break;
                case State.Death:
                case State.EndGame:
                    UIState = UserInterfaceState.EndGameView;
                    break;
                default:
                    UIState = UserInterfaceState.None;
                    break;
            }
        }

        private void OnUserInterfaceStateChanged()
        {
            switch (_state)
            {
                case UserInterfaceState.None:
                    _settingsViewBehaviour.gameObject.SetActive(false);
                    _pauseViewBehaviour.gameObject.SetActive(false);
                    _endGameViewBehaviour.gameObject.SetActive(false);
                    _tutorialCanvas.gameObject.SetActive(false);
                    break;
                case UserInterfaceState.PauseView:
                    _settingsViewBehaviour.gameObject.SetActive(false);
                    _pauseViewBehaviour.gameObject.SetActive(true);
                    _tutorialCanvas.gameObject.SetActive(false);
                    break;
                case UserInterfaceState.SettingsView:
                    _settingsViewBehaviour.gameObject.SetActive(true);
                    _pauseViewBehaviour.gameObject.SetActive(false);
                    break;
                case UserInterfaceState.TutorialView:
                    _settingsViewBehaviour.gameObject.SetActive(false);
                    _pauseViewBehaviour.gameObject.SetActive(false);
                    _tutorialCanvas.gameObject.SetActive(true);
                    break;
                case UserInterfaceState.EndGameView:
                    _settingsViewBehaviour.gameObject.SetActive(false);
                    _pauseViewBehaviour.gameObject.SetActive(false);
                    _endGameViewBehaviour.gameObject.SetActive(true);
                    _tutorialCanvas.gameObject.SetActive(false);
                    break;
            }
        }

        private void Awake()
        {
            this.PanicIfNullObject(_settingsViewBehaviour, nameof(_settingsViewBehaviour));
            this.PanicIfNullObject(_pauseViewBehaviour, nameof(_pauseViewBehaviour));
            this.PanicIfNullObject(_endGameViewBehaviour, nameof(_endGameViewBehaviour));
            this.PanicIfNullObject(_tutorialCanvas, nameof(_tutorialCanvas));
        }

        private void OnDestroy()
        {
            StateManager.Instance.OnStateChanged -= HandleOnStateChangedForUserInterface;
        }
    }
}