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
        
        private ILockInput _lockInput;
        private State _stateWhenPaused;
        
        public void Initialize(IInputSettings inputSettings, ILockInput lockInput)
        {
            _lockInput = lockInput;
            _settingsView.Initialize(inputSettings, lockInput);
            _resumeButton.onClick.AddListener(ResumeGame);
            _resetButton.onClick.AddListener(ResetGame);
            _quitButton.onClick.AddListener(QuitGame);
            _settingsButton.onClick.AddListener(OpenSettings);
            ClosePanel();
        }

        public void ForceOpenLeaderboardWithScore(int time, string input)
        {
            _lockInput.LockInput();
            OpenPanel();
            _settingsView.OpenLeaderboard(time, input);
        }

        private void OpenPanel()
        {
            transform.ResetScale();
        }
        
        private void ClosePanel()
        {
            transform.localScale = Vector3.zero;
        }
        
        private void PauseGame()
        {
            _stateWhenPaused = StateManager.Instance.GetState();
            StateManager.Instance.SetState(State.Pause);
            _lockInput.LockInput();
            OpenPanel();
        }
        
        private void ResumeGame()
        {
            _lockInput.UnlockInput();
            StateManager.Instance.SetState(_stateWhenPaused);
            ClosePanel();
        }
        
        private void ResetGame()
        {
            _lockInput.UnlockInput();
            StateManager.Instance.SetState(State.PreGame);
            ClosePanel();
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        private void OpenSettings()
        {
            _settingsView.OpenPanel();
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (StateManager.Instance.GetState() == State.Pause)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
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