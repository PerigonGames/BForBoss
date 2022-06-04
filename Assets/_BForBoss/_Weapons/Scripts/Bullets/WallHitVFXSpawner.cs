using Perigon.Utility;
using UnityEngine;

namespace Perigon.Weapons
{
    public class WallHitVFXSpawner : MonoBehaviour
    {
        [SerializeField] private WallHitVFX _wallHitVFXPrefab = null;
        private ObjectPooler<WallHitVFX> _pools = null;
        
        public WallHitVFX SpawnWallHitVFX()
        {
            if (_pools == null)
            {
                SetupPools();
            }

            return _pools.Get();
        }

        private void SetupPools()
        {
            _pools = new ObjectPooler<WallHitVFX>(
                () =>
                {
                    var vfx = Instantiate(_wallHitVFXPrefab);
                    vfx.Initialize(() =>
                    {
                        _pools.Reclaim(vfx);
                    });
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
