using System;
using Perigon.Entities;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(PlayerLifeCycleBehaviour))]
    [RequireComponent(typeof(PlayerMovementBehaviour))]
    public class PlayerBehaviour : MonoBehaviour
    {
        private PlayerMovementBehaviour _playerMovement;
        private PlayerLifeCycleBehaviour _playerLifeCycleBehaviour;
        private PlayerSlowMotionBehaviour _playerSlowMotion;
        private PGInputSystem _inputSystem;

        public PlayerMovementBehaviour PlayerMovement => _playerMovement;

        public void Initialize(PGInputSystem inputSystem, LifeCycle playerLifeCycle, IEnergySystem energySystem)
        {
            _playerMovement.Initialize(energySystem, inputSystem);
            _playerSlowMotion.Initialize(energySystem);
            _playerLifeCycleBehaviour.Initialize(
                playerLifeCycle,
                onEndGameCallback: () =>
            {
                StateManager.Instance.SetState(State.EndGame);
            }, 
                onDeathCallback: () =>
            {
                StateManager.Instance.SetState(State.Death);
            });
            

            _inputSystem = inputSystem;
            _inputSystem.OnSlowTimeAction += _playerSlowMotion.OnSlowMotion;
            StateManager.Instance.OnStateChanged += HandleOnStateChanged;
        }

        public void Reset()
        {
            _playerLifeCycleBehaviour.Reset();
        }

        public void SpawnAt(Vector3 position, Quaternion facing)
        {
            _playerMovement.SetVelocity(Vector3.zero);
            _playerMovement.SetPosition(position);
            _playerMovement.rootPivot.rotation = facing;
            _playerMovement.eyePivot.rotation = facing;
        }

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovementBehaviour>();
            _playerLifeCycleBehaviour = GetComponent<PlayerLifeCycleBehaviour>();
            _playerSlowMotion = GetComponent<PlayerSlowMotionBehaviour>();
            
            this.PanicIfNullObject(_playerSlowMotion, nameof(_playerSlowMotion));
        }

        private void HandleOnStateChanged(State state)
        {
            if (state == State.Play)
            {
                _playerMovement.SetControlConfiguration();
                _playerSlowMotion.ResumeTween();
            }

            if (state == State.Pause)
            {
                _playerSlowMotion.PauseTween();
            }
        }

        private void OnDestroy()
        {
            StateManager.Instance.OnStateChanged -= HandleOnStateChanged;
            if (_inputSystem != null)
            {
                _inputSystem.OnSlowTimeAction -= _playerSlowMotion.OnSlowMotion;
            }
        }
    }
}
