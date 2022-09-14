using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class PauseViewBehaviour : MonoBehaviour
    {
        private readonly QuitGameUseCase _quitGameUseCase = new QuitGameUseCase();

        [Title("Buttons")]
        [SerializeField] private Button _resumeButton = null;
        [SerializeField] private Button _resetButton = null;
        [SerializeField] private Button _quitButton = null;
        [SerializeField] private Button _settingsButton = null;

        private Action _onSettingsPressed;
        private ResetGameUseCase _resetGameUseCase;

        public void Initialize(ResetGameUseCase resetGameUseCase, Action onSettingsPressed)
        {
            _resetGameUseCase = resetGameUseCase;
            _onSettingsPressed = onSettingsPressed;
        }

        private void ResumeGame()
        {
            StateManager.Instance.SetState(State.Play);
        }
        
        private void Awake()
        {
            _resumeButton.onClick.AddListener(ResumeGame);
            _resetButton.onClick.AddListener(_resetGameUseCase.Execute);
            _quitButton.onClick.AddListener(_quitGameUseCase.Execute);
            _settingsButton.onClick.AddListener(() =>
            {
                _onSettingsPressed?.Invoke();
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