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
            _playerHealthViewBehaviour.gameObject.SetActive(gameState != State.EndGame);
            _energySystemViewBehaviour.gameObject.SetActive(gameState != State.EndGame);
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
