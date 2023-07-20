using System;
using Perigon.Utility;
using Perigon.Weapons;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class DerekMissileLauncherBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform[] _launcherLocations;
        private BulletSpawner _bulletSpawner;
        private IGetPlayerTransform _playerTransform;

        private bool _canShootMissiles = false;
        private float _intervalBetweenShots;
        private float _shootTimer;

        public void Reset()
        {
            _canShootMissiles = false;
        }
        
        public void Initialize(IGetPlayerTransform playerTransform)
        {
            _playerTransform = playerTransform;
        }
        
        public void ShootMissile()
        {
            var bullet = _bulletSpawner.SpawnBullet(BulletTypes.NoPhysics);
            bullet.SetSpawnAndDirection(GetLaunchPosition(),Vector3.up);
            bullet.HomingTarget = _playerTransform.Value;
        }

        public void StartShooting(float intervalBetweenShots)
        {
            if (intervalBetweenShots <= 0.0f)
            {
                Perigon.Utility.Logger.LogWarning("Trying to shoot missiles with non-existent interval times, disregarding", LoggerColor.Default, "derekboss");
                return;
            }

            _intervalBetweenShots = intervalBetweenShots;
            _shootTimer = _intervalBetweenShots;
            _canShootMissiles = true;
        }

        public void StopShooting()
        {
            _canShootMissiles = false;
        }
        
        private Vector3 GetLaunchPosition()
        {
            var minDistance = float.MaxValue;
            var closestLauncher = _launcherLocations[0];
            foreach (var launcher in _launcherLocations)
            {
                var distance = Vector3.Distance(_playerTransform.Value.position, launcher.position);
                if (Vector3.Distance(_playerTransform.Value.position, launcher.position) < minDistance)
                {
                    closestLauncher = launcher;
                    minDistance = distance;
                }
            }

            return closestLauncher.position;
        }

        private void Awake()
        {
            _bulletSpawner = GetComponent<BulletSpawner>();
            this.PanicIfNullOrEmptyList(_launcherLocations, nameof(_launcherLocations));
        }

        private void Update()
        {
            if (!_canShootMissiles)
            {
                return;
            }
            
            if (_shootTimer > 0.0f)
            {
                _shootTimer -= Time.deltaTime;
                return;
            }
            
            ShootMissile();
            _shootTimer = _intervalBetweenShots;
        }

        private void Update()
        {
            if (!_canShootMissiles)
            {
                return;
            }
            
            if (_shootTimer > 0.0f)
            {
                _shootTimer -= Time.deltaTime;
                return;
            }
            
            ShootMissile();
            _shootTimer = _intervalBetweenShots;
        }
    }
}
