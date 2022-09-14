using System;
using Perigon.Entities;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(AgentNavigationBehaviour))]
    public class FloatingTargetBehaviour : EnemyBehaviour
    {
        [SerializeField] private Transform _shootingFromPosition;
        
        private EnemyLifeCycleBehaviour _lifeCycleBehaviour;
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
        
        public void Initialize(Func<Vector3> getPlayerPosition, BulletSpawner bulletSpawner)
        {
            _animationBehaviour = GetComponent<FloatingEnemyAnimationBehaviour>();
            _animationBehaviour.Initialize();
            _animationBehaviour.SetMovementAnimation();
            var isLineOfSightBlocked =
                new IsLineOfSightBlocked(getPlayerPosition, () => _shootingFromPosition.position);

            _navigationBehaviour = GetComponent<AgentNavigationBehaviour>();
            _navigationBehaviour.Initialize(
                getPlayerPosition,
                isLineOfSightBlocked,
                onDestinationReached: () =>
            {
                _state = FloatingTargetState.ShootTarget;
            });
            
            _lifeCycleBehaviour = GetComponent<EnemyLifeCycleBehaviour>();
            _lifeCycleBehaviour.Initialize(releaseToPool: ReleaseToPool);
            
            _shootingBehaviour = GetComponent<EnemyShootingBehaviour>();
            _shootingBehaviour.Initialize(
                getPlayerPosition,
                bulletSpawner,
                () => _shootingFromPosition.position,
                isLineOfSightBlocked,
                _animationBehaviour,
                onFinishedShooting:() =>
            {
                _state = FloatingTargetState.MoveTowardsDestination;
            });

            _knockbackBehaviour = GetComponent<FloatingEnemyKnockbackBehaviour>();
            _knockbackBehaviour.Initialize(
                _lifeCycleBehaviour.LifeCycle, 
                onKnockbackStarted: () =>
            {
                _navigationBehaviour.PauseNavigation();
                _state = FloatingTargetState.KnockedBack;
            }, 
                onKnockbackFinished:() =>
            {
                _navigationBehaviour.ResumeNavigation();
                _state = FloatingTargetState.MoveTowardsDestination;
            });
        }

        public override void Reset()
        {
            _lifeCycleBehaviour.Reset();
            _knockbackBehaviour.Reset();
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

        private void Awake()
        {
            if (_shootingFromPosition == null)
            {
                PanicHelper.Panic(new Exception("Shooting from transform missing from Floating TargetBehaviour"));
            }
        }
    }
}
