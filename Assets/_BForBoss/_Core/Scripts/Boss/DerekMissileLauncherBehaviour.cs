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

        public void Initialize(IGetPlayerTransform playerTransform)
        {
            _playerTransform = playerTransform;
        }
        
        public void ShootMissile()
        {
            var bullet = _bulletSpawner.SpawnBullet(BulletTypes.NoPhysics);
            //TODO - need to refactor bullet spawner, or create new spawner
            (bullet as DerekMissileBehaviour)?.Initialize(_playerTransform);
            var direction = _playerTransform.Value.position - transform.position;
            bullet.SetSpawnAndDirection(GetLaunchPosition() + Vector3.up * 3, direction.normalized);
        }

        private void Awake()
        {
            _bulletSpawner = GetComponent<BulletSpawner>();
            if (_launcherLocations.IsNullOrEmpty())
            {
                PanicHelper.Panic(new Exception("LauncherLocations missing from DerekMissileLauncher"));
            }
        }
    }
}
