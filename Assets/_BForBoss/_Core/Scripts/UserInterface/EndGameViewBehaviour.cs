using Perigon.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class EndGameViewBehaviour : MonoBehaviour
    {
        private const string VictoryLabel = "Victory";
        private const string GameOverLabel = "Game Over";
            
        private readonly QuitGameUseCase _quitGameUseCase = new QuitGameUseCase();
        private ResetGameUseCase _resetGameUseCase;

        [Resolve][SerializeField] private TMP_Text _title;
        [Resolve][SerializeField] private Button _resetButton;
        [Resolve][SerializeField] private Button _quitGameButton;

        public void Initialize(ResetGameUseCase resetGameUseCase)
        {
            _resetGameUseCase = resetGameUseCase;
            _quitGameButton.onClick.AddListener(_quitGameUseCase.Execute);
            _resetButton.onClick.AddListener(_resetGameUseCase.Execute);
        }

        private void OnEnable()
        {
            switch (StateManager.Instance.GetState())
            {
                case State.Death:
                    _title.text = GameOverLabel;
                    break;
                case State.EndGame:
                    _title.text = VictoryLabel;
                    break;
                default:
                    return;
            }
        }

        private void Awake()
        {
            this.PanicIfNullObject(_resetButton, nameof(_resetButton));
            this.PanicIfNullObject(_quitGameButton, nameof(_quitGameButton));
            this.PanicIfNullObject(_title, nameof(_title));
        }

        private void OnDestroy()
        {
            _quitGameButton.onClick.RemoveAllListeners();
            _resetButton.onClick.RemoveAllListeners();
        }
    }
}
