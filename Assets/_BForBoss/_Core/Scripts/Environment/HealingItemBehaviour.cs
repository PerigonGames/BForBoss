using BForBoss;
using UnityEngine;

namespace Perigon.Entities
{
    public class HealingItemBehaviour : ItemPickupBehaviour
    {
        [SerializeField] private float _healAmount = 50f;
        private bool _shouldResetOnlyWhenWaveIncrements = false;
        private int _currentWaveNumber;
        
        public void Initialize(WaveModel waveModel)
        {
            _shouldResetOnlyWhenWaveIncrements = true;
            waveModel.OnDataUpdated += ResetItemIfWaveIncrements;
        }

        protected override bool DidPickUpItem(Collider other)
        {
            return other.TryGetComponent(out PlayerLifeCycleBehaviour lifeCycle) && !lifeCycle.IsFullHealth;
        }

        protected override void OnPickedUpItem(Collider other)
        {
            if (other.TryGetComponent(out PlayerLifeCycleBehaviour lifeCycle))
            {
                lifeCycle.HealBy(_healAmount);
            }
        }

        protected override void Update()
        {
            if (!_shouldResetOnlyWhenWaveIncrements)
            {
                base.Update();
            }
        }

        private void ResetItemIfWaveIncrements(int waveNumber, int enemyCount)
        {
            if (waveNumber == _currentWaveNumber || !_shouldResetOnlyWhenWaveIncrements || _isSpawned)
            {
                return;
            }
            
            IsShown(true);
            _currentWaveNumber = waveNumber;
        }

    }
}
