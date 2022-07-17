using System;
using Perigon.Entities;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class FloatingTargetBehaviour : EnemyBehaviour
    {
        [SerializeField] private HealthbarViewBehaviour _healthbar;
        private AgentNavigationBehaviour _navigationBehaviour = null;
        private EnemyShootingBehaviour _shootingBehaviour = null;
        private FloatingTargetState _state = FloatingTargetState.MoveTowardsDestination;
        
        public void Initialize(Func<Vector3> getPlayerPosition, BulletSpawner bulletSpawner, Action onDeathCallback, Action<EnemyBehaviour> onReleaseToSpawner = null)
        {
            base.Initialize(getPlayerPosition, onDeathCallback, onReleaseToSpawner);
            
            _navigationBehaviour = GetComponent<AgentNavigationBehaviour>();
            _navigationBehaviour.Initialize(getPlayerPosition, OnDestinationReached);
            
            if (_healthbar != null)
            {
                _healthbar.Initialize(_lifeCycle);
            }
            _shootingBehaviour.Initialize(getPlayerPosition, bulletSpawner);
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
        }

        private void OnDestinationReached()
        {
            _state = FloatingTargetState.ShootTarget;
            _shootingBehaviour.Reset();
        }
        
        private void Awake()
        {
            if (_healthbar == null)
            {                
                Debug.LogWarning("A FloatingTargetBehaviour is missing a health bar");
            }
        }
        
        private void Update()
        {
            switch (_state)
            {
                case FloatingTargetState.MoveTowardsDestination:
                    _navigationBehaviour.MovementUpdate();
                    break;
                case FloatingTargetState.ShootTarget:
                    _shootingBehaviour.ShootingUpdate();
                    break;
            }
        }
        
        public enum FloatingTargetState
        {
            MoveTowardsDestination,
            ShootTarget
        }
    }
}
