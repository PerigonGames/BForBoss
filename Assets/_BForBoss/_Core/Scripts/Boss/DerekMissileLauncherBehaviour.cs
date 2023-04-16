using System;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class DerekMissileLauncherBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform _lowerLauncher;
        [SerializeField] private Transform _middleLauncher;
        [SerializeField] private Transform _upperLauncher;
        private BulletSpawner _bulletSpawner;
        private IGetPlayerTransform _playerTransform;

        public void Initialize(IGetPlayerTransform playerTransform)
        {
            _playerTransform = playerTransform;
        }
        
        public void ShootLowerLauncher()
        {
            var bullet = _bulletSpawner.SpawnBullet(BulletTypes.NoPhysics);
            var direction = _playerTransform.Value.position - transform.position;
            bullet.SetSpawnAndDirection(_lowerLauncher.transform.position + Vector3.up * 3, direction.normalized);
        }

        private void Awake()
        {
            _bulletSpawner = GetComponent<BulletSpawner>();
        }
    }
}
