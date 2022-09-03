using UnityEngine;

namespace Perigon.Entities
{
    public class EnemyLifeCycleBehaviour : LifeCycleBehaviour
    {
        [SerializeField] private HealthbarViewBehaviour _healthbar;

        //TODO - don't do this.knockback should rely on something else
        public ILifeCycle LifeCycle => _lifeCycle;
        
        public void Initialize()
        {
            _healthbar.Initialize(_lifeCycle);
        }

        public void Reset()
        {
            _healthbar.Reset();
        }
        
        protected override void LifeCycleFinished()
        {
            throw new System.NotImplementedException();
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
