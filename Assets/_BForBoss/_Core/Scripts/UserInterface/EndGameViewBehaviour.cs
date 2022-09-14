using Perigon.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class EndGameViewBehaviour : MonoBehaviour
    {
        private readonly QuitGameUseCase _quitGameUseCase = new QuitGameUseCase();
        private ResetGameUseCase _resetGameUseCase;

        [Resolve][SerializeField] private Button _resetButton;
        [Resolve][SerializeField] private Button _quitGameButton;

        public void Initialize(ResetGameUseCase resetGameUseCase)
        {
            _resetGameUseCase = resetGameUseCase;
            _quitGameButton.onClick.AddListener(_quitGameUseCase.Execute);
            _resetButton.onClick.AddListener(_resetGameUseCase.Execute);
        }

        private void OnDestroy()
        {
            _quitGameButton.onClick.RemoveAllListeners();
            _resetButton.onClick.RemoveAllListeners();
        }
    }
}
