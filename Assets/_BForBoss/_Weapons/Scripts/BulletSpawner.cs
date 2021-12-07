using Perigon.Utility;
using UnityEngine;

namespace Perigon.Weapons
{

    public enum BulletTypes
    {
        NoPhysics,
        Physics
    }
    
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private BulletBehaviour[] _bulletPrefabs;
        private ObjectPooler<BulletBehaviour>[] _pools = null;

        public IBullet SpawnBullet(BulletTypes typeOfBullet = BulletTypes.NoPhysics)
        {
            if(_pools == null) 
                SetupPools();
            return _pools[(int)typeOfBullet].Get();
        }

        private void SetupPools()
        {
            _pools = new ObjectPooler<BulletBehaviour>[_bulletPrefabs.Length];
            for (int i = 0; i < _bulletPrefabs.Length; i++)
            {
                var prefab = _bulletPrefabs[i];
                var index = i;
                _pools[i] = new ObjectPooler<BulletBehaviour>($"{prefab.name}Pool", () => GenerateBullet(prefab,index),
                    (bullet) => bullet.gameObject.SetActive(true),
                    (bullet) => bullet.gameObject.SetActive(false));
            }
        }

        private BulletBehaviour GenerateBullet(BulletBehaviour prefab, int poolIndex)
        {
            var newBullet = Instantiate(prefab);
            newBullet.Pool = _pools[poolIndex];
            return newBullet;
        }
    }
}
