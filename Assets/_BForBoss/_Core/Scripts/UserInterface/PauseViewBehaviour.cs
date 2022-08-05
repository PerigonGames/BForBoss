using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class PauseViewBehaviour : MonoBehaviour
    {
        [Title("Buttons")]
        [SerializeField] private Button _resumeButton = null;
        [SerializeField] private Button _resetButton = null;
        [SerializeField] private Button _quitButton = null;
        [SerializeField] private Button _settingsButton = null;

        private Action OnSettingsPressed;

        public void Initialize(Action onSettingsPressed)
        {
            OnSettingsPressed = onSettingsPressed;
        }

        private void ResetGame()
        {
            StateManager.Instance.SetState(State.PreGame);
        }

        private void ResumeGame()
        {
            StateManager.Instance.SetState(State.Play);
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
        
        private void Awake()
        {
            _resumeButton.onClick.AddListener(ResumeGame);
            _resetButton.onClick.AddListener(ResetGame);
            _quitButton.onClick.AddListener(QuitGame);
            _settingsButton.onClick.AddListener(() =>
            {
                OnSettingsPressed?.Invoke();
            });
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