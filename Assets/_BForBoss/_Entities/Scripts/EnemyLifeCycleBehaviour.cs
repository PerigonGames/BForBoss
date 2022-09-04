using System;
using UnityEngine;

namespace Perigon.Entities
{
    public class EnemyLifeCycleBehaviour : LifeCycleBehaviour
    {
        [SerializeField] private HealthbarViewBehaviour _healthbar;

        //TODO - don't do this.knockback should rely on something else
        public ILifeCycle LifeCycle => _lifeCycle;
        private Action _releaseToPool;
        
        public void Initialize(Action releaseToPool)
        {
            base.Initialize();
            _releaseToPool = releaseToPool;
            _healthbar.Initialize(_lifeCycle);
        }

        public override void Reset()
        {
            base.Reset();
            _healthbar.Reset();
        }
        
        protected override void LifeCycleFinished()
        {
            _releaseToPool?.Invoke();
        }
        
        private void Awake()
        {
            if (_healthbar == null)
            {                
                Debug.LogWarning("A FloatingTargetBehaviour is missing a health bar");
            }
        }
    }
}
