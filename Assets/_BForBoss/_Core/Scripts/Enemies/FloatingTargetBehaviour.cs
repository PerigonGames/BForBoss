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
        private FloatingEnemyKnockbackBehaviour _knockbackBehaviour;
        private AgentNavigationBehaviour _navigationBehaviour;
        private EnemyShootingBehaviour _shootingBehaviour;
        private FloatingEnemyAnimationBehaviour _animationBehaviour;
        private FloatingEnemyVisualEffectsBehaviour _visualEffectsBehaviour;
        private EnemyAudioController _audioController;
        private FloatingTargetState _state = FloatingTargetState.Spawning;

        private enum FloatingTargetState
        {
            Spawning,
            MoveTowardsDestination,
            ShootTarget,
            KnockedBack,
            Death
        }
        
        public void Initialize(Func<Vector3> getPlayerPosition, BulletSpawner bulletSpawner)
        {
            _animationBehaviour = GetComponent<FloatingEnemyAnimationBehaviour>();
            _animationBehaviour.Initialize();
            _animationBehaviour.SetMovementAnimation();

            _visualEffectsBehaviour = GetComponent<FloatingEnemyVisualEffectsBehaviour>();
            _visualEffectsBehaviour.Initialize(onSpawnVisualsComplete: () =>
            {
                _state = FloatingTargetState.MoveTowardsDestination;
            }, onDeathVisualsComplete: ReleaseToPool);

            _navigationBehaviour = GetComponent<AgentNavigationBehaviour>();
            _navigationBehaviour.Initialize(
                getPlayerPosition,
                () => _shootingFromPosition.position,
                onDestinationReached: () =>
            {
                _state = FloatingTargetState.ShootTarget;
            });
            
            _lifeCycleBehaviour = GetComponent<EnemyLifeCycleBehaviour>();
            _lifeCycleBehaviour.Initialize(onDeath: () =>
            {
                _visualEffectsBehaviour.StartDeathVisual();
                _navigationBehaviour.PauseNavigation();
                _state = FloatingTargetState.Death;
                InvokeOnDeathEvent();
            });
            
            _audioController = GetComponent<EnemyAudioController>();
            _audioController.Initialize(_lifeCycleBehaviour.LifeCycle);
            
            _shootingBehaviour = GetComponent<EnemyShootingBehaviour>();
            _shootingBehaviour.Initialize(
                getPlayerPosition,
                bulletSpawner,
                () => _shootingFromPosition.position,
                _animationBehaviour,
                _audioController,
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
            }, onKnockbackFinished:() =>
            {
                _navigationBehaviour.ResumeNavigation();
                _state = FloatingTargetState.MoveTowardsDestination;
            });
        }

        public override void Reset()
        {            
            _state = FloatingTargetState.Spawning;
            _lifeCycleBehaviour.Reset();
            _knockbackBehaviour.Reset();
            _visualEffectsBehaviour.Reset();
        }

        private void FixedUpdate()
        {
            switch (_state)
            {
                case FloatingTargetState.Spawning:
                    _visualEffectsBehaviour.OnSpawningFixedUpdate();
                    break;
                case FloatingTargetState.MoveTowardsDestination:
                    _navigationBehaviour.OnMovementFixedUpdate();
                    break;
                case FloatingTargetState.ShootTarget:
                    _shootingBehaviour.ShootingFixedUpdate();
                    break;
                case FloatingTargetState.KnockedBack:
                    _knockbackBehaviour.UpdateKnockback(Time.fixedDeltaTime);
                    break;
                case FloatingTargetState.Death:
                    _visualEffectsBehaviour.OnDeathFixedUpdate();
                    break;
            }
        }

        private void Awake()
        {
            this.PanicIfNullObject(_shootingFromPosition, nameof(_shootingFromPosition));
        }
    }
}
