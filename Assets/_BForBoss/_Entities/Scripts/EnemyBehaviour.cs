using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Perigon.Entities
{
    public abstract class EnemyBehaviour: MonoBehaviour
    {
        public IObjectPool<EnemyBehaviour> Pool { get; set; }

        public event Action<EnemyBehaviour> OnRelease;

        public abstract void Reset();
        public abstract void CleanUp();
        
        protected void ReleaseToPool()
        {
            OnRelease?.Invoke(this);
            Pool.Release(this);
        }
    }
}
