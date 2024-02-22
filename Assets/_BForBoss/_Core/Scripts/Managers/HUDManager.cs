using Perigon.Entities;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class HUDManager : MonoBehaviour
    {
        [Resolve] [SerializeField] private PlayerHealthViewBehaviour _playerHealthViewBehaviour;
        [Resolve] [SerializeField] private EnergySystemViewBehaviour _energySystemViewBehaviour;
        [Resolve] [SerializeField] private IncomingAttackIndicatorView _incomingAttackIndicatorView;
        [Resolve] [SerializeField] private CountdownViewBehaviour _countdownViewBehaviour;
        public void Initialize(ILifeCycle playerLifeCycle, IEnergyDataSubject energyDataSubject)
        {
            StateManager.Instance.OnStateChanged += HandleOnStateChanged;
            _playerHealthViewBehaviour.Initialize(playerLifeCycle);
            _energySystemViewBehaviour.Initialize(energyDataSubject);
        }

        public void Reset()
        {
            _incomingAttackIndicatorView.Reset();
            _countdownViewBehaviour.Reset();
        }

        private void HandleOnStateChanged(State gameState)
        {
            switch (gameState)
            {
                case State.Tutorial:
                case State.EndGame:
                    _playerHealthViewBehaviour.gameObject.SetActive(false);
                    _energySystemViewBehaviour.gameObject.SetActive(false);
                    _incomingAttackIndicatorView.gameObject.SetActive(false);
                    break;    
                default:
                    _playerHealthViewBehaviour.gameObject.SetActive(true);
                    _energySystemViewBehaviour.gameObject.SetActive(true);
                    _incomingAttackIndicatorView.gameObject.SetActive(true);
                    _countdownViewBehaviour.gameObject.SetActive(true);
                    break;
            }
        }

        private void Awake()
        {
            this.PanicIfNullObject(_playerHealthViewBehaviour, nameof(_playerHealthViewBehaviour));
            this.PanicIfNullObject(_energySystemViewBehaviour, nameof(_energySystemViewBehaviour));
            this.PanicIfNullObject(_incomingAttackIndicatorView, nameof(_incomingAttackIndicatorView));
            this.PanicIfNullObject(_countdownViewBehaviour, nameof(_countdownViewBehaviour));
        }

        private void OnDestroy()
        {
            StateManager.Instance.OnStateChanged -= HandleOnStateChanged;
        }
    }
}
