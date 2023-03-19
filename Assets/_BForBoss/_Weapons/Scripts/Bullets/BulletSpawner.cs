using System;
using System.Collections.Generic;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Weapons
{
    [RequireComponent(typeof(WallHitVFXSpawner))]
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private BulletBehaviour[] _bulletPrefabs;
        //private ObjectPooler<BulletBehaviour>[] _pools = null;

        private ObjectPooler<BulletBehaviour>[] _playerBulletPools = null;
        private ObjectPooler<BulletBehaviour>[] _enemyBulletPools = null;

        [SerializeField, Layer] private int _layer = 8; //default is Bullet layer

        //[SerializeField] private LayerMask _layerMask = -1;
        [SerializeField] private LayerMask _playerLayerMask = -1;
        [SerializeField] private LayerMask _enemyLayerMask = -1;

        private List<BulletBehaviour> _listOfActiveBullets = new List<BulletBehaviour>();
        private WallHitVFXSpawner _wallHitVFXSpawner;
        
        public IBullet SpawnBullet(BulletTypes typeOfBullet = BulletTypes.NoPhysics, BulletOwner bulletOwner = BulletOwner.Player)
        {
            // if(_pools == null) 
            //     SetupPools();
            // var bullet = _pools[(int)typeOfBullet].Get();

            if (_playerBulletPools == null || _enemyBulletPools == null)
            {
                SetupPools();
            }
            ObjectPooler<BulletBehaviour>[] pools = bulletOwner == BulletOwner.Player ? _playerBulletPools : _enemyBulletPools;
            var bullet = pools[(int)typeOfBullet].Get();
            
            
            _listOfActiveBullets.Add(bullet);
            return bullet;
        }
        
        public void Reset()
        {
            if (_playerBulletPools != null)
            {
                foreach (var pool in _playerBulletPools)
                {
                    pool.Clear();
                }
            }
            
            if (_enemyBulletPools != null)
            {
                foreach (var pool in _enemyBulletPools)
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
            // _pools = new ObjectPooler<BulletBehaviour>[_bulletPrefabs.Length];
            // for (int i = 0; i < _bulletPrefabs.Length; i++)
            // {
            //     var prefab = _bulletPrefabs[i];
            //     var index = i;
            //     _pools[i] = new ObjectPooler<BulletBehaviour>(
            //         () => GenerateBullet(prefab,index),
            //         (bullet) => bullet.gameObject.SetActive(true),
            //         OnRelease);
            // }

            _playerBulletPools = SetupPool(BulletOwner.Player);
            _enemyBulletPools = SetupPool(BulletOwner.Enemy);
        }

        private ObjectPooler<BulletBehaviour>[] SetupPool(BulletOwner bulletOwner)
        {
            ObjectPooler<BulletBehaviour>[] pools = new ObjectPooler<BulletBehaviour>[_bulletPrefabs.Length];
            for (int i = 0; i < _bulletPrefabs.Length; i++)
            {
                var prefab = _bulletPrefabs[i];
                var index = i;
                pools[i] = new ObjectPooler<BulletBehaviour>(
                    () => GenerateBullet(prefab, index, bulletOwner),
                    (bullet) => bullet.gameObject.SetActive(true),
                    OnRelease);
            }

            return pools;
        }

        private void Awake()
        {
            _wallHitVFXSpawner = GetComponent<WallHitVFXSpawner>();
        }

        private void OnRelease(BulletBehaviour bullet)
        {
            var wallHit = _wallHitVFXSpawner.SpawnWallHitVFX();
            wallHit.transform.position = bullet.transform.position;
            wallHit.Spawn();
            _listOfActiveBullets.Remove(bullet);
            bullet.gameObject.SetActive(false);
        }

        private BulletBehaviour GenerateBullet(BulletBehaviour prefab, int poolIndex, BulletOwner bulletOwner)
        {
            var newBullet = Instantiate(prefab);
            _listOfActiveBullets.Add(newBullet);
            newBullet.gameObject.layer = _layer;

            switch (bulletOwner)
            {
                case BulletOwner.Player:
                    newBullet.Pool = _playerBulletPools[poolIndex];
                    newBullet.Mask = _playerLayerMask;
                    break;
                case BulletOwner.Enemy:
                    newBullet.Pool = _enemyBulletPools[poolIndex];
                    newBullet.Mask = _enemyLayerMask;
                    break;
                default:
                    PanicHelper.Panic(new Exception("Unrecognized Bullet Owner"));
                    break;
            }
            
            return newBullet;
        }
    }
}
