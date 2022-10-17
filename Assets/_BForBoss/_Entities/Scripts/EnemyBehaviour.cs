using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Perigon.Entities
{
    [RequireComponent(typeof(EnemyLifeCycleBehaviour))]
    public abstract class EnemyBehaviour: MonoBehaviour
    {
        public IObjectPool<EnemyBehaviour> Pool { get; set; }

        public event Action<EnemyBehaviour> OnDeath;

        public abstract void Reset();

        protected void InvokeOnDeathEvent()
        {
            OnDeath?.Invoke(this);
        }
        
        protected void ReleaseToPool()
        {
            Pool.Release(this);
        }
    }
}
