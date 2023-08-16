using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class PauseViewBehaviour : MonoBehaviour
    {
        private readonly QuitGameUseCase _quitGameUseCase = new QuitGameUseCase();

        [Title("Buttons")]
        [Resolve] [SerializeField] private Button _resumeButton = null;
        [Resolve] [SerializeField] private Button _resetButton = null;
        [Resolve] [SerializeField] private Button _tutorialButton = null;
        [Resolve] [SerializeField] private Button _quitButton = null;
        [Resolve] [SerializeField] private Button _settingsButton = null;

        private Action _onSettingsPressed;
        private Action _onTutorialPressed;
        private ResetGameUseCase _resetGameUseCase;

        public void Initialize(ResetGameUseCase resetGameUseCase, Action onSettingsPressed, Action onTutorialPressed)
        {
            _resetGameUseCase = resetGameUseCase;
            _onSettingsPressed = onSettingsPressed;
            _onTutorialPressed = onTutorialPressed;
            BindButtons();
        }

        private void ResumeGame()
        {
            StateManager.Instance.SetState(State.Play);
        }
        
        private void BindButtons()
        {
            _resumeButton.onClick.AddListener(ResumeGame);
            _resetButton.onClick.AddListener(_resetGameUseCase.Execute);
            _quitButton.onClick.AddListener(_quitGameUseCase.Execute);
            _settingsButton.onClick.AddListener(() =>
            {
                _onSettingsPressed?.Invoke();
            });
            _tutorialButton.onClick.AddListener(() =>
            {
                _onTutorialPressed?.Invoke();
            });
        }
        
        private void OnDestroy()
        {
            _resumeButton.onClick.RemoveAllListeners();
            _resetButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _tutorialButton.onClick.RemoveAllListeners();
        }
    }
}