using UnityEngine;
using UnityEngine.Pool;

namespace Perigon.Weapons
{
    public class WallHitVFXSpawner : MonoBehaviour
    {
        [SerializeField] private BaseWallHitVFX _baseWallHitVFXPrefab = null;
        private IObjectPool<BaseWallHitVFX> _pool = null;
        
        public BaseWallHitVFX SpawnWallHitVFX()
        {
            if (_pool == null)
            {
                SetupPools();
            }

            return _pool.Get();
        }

        private void SetupPools()
        {
            _pool = new ObjectPool<BaseWallHitVFX>(
                () =>
                {
                    var vfx = Instantiate(_baseWallHitVFXPrefab);
                    vfx.Initialize(_pool);
                    return vfx;
                },
                (vfx) => vfx.gameObject.SetActive(true),
                (vfx) =>
                {
                    vfx.Reset();
                    vfx.gameObject.SetActive(false);
                });
        }
    }
}
