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

        protected virtual void LifeCycleFinished()
        {
            _enemyKillAudio.Play();
        }

        protected virtual void LifeCycleDamageTaken()
        {
            //_enemyHitAudio.Play();
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
