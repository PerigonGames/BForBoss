using UnityEngine;

namespace Perigon.Entities
{
    public abstract class HealthViewBehaviour : MonoBehaviour
    {
        protected ILifeCycle _lifeCycle = null;

        public void Initialize(ILifeCycle lifeCycle)
        {
            _lifeCycle = lifeCycle;
            _lifeCycle.OnDamageTaken += OnHealthChanged;
            _lifeCycle.OnHeal += OnHealthChanged;
            OnHealthChanged();
        }

        public abstract void Reset();
        protected abstract void OnHealthChanged();
        
        protected float GetHealthPercentage()
        {
            if (_lifeCycle != null)
            {
                var maxHealth = Mathf.Clamp(_lifeCycle.MaxHealth, 1, float.MaxValue);
                return _lifeCycle.CurrentHealth / maxHealth;
            }
            
            return 0f;
        }

        private void OnDestroy()
        {
            if (_lifeCycle != null)
            {
                _lifeCycle.OnHeal -= OnHealthChanged;
                _lifeCycle.OnDamageTaken -= OnHealthChanged;
            }
        }
    }
}
