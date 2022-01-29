using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Entities
{
    public abstract class LifeCycleBehaviour : MonoBehaviour
    {
        [InlineEditor]
        [SerializeField] private HealthScriptableObject _health = null;

        protected LifeCycle _lifeCycle;

        public bool IsAlive => _lifeCycle.IsAlive;

        [Button]
        public void Damage(float amount = 5f)
        {
            _lifeCycle.DamageBy(amount);
        }
        
        public void Heal(float amount)
        {
            _lifeCycle.HealBy(amount);
        }

        public virtual void Reset()
        {
            _lifeCycle.Reset();
        }

        protected virtual void Awake()
        {
            _lifeCycle = new LifeCycle(_health);
        }

        protected abstract void LifeCycleFinished();

        protected virtual void OnEnable()
        {
            ((ILifeCycle) _lifeCycle).OnDeath += LifeCycleFinished;
        }

        protected virtual void OnDisable()
        {
            ((ILifeCycle) _lifeCycle).OnDeath -= LifeCycleFinished;
        }
    }
}
