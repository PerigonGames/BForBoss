using System;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace BForBoss
{
    public class EnemyShootingBehaviour : MonoBehaviour
    {
        [InfoBox("Player must be within this distance for AI to shoot, when outside, AI will move closer")]
        [SerializeField] private float _distanceToShootAt = 4;
        [InfoBox("Time taken in seconds for AI to aim at players direction before shooting")]
        [SerializeField] private float _aimCountDownInSeconds = 0.5f;
        [InfoBox("Rotation Lerp Speed - Speed at which AI rotates and aims at player")]
        [SerializeField] private float _rotationSpeed = 15f;
        [InfoBox("Time taken in seconds for AI to shoot after taking aim")]
        [SerializeField] private float _shootCountDownInSeconds = 1f;

        [InfoBox("Delay between shot readying effect and shot firing effect")]
        [SerializeField] private float _vfxShootDelay = 0.1f;

        [SerializeField] private VisualEffect _muzzleFlashVFX = null;
        
        private enum ShootState
        {
            Aim,
            Shoot,
            Evaluate
        }
        
        private float _elapsedShootCountDown = 0;
        private float _elapsedAimCountDown = 0;
        private Func<Vector3> _destination = null;
        private BulletSpawner _bulletSpawner = null;
        private IFloatingEnemyAnimation _enemyAnimation = null;
        private Action _onFinishedShooting = null;
        
        private ShootState _state = ShootState.Aim;
        
        private Vector3 _shootDirection = Vector3.zero;
        private Func<Vector3> _shootingFromPosition;
        private LineOfSight _lineOfSight;
        
        private readonly int _vfxFireEvent = Shader.PropertyToID("OnFire");
        private readonly int _vfxChargeTime = Shader.PropertyToID("Charge Time");

        public float DistanceToShootAt => _distanceToShootAt;

        public void Initialize(Func<Vector3> getPlayerPosition, 
            BulletSpawner bulletSpawner,
            Func<Vector3> shootingFromPosition,
            LineOfSight lineOfSight,
            IFloatingEnemyAnimation enemyAnimation,
            Action onFinishedShooting
            )
        {
            _destination = getPlayerPosition;
            _bulletSpawner = bulletSpawner;
            _shootingFromPosition = shootingFromPosition;
            _lineOfSight = lineOfSight;
            _enemyAnimation = enemyAnimation;
            _onFinishedShooting = onFinishedShooting;
        }

        private void Reset()
        {
            _muzzleFlashVFX.Stop();
            _elapsedAimCountDown = _aimCountDownInSeconds;
            _elapsedShootCountDown = _shootCountDownInSeconds;
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
                case ShootState.Evaluate:
                    Evaluate();
                    break;
            }
        }

        private void RotateTowardPlayer()
        {
            _shootDirection = _destination() - _shootingFromPosition();
            Quaternion toRotation = Quaternion.LookRotation(_shootDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, _rotationSpeed * Time.fixedDeltaTime);
        }

        private void CountDownWhileAiming()
        {
            _enemyAnimation.SetIdleAnimation();
            _elapsedAimCountDown -= Time.deltaTime;
            if (_elapsedAimCountDown <= 0)
            {
                _state = ShootState.Shoot;
                _muzzleFlashVFX.Play();
                _elapsedAimCountDown = _aimCountDownInSeconds;
            }
        }

        private void CountDownUntilShoot()
        {
            _elapsedShootCountDown -= Time.deltaTime;
            if (_elapsedShootCountDown <= 0)
            {
                Shoot();
                _state = ShootState.Evaluate;
                _elapsedShootCountDown = _shootCountDownInSeconds;
            }
        }

        private void Evaluate()
        {
            if (IsDestinationTooFar() || _lineOfSight.IsBlocked())
            {
                _enemyAnimation.SetMovementAnimation();
                Reset();
                _onFinishedShooting?.Invoke();
            }
            else
            {
                _state = ShootState.Aim;
            }
        }

        private bool IsDestinationTooFar()
        {
            return Vector3.Distance(transform.position, _destination()) > _distanceToShootAt;
        }

        private void Shoot()
        {
            _muzzleFlashVFX.SendEvent(_vfxFireEvent);
            _enemyAnimation.SetShootingAnimation();
            var bullet = _bulletSpawner.SpawnBullet();
            bullet.SetSpawnAndDirection(_shootingFromPosition(), _shootDirection.normalized);
        }

        private void Awake()
        {
            var vfxDuration = Mathf.Max(_shootCountDownInSeconds - _vfxShootDelay, 0);
            _muzzleFlashVFX.SetFloat(_vfxChargeTime, vfxDuration);
        }
    }
}
