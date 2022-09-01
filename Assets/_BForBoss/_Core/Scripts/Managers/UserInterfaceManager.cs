using System;
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
            SettingsView
        }
        
        [SerializeField] private SettingsViewBehaviour _settingsViewBehaviour = null;
        [SerializeField] private PauseViewBehaviour _pauseViewBehaviour = null;
        
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
            StateManager.Instance.OnStateChanged += HandleOnStateChanged;
            _settingsViewBehaviour.Initialize(onBackPressed: () =>
            {
                UIState = UserInterfaceState.PauseView;
            });
            _pauseViewBehaviour.Initialize(onSettingsPressed: () =>
            {
                UIState = UserInterfaceState.SettingsView;
            });
        }

        private void HandleOnStateChanged(State gameState)
        {
            if (gameState == State.Pause)
            {
                UIState = UserInterfaceState.PauseView;
            }
            else
            {
                UIState = UserInterfaceState.None;
            }
        }

        private void OnUserInterfaceStateChanged()
        {
            switch (_state)
            {
                case UserInterfaceState.None:
                    _settingsViewBehaviour.gameObject.SetActive(false);
                    _pauseViewBehaviour.gameObject.SetActive(false);
                    break;
                case UserInterfaceState.PauseView:
                    _settingsViewBehaviour.gameObject.SetActive(false);
                    _pauseViewBehaviour.gameObject.SetActive(true);
                    break;
                case UserInterfaceState.SettingsView:
                    _settingsViewBehaviour.gameObject.SetActive(true);
                    _pauseViewBehaviour.gameObject.SetActive(false);
                    break;
            }
        }

        private void Awake()
        {
            if (_settingsViewBehaviour == null)
            {
                PanicHelper.Panic(new Exception("_settingsViewBehaviour missing from UserInterfaceManager"));
            }
            
            if (_pauseViewBehaviour == null)
            {
                PanicHelper.Panic(new Exception("_pauseViewBehaviour missing from UserInterfaceManager"));
            }
        }

        private void OnDestroy()
        {
            StateManager.Instance.OnStateChanged -= HandleOnStateChanged;
        }
    }
}