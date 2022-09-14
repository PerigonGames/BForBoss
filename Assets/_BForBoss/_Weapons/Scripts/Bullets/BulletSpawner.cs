using System.Collections.Generic;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Weapons
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private BulletBehaviour[] _bulletPrefabs;
        private ObjectPooler<BulletBehaviour>[] _pools = null;

        [SerializeField, Layer] private int _layer = 8; //default is Bullet layer

        [SerializeField] private LayerMask _layerMask = -1;

        private List<BulletBehaviour> _listOfActiveBullets = new List<BulletBehaviour>();

        public IBullet SpawnBullet(BulletTypes typeOfBullet = BulletTypes.NoPhysics)
        {
            if(_pools == null) 
                SetupPools();
            var bullet = _pools[(int)typeOfBullet].Get();
            _listOfActiveBullets.Add(bullet);
            return bullet;
        }


        public void Reset()
        {
            if (_pools != null)
            {
                foreach (var pool in _pools)
                {
                    pool.Clear();
                }
            }
            
            foreach (var bullet in _listOfActiveBullets)
            {
                Destroy(bullet.gameObject);
            }
            _listOfActiveBullets.Clear();
        }

        private void SetupPools()
        {
            _pools = new ObjectPooler<BulletBehaviour>[_bulletPrefabs.Length];
            for (int i = 0; i < _bulletPrefabs.Length; i++)
            {
                var prefab = _bulletPrefabs[i];
                var index = i;
                _pools[i] = new ObjectPooler<BulletBehaviour>(() => GenerateBullet(prefab,index),
                    (bullet) => bullet.gameObject.SetActive(true),
                    OnRelease);
            }
        }

        private void OnRelease(BulletBehaviour bullet)
        {
            _listOfActiveBullets.Remove(bullet);
            bullet.gameObject.SetActive(false);
        }

        private BulletBehaviour GenerateBullet(BulletBehaviour prefab, int poolIndex)
        {
            var newBullet = Instantiate(prefab);
            _listOfActiveBullets.Add(newBullet);
            newBullet.gameObject.layer = _layer;
            newBullet.Pool = _pools[poolIndex];
            newBullet.Mask = _layerMask;
            return newBullet;
        }
    }
}
