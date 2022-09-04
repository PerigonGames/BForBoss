using UnityEngine;
using UnityEngine.Pool;

namespace Perigon.Entities
{
    public abstract class EnemyBehaviour: MonoBehaviour
    {
        public IObjectPool<EnemyBehaviour> Pool { get; set; }

        public abstract void Reset();
        
        protected void ReleaseToPool()
        {
            Pool.Release(this);
        }
    }
}
