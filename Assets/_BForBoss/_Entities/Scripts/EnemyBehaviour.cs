using UnityEngine;
using UnityEngine.Pool;

namespace Perigon.Entities
{
    public abstract class EnemyBehaviour: MonoBehaviour
    {
        public IObjectPool<EnemyBehaviour> Pool { get; set; }
        

        public virtual void Reset()
        {
            ReleaseToPool();
        }

        public void ReleaseToPool()
        {
            Pool.Release(this);
        }
    }
}
