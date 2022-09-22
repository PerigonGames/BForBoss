using System;
using UnityEngine;

namespace Perigon.Entities
{
    public class EnemyLifeCycleBehaviour : LifeCycleBehaviour
    {
        [SerializeField] private HealthBarViewBehaviour _healthBar;

        //TODO - don't do this.knockback should rely on something else
        public ILifeCycle LifeCycle => _lifeCycle;
        private Action _onDeath;
        
        public void Initialize(Action onDeath)
        {
            base.Initialize();
            _onDeath = onDeath;
            _healthBar.Initialize(_lifeCycle);
        }

        public override void Reset()
        {
            base.Reset();
            _healthBar.gameObject.SetActive(true);
            _healthBar.Reset();
        }
        
        protected override void LifeCycleFinished()
        {
            _healthBar.gameObject.SetActive(false);
            _onDeath?.Invoke();
        }
        
        private void Awake()
        {
            if (_healthBar == null)
            {                
                Debug.LogWarning("A FloatingTargetBehaviour is missing a health bar");
            }
        }
    }
}
