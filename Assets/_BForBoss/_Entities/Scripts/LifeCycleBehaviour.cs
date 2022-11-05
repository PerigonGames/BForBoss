using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Entities
{
    public abstract class LifeCycleBehaviour : MonoBehaviour, IWeaponHolder
    {
        [InlineEditor]
        [SerializeField] private HealthScriptableObject _health = null;

        protected LifeCycle _lifeCycle;

        public bool IsAlive => _lifeCycle.IsAlive;

        public virtual void Initialize(LifeCycle lifeCycle = null)
        {
            _lifeCycle = lifeCycle ?? new LifeCycle();
            _lifeCycle.SetHealth(_health.Amount);
            _lifeCycle.OnDeath += LifeCycleFinished;
            _lifeCycle.OnDamageTaken += LifeCycleDamageTaken;
            _lifeCycle.OnHeal += LifeCycleOnHeal;
        }

        protected virtual void LifeCycleOnHeal()
        {
            
        }

        public void DamageBy(float amount)
        {
            _lifeCycle.DamageBy(amount);
        }
        
        public void HealBy(float amount)
        {
            _lifeCycle.HealBy(amount);
        }

        public virtual void Reset()
        {
            if (_lifeCycle != null)
            {
                _lifeCycle.Reset();
            }
        }
        
        public virtual void CleanUp()
        {
        }

        protected abstract void LifeCycleFinished();

        protected virtual void LifeCycleDamageTaken(){}

        protected void OnDisable()
        {
            CleanUp();
        }
    }
}
