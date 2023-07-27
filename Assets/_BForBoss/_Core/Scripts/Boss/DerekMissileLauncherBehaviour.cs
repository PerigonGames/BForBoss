using System;
using Perigon.Utility;
using Perigon.VFX;
using Perigon.Weapons;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class DerekMissileLauncherBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform[] _launcherLocations;
        private TimedVFXEffect[] _launcherVFX;
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

            _launcherVFX = new TimedVFXEffect[_launcherLocations.Length];
            for (int i = 0; i < _launcherLocations.Length; i++)
            {
                _launcherVFX[i] = _launcherLocations[i].gameObject.GetComponentInChildren<TimedVFXEffect>();
            }
        }
        
        public void ShootMissile()
        {
            var bullet = _bulletSpawner.SpawnBullet(BulletTypes.NoPhysics);
            var closestLauncherIndex = GetClosestLaunchTransformIndex(); 
            bullet.SetSpawnAndDirection(_launcherLocations[closestLauncherIndex].position,Vector3.up);
            if (_launcherVFX[closestLauncherIndex] != null)
            {
                _launcherVFX[closestLauncherIndex].StartEffect();
            }
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
        
        private int GetClosestLaunchTransformIndex()
        {
            var minDistance = float.MaxValue;
            var launcherIndex = 0;
            for(int i = 0; i < _launcherLocations.Length; i++)
            {
                var launcher = _launcherLocations[i];
                var distance = Vector3.Distance(_playerTransform.Value.position, launcher.position);
                if (Vector3.Distance(_playerTransform.Value.position, launcher.position) < minDistance)
                {
                    launcherIndex = i;
                    minDistance = distance;
                }
            }

            return launcherIndex;
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
    }
}
