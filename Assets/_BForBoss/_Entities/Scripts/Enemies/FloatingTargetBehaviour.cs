using System;
using Perigon.Weapons;
using UnityEngine;

namespace Perigon.Entities
{
    [RequireComponent(typeof(FloatingTargetLifeCycleBehaviour))]
    [RequireComponent(typeof(AgentNavigationBehaviour))]
    [RequireComponent(typeof(EnemyShootingBehaviour))]
    public class FloatingTargetBehaviour : MonoBehaviour
    {
        private LifeCycleBehaviour _lifeCycle = null;
        private AgentNavigationBehaviour _navigation = null;
        private EnemyShootingBehaviour _shootingBehaviour = null;

        private FloatingTargetState _state = FloatingTargetState.MoveTowardsDestination;
        
        public void Initialize(Func<Transform> getPlayerPosition, BulletSpawner bulletSpawner)
        {
            _lifeCycle.Initialize(null);
            _navigation.Initialize(getPlayerPosition, OnDestinationReached);
            _shootingBehaviour.Initialize(getPlayerPosition, bulletSpawner);
        }

        private void Awake()
        {
            _lifeCycle = GetComponent<LifeCycleBehaviour>();
            _navigation = GetComponent<AgentNavigationBehaviour>();
            _shootingBehaviour = GetComponent<EnemyShootingBehaviour>();
        }

        private void Update()
        {
            switch (_state)
            {
                case FloatingTargetState.MoveTowardsDestination:
                    _navigation.MovementUpdate();
                    break;
                case FloatingTargetState.ShootTarget:
                    _shootingBehaviour.ShootingUpdate();
                    break;
            }
        }

        private void OnDestinationReached()
        {
            _state = FloatingTargetState.ShootTarget;
            _shootingBehaviour.Reset();
        }
    }

    public enum FloatingTargetState
    {
        MoveTowardsDestination,
        ShootTarget
    }
}
