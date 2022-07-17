using System;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class EnemyShootingBehaviour : MonoBehaviour
    {
        [SerializeField] private float _distanceToShootAt = 4;
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _shootCountDown = 1f;
        [SerializeField] private float _aimCountDown = 0.5f;
        [SerializeField] private Transform _shootingFromPosition = null;
        private enum ShootState
        {
            Aim,
            Shoot,
            DistanceCheck
        }
        
        private float _elapsedShootCountDown = 0;
        private float _elapsedAimCountDown = 0;
        private Func<Vector3> _destination = null;
        private BulletSpawner _bulletSpawner = null;
        private Action OnFinishedShooting = null;
        
        private ShootState _state = ShootState.Aim;
        
        private Vector3 _shootDirection = Vector3.zero;

        public void Initialize(Func<Vector3> getPlayerPosition, BulletSpawner bulletSpawner, Action onFinishedShooting)
        {
            _destination = getPlayerPosition;
            _bulletSpawner = bulletSpawner;
            OnFinishedShooting = onFinishedShooting;
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
                case  ShootState.DistanceCheck:
                    DistanceCheck();
                    break;
            }
        }

        private void RotateTowardPlayer()
        {
            _shootDirection = _destination() - _shootingFromPosition.position;
            Quaternion toRotation = Quaternion.LookRotation(_shootDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
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
                _state = ShootState.DistanceCheck;
                _elapsedShootCountDown = _shootCountDown;
            }
        }

        private void DistanceCheck()
        {
            if (Vector3.Distance(transform.position, _destination()) > _distanceToShootAt)
            {
                OnFinishedShooting?.Invoke();
            }
            else
            {
                _state = ShootState.Aim;
            }
        }

        private void Shoot()
        {
            var bullet = _bulletSpawner.SpawnBullet();
            bullet.SetSpawnAndDirection(_shootingFromPosition.position, _shootDirection.normalized);
        }

        private void Awake()
        {
            if (_shootingFromPosition == null)
            {
                PanicHelper.Panic(new Exception("Shooting from transform missing from EnemyShootingBehaviour"));
            }
        }
    }
}
