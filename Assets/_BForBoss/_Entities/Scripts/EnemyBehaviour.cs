using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Perigon.Entities
{
    [RequireComponent(typeof(EnemyLifeCycleBehaviour))]
    public abstract class EnemyBehaviour: MonoBehaviour
    {
        public IObjectPool<EnemyBehaviour> Pool { get; set; }

        public event Action<EnemyBehaviour> OnRelease;

        public abstract void Reset();
        
        protected void ReleaseToPool()
        {
            OnRelease?.Invoke(this);
            Pool.Release(this);
        }
    }
}
