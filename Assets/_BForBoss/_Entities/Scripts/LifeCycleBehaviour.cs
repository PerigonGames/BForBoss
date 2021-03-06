using System;
using FMODUnity;
using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Entities
{
    public abstract class LifeCycleBehaviour : MonoBehaviour, IWeaponHolder
    {
        [Resolve][SerializeField] private StudioEventEmitter _onHitAudio = null;
        [Resolve][SerializeField] private StudioEventEmitter _onDeathAudio = null;
        
        [InlineEditor]
        [SerializeField] private HealthScriptableObject _health = null;

        protected LifeCycle _lifeCycle;

        public bool IsAlive => _lifeCycle.IsAlive;

        public virtual void Initialize(Action onDeathCallback)
        {
            _lifeCycle = new LifeCycle(_health);
            _lifeCycle.OnDeath += LifeCycleFinished;
            _lifeCycle.OnDamageTaken += LifeCycleDamageTaken;
            _lifeCycle.NotifyOnDeath(onDeathCallback);
        }

        public void DamagedBy(float amount)
        {
            _lifeCycle.DamageBy(amount);
        }
        
        public void Heal(float amount)
        {
            _lifeCycle.HealBy(amount);
        }

        public virtual void Reset()
        {
            if (_lifeCycle != null)
            {
                _lifeCycle.Reset();
                _lifeCycle.OnDeath += LifeCycleFinished;
                _lifeCycle.OnDamageTaken += LifeCycleDamageTaken;
            }
        }

        protected abstract void LifeCycleFinished();

        protected virtual void LifeCycleDamageTaken()
        {
            if (_lifeCycle.IsAlive)
            {
                _onHitAudio?.Play();
            }
            else
            {
                _onDeathAudio?.Play();
            }
        }

        protected virtual void CleanUp()
        {
            if (_lifeCycle != null)
            {
                _lifeCycle.OnDeath -= LifeCycleFinished;
                _lifeCycle.OnDamageTaken -= LifeCycleDamageTaken;
            }
        }

        protected void OnDisable()
        {
            CleanUp();
        }
    }
}
