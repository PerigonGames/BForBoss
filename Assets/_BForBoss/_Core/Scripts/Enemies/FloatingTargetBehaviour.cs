using System;
using Perigon.Entities;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(AgentNavigationBehaviour))]
    public class FloatingTargetBehaviour : EnemyBehaviour
    {
        [SerializeField] private HealthbarViewBehaviour _healthbar;

        private FloatingEnemyKnockbackBehaviour _knockbackBehaviour = null;
        private AgentNavigationBehaviour _navigationBehaviour = null;
        private EnemyShootingBehaviour _shootingBehaviour = null;
        private FloatingEnemyAnimationBehaviour _animationBehaviour = null;
        private FloatingTargetState _state = FloatingTargetState.MoveTowardsDestination;

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

            _knockbackBehaviour = GetComponent<FloatingEnemyKnockbackBehaviour>();
            _knockbackBehaviour.Initialize(_lifeCycle, () =>
            {
                _navigationBehaviour.PauseNavigation();
                _state = FloatingTargetState.KnockedBack;
            }, () =>
            {
                _navigationBehaviour.ResumeNavigation();
                _state = FloatingTargetState.MoveTowardsDestination;
            });
        }

        public override void Reset()
        {
            base.Reset();
            gameObject.SetActive(true);
            _healthbar.Reset();
            _knockbackBehaviour.Reset();
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
            _knockbackBehaviour.Reset();
        }

        private void Awake()
        {
            if (_healthbar == null)
            {                
                Debug.LogWarning("A FloatingTargetBehaviour is missing a health bar");
            }
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
                    _knockbackBehaviour.UpdateKnockback(Time.fixedDeltaTime);
                    break;
            }
        }
    }
}
