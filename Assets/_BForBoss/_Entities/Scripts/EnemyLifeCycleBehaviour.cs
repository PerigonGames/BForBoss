using System;
using UnityEngine;

namespace Perigon.Entities
{
    public class EnemyLifeCycleBehaviour : LifeCycleBehaviour
    {
        [SerializeField] private EnemyHealthBarViewBehaviour _enemyHealthBar;

        //TODO - don't do this.knockback should rely on something else
        public ILifeCycle LifeCycle => _lifeCycle;
        private Action _onDeath;
        
        public void Initialize(Action onDeath)
        {
            base.Initialize();
            _onDeath = onDeath;
            _enemyHealthBar.Initialize(_lifeCycle);
        }

        public override void Reset()
        {
            base.Reset();
            _enemyHealthBar.gameObject.SetActive(true);
            _enemyHealthBar.Reset();
        }
        
        protected override void LifeCycleFinished()
        {
            _enemyHealthBar.gameObject.SetActive(false);
            _onDeath?.Invoke();
        }
        
        private void Awake()
        {
            if (_enemyHealthBar == null)
            {                
                Debug.LogWarning("A FloatingTargetBehaviour is missing a health bar");
            }
        }
    }
}
