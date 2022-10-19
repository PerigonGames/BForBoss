using UnityEngine;
using UnityEngine.Pool;

namespace Perigon.Weapons
{
    public abstract class BaseWallHitVFX : MonoBehaviour
    {
        private IObjectPool<BaseWallHitVFX> _objectPool;

        public void Initialize(IObjectPool<BaseWallHitVFX> pool)
        {
            _objectPool = pool;
        }

        public abstract void Spawn();
        public abstract void Reset();
        
        protected void ReleaseToPool()
        {
            _objectPool.Release(this);
        }
    }
}
