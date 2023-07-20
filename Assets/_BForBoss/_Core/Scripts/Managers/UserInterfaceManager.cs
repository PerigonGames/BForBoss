using System;
using Perigon.Entities;
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
            EndGameView
        }
        
        [SerializeField] private SettingsViewBehaviour _settingsViewBehaviour;
        [SerializeField] private PauseViewBehaviour _pauseViewBehaviour;
        [SerializeField] private EndGameViewBehaviour _endGameViewBehaviour;
        
        private UserInterfaceState UIState
        {
            set
            {
                _state = value;
                OnUserInterfaceStateChanged();
            }
        }

        private UserInterfaceState _state = UserInterfaceState.None;
        
        public void Initialize(ILifeCycle playerLifeCycle)
        {
            StateManager.Instance.OnStateChanged += HandleOnStateChangedForUserInterface;
            var resetGameUseCase = new ResetGameUseCase(StateManager.Instance);
            _settingsViewBehaviour.Initialize(onBackPressed: () =>
            {
                UIState = UserInterfaceState.PauseView;
            });
            _pauseViewBehaviour.Initialize(resetGameUseCase, onSettingsPressed: () =>
            {
                UIState = UserInterfaceState.SettingsView;
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
                    break;
                case UserInterfaceState.PauseView:
                    _settingsViewBehaviour.gameObject.SetActive(false);
                    _pauseViewBehaviour.gameObject.SetActive(true);
                    break;
                case UserInterfaceState.SettingsView:
                    _settingsViewBehaviour.gameObject.SetActive(true);
                    _pauseViewBehaviour.gameObject.SetActive(false);
                    break;
                case UserInterfaceState.EndGameView:
                    _settingsViewBehaviour.gameObject.SetActive(false);
                    _pauseViewBehaviour.gameObject.SetActive(false);
                    _endGameViewBehaviour.gameObject.SetActive(true);
                    break;
            }
        }

        private void Awake()
        {
            this.PanicIfNullObject(_settingsViewBehaviour, nameof(_settingsViewBehaviour));
            this.PanicIfNullObject(_pauseViewBehaviour, nameof(_pauseViewBehaviour));
            this.PanicIfNullObject(_endGameViewBehaviour, nameof(_endGameViewBehaviour));
        }

        private void OnDestroy()
        {
            StateManager.Instance.OnStateChanged -= HandleOnStateChangedForUserInterface;
        }
    }
}