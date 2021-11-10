using System;
using PerigonGames;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BForBoss
{
    public class PauseMenu : MonoBehaviour
    {
        [Title("Panels")] 
        [SerializeField] private SettingsViewBehaviour _settingsView = null;
        [Title("Buttons")]
        [SerializeField] private Button _resumeButton = null;
        [SerializeField] private Button _resetButton = null;
        [SerializeField] private Button _quitButton = null;
        [SerializeField] private Button _settingsButton = null;
        
        private IInputSettings _inputSettings;
        private State _stateWhenPaused;
        
        public void Initialize(IInputSettings inputSettings)
        {
            _inputSettings = inputSettings;
            _settingsView.Initialize(inputSettings);
            _resumeButton.onClick.AddListener(ResumeGame);
            _resetButton.onClick.AddListener(ResetGame);
            _quitButton.onClick.AddListener(QuitGame);
            _settingsButton.onClick.AddListener(OpenSettings);
            ClosePanel();
        }

        private void OpenPanel()
        {
            transform.ResetScale();
        }
        
        private void ClosePanel()
        {
            transform.localScale = Vector3.zero;
        }
        
        private void HandleOnPause()
        {
            _stateWhenPaused = StateManager.Instance.GetState();
            StateManager.Instance.SetState(State.Pause);
            LockCharacterFunctionality(_inputSettings);
        }
        
        private void ResumeGame()
        {
            UnlockCharacterFunctionality(_inputSettings);
            StateManager.Instance.SetState(_stateWhenPaused);
        }
        
        private void ResetGame()
        {
            UnlockCharacterFunctionality(_inputSettings);
            StateManager.Instance.SetState(State.PreGame);
        }

        private void QuitGame()
        {
            Application.Quit();

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
        }

        private void OpenSettings()
        {
            _settingsView.OpenPanel();
        }
        
        private void LockCharacterFunctionality(IInputSettings inputSettings)
        {
            LockMouseUtility.Instance.UnlockMouse();
            inputSettings.DisableActions();
        }

        private void UnlockCharacterFunctionality(IInputSettings inputSettings)
        {
            LockMouseUtility.Instance.LockMouse();
            inputSettings.EnableActions();
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (StateManager.Instance.GetState() == State.Pause)
                {
                    ClosePanel();
                    ResumeGame();
                }
                else
                {
                    OpenPanel();
                    HandleOnPause();
                }
            }
        }

        private void OnDestroy()
        {
            _resumeButton.onClick.RemoveAllListeners();
            _resetButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
        }
    }
}