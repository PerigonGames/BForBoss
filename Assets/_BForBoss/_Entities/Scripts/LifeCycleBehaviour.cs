using FMODUnity;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Entities
{
    public abstract class LifeCycleBehaviour : MonoBehaviour
    {
        [Resolve][SerializeField] private StudioEventEmitter _enemyHitAudio = null;
        [Resolve][SerializeField] private StudioEventEmitter _enemyKillAudio = null;
        
        [InlineEditor]
        [SerializeField] private HealthScriptableObject _health = null;

        protected LifeCycle _lifeCycle;

        public bool IsAlive => _lifeCycle.IsAlive;

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

        protected virtual void LifeCycleDamageTaken()
        {
            if (_lifeCycle.IsAlive)
            {
                _enemyHitAudio.Play();
            }
            else
            {
                _enemyKillAudio.Play();
            }
        }

        protected virtual void OnEnable()
        {
            _lifeCycle.OnDeath += LifeCycleFinished;
            _lifeCycle.OnDamageTaken += LifeCycleDamageTaken;
        }

        protected virtual void OnDisable()
        {
            _lifeCycle.OnDeath -= LifeCycleFinished;
            _lifeCycle.OnDamageTaken -= LifeCycleDamageTaken;
        }
    }
}
