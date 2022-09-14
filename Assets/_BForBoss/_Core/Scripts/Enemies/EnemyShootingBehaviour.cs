using System;
using Perigon.Utility;
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

        [SerializeField] private Transform _shootingFromPosition = null;

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
        private Action OnFinishedShooting = null;
        
        private ShootState _state = ShootState.Aim;
        
        private Vector3 _shootDirection = Vector3.zero;
        
        private readonly int _vfxFireEvent = Shader.PropertyToID("OnFire");
        private readonly int _vfxChargeTime = Shader.PropertyToID("Charge Time");

        public void Initialize(Func<Vector3> getPlayerPosition, 
            BulletSpawner bulletSpawner,
            IFloatingEnemyAnimation enemyAnimation,
            Action onFinishedShooting
            )
        {
            _destination = getPlayerPosition;
            _bulletSpawner = bulletSpawner;
            _enemyAnimation = enemyAnimation;
            OnFinishedShooting = onFinishedShooting;
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
            _shootDirection = _destination() - _shootingFromPosition.position;
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
            if (IsDestinationTooFar() || IsLineOfSightBlocked())
            {
                _enemyAnimation.SetMovementAnimation();
                Reset();
                OnFinishedShooting?.Invoke();
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

        private bool IsLineOfSightBlocked()
        {
            var direction = _destination() - _shootingFromPosition.position;
            if (Physics.Raycast(_shootingFromPosition.position, direction.normalized, out var hitInfo))
            {
                Debug.DrawRay(_shootingFromPosition.position, direction.normalized, Color.red);
                if (hitInfo.collider.CompareTag(TagsAndLayers.Tags.Player))
                {
                    return false;
                }
            }

            return true;
        }

        private void Shoot()
        {
            _muzzleFlashVFX.SendEvent(_vfxFireEvent);
            _enemyAnimation.SetShootingAnimation();
            var bullet = _bulletSpawner.SpawnBullet();
            bullet.SetSpawnAndDirection(_shootingFromPosition.position, _shootDirection.normalized);
        }

        private void Awake()
        {
            if (_shootingFromPosition == null)
            {
                PanicHelper.Panic(new Exception("Shooting from transform missing from EnemyShootingBehaviour"));
            }

            var vfxDuration = Mathf.Max(_shootCountDownInSeconds - _vfxShootDelay, 0);
            _muzzleFlashVFX.SetFloat(_vfxChargeTime, vfxDuration);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawWireSphere(transform.position, _distanceToShootAt);
        }
    }
}
