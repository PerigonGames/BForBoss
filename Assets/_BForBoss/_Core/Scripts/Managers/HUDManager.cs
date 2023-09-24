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

        public void Initialize(ILifeCycle playerLifeCycle, IEnergyDataSubject energyDataSubject)
        {
            StateManager.Instance.OnStateChanged += HandleOnStateChanged;
            _playerHealthViewBehaviour.Initialize(playerLifeCycle);
            _energySystemViewBehaviour.Initialize(energyDataSubject);
        }

        public void Reset()
        {
            _incomingAttackIndicatorView.Reset();
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
                    break;
            }
        }

        private void Awake()
        {
            this.PanicIfNullObject(_playerHealthViewBehaviour, nameof(_playerHealthViewBehaviour));
        }

        private void OnDestroy()
        {
            StateManager.Instance.OnStateChanged -= HandleOnStateChanged;
        }
    }
}
