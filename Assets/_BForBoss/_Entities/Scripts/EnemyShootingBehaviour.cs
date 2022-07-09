using System;
using Perigon.Weapons;
using UnityEngine;

namespace Perigon.Entities
{
    public class EnemyShootingBehaviour : MonoBehaviour
    {
        [SerializeField] private float _shootCountDown = 1;
        [SerializeField] private float _aimCountDown = 0.5f;
        [SerializeField] private Transform _shootingFromPosition = null;
        private enum ShootState
        {
            Aim,
            Shoot
        }
        
        private float _elapsedShootCountDown = 0;
        private float _elapsedAimCountDown = 0;
        private Func<Transform> _destination = null;
        private BulletSpawner _bulletSpawner = null;
        
        private ShootState _state = ShootState.Aim;
        
        private Vector3 _shootDirection = Vector3.zero;

        public void Initialize(Func<Transform> getPlayerPosition, BulletSpawner bulletSpawner)
        {
            _destination = getPlayerPosition;
            _bulletSpawner = bulletSpawner;
        }

        public void Reset()
        {
            _elapsedAimCountDown = _aimCountDown;
            _elapsedShootCountDown = _shootCountDown;
        }

        public void ShootingUpdate()
        {
            switch (_state)
            {
                case ShootState.Aim:
                    RotateTowardPlayer();
                    CountDownWhileAiming();
                    break;
                case ShootState.Shoot:
                    CountDownUntilShoot();
                    break;
            }
        }

        private void RotateTowardPlayer()
        {
            _shootDirection = _destination().position - transform.position; 
            transform.LookAt(_destination());
        }

        private void CountDownWhileAiming()
        {
            _elapsedAimCountDown -= Time.deltaTime;
            if (_elapsedAimCountDown <= 0)
            {
                _state = ShootState.Shoot;
                _elapsedAimCountDown = _aimCountDown;
                
            }
        }

        private void CountDownUntilShoot()
        {
            _elapsedShootCountDown -= Time.deltaTime;
            if (_elapsedShootCountDown <= 0)
            {
                Shoot();
                _state = ShootState.Aim;
                _elapsedShootCountDown = _shootCountDown;
            }
        }

        private void Shoot()
        {
            var bullet = _bulletSpawner.SpawnBullet();
            bullet.SetSpawnAndDirection(_shootingFromPosition.position, _shootDirection);
        }
    }
}
