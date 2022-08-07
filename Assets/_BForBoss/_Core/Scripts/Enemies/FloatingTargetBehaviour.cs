using System;
using Perigon.Entities;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(AgentNavigationBehaviour))]
    public class FloatingTargetBehaviour : EnemyBehaviour, IKnockback
    {
        [SerializeField] private HealthbarViewBehaviour _healthbar;

        [Header("Knockback Settings")]
        [SerializeField] private float _knockbackRadiusModifier = 10f;
        [SerializeField] private float _knockbackUpwardsForceModifier = 10f;
        [SerializeField] private float _knockbackMaxDuration = 2f;
        
        private AgentNavigationBehaviour _navigationBehaviour = null;
        private EnemyShootingBehaviour _shootingBehaviour = null;
        private FloatingEnemyAnimationBehaviour _animationBehaviour = null;
        private FloatingTargetState _state = FloatingTargetState.MoveTowardsDestination;
        
        private Rigidbody _rb;
        private float _knockbackCurrentTime;

        private enum FloatingTargetState
        {
            MoveTowardsDestination,
            ShootTarget,
            KnockedBack
        }
        
        public void Initialize(Func<Vector3> getPlayerPosition, BulletSpawner bulletSpawner, Action onDeathCallback, Action<EnemyBehaviour> onReleaseToSpawner = null)
        {
            base.Initialize(getPlayerPosition, onDeathCallback, onReleaseToSpawner);

            _animationBehaviour = GetComponent<FloatingEnemyAnimationBehaviour>();
            _animationBehaviour.Initialize();
            _animationBehaviour.SetMovementAnimation();
            
            _navigationBehaviour = GetComponent<AgentNavigationBehaviour>();
            _navigationBehaviour.Initialize(getPlayerPosition, () =>
            {
                _state = FloatingTargetState.ShootTarget;
            });
            
            if (_healthbar != null)
            {
                _healthbar.Initialize(_lifeCycle);
            }

            _shootingBehaviour = GetComponent<EnemyShootingBehaviour>();
            _shootingBehaviour.Initialize(getPlayerPosition, bulletSpawner, _animationBehaviour,() =>
            {
                _state = FloatingTargetState.MoveTowardsDestination;
            });
        }
        
        public void ApplyKnockback(float force, Vector3 originPosition)
        {
            if (_rb == null || !_lifeCycle.IsAlive)
                return;
            ToggleKnockbackState(true);
            _rb.AddExplosionForce(force, originPosition, _knockbackRadiusModifier, _knockbackUpwardsForceModifier);
        }

        public override void Reset()
        {
            base.Reset();
            gameObject.SetActive(true);
            _healthbar.Reset();
        }
        
        protected override void LifeCycleFinished()
        {
            if (_onReleaseToSpawner != null)
            {
                _onReleaseToSpawner.Invoke(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
            ToggleKnockbackState(false);
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb != null)
            {
                _rb.isKinematic = true;
                _rb.useGravity = false;
            }
            if (_healthbar == null)
            {                
                Debug.LogWarning("A FloatingTargetBehaviour is missing a health bar");
            }
        }

        private void CheckKnockback(float time)
        {
            if (_rb == null) 
                return;
            _knockbackCurrentTime += time;
            if (_knockbackCurrentTime > _knockbackMaxDuration || _rb.velocity.sqrMagnitude < float.Epsilon * float.Epsilon)
            {
                ToggleKnockbackState(false);
            }
        }

        private void ToggleKnockbackState(bool active)
        {
            if (_rb == null)
                return;
            if (active)
            {
                _knockbackCurrentTime = 0f;
                _navigationBehaviour.PauseNavigation();
            }
            else
                _navigationBehaviour.ResumeNavigation();
            _rb.isKinematic = !active;
            _rb.useGravity = active;
            _state = active ? FloatingTargetState.KnockedBack : FloatingTargetState.MoveTowardsDestination;
        }
        
        private void FixedUpdate()
        {
            switch (_state)
            {
                case FloatingTargetState.MoveTowardsDestination:
                    _navigationBehaviour.MovementUpdate();
                    break;
                case FloatingTargetState.ShootTarget:
                    _shootingBehaviour.ShootingUpdate();
                    break;
                case FloatingTargetState.KnockedBack:
                    CheckKnockback(Time.fixedDeltaTime);
                    break;
            }
        }
    }
}
