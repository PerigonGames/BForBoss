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
        private PlayerHookshotBehaviour _playerHookshot;
        private PlayerRailGrindBehaviour _playerRailGrind;
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
            _playerHookshot.Initialize(inputSystem, _playerMovement);
            _playerRailGrind.Initialize(_playerMovement);
            _inputSystem = inputSystem;
            _inputSystem.OnSlowTimeAction += _playerSlowMotion.OnSlowMotion;
            StateManager.Instance.OnStateChanged += HandleOnStateChanged;
        }

        public void Reset()
        {
            _playerLifeCycleBehaviour.Reset();
            _playerRailGrind.Reset();
            _playerHookshot.Reset();    
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
            _playerHookshot = GetComponent<PlayerHookshotBehaviour>();
            _playerRailGrind = GetComponent<PlayerRailGrindBehaviour>();
            
            this.PanicIfNullObject(_playerSlowMotion, nameof(_playerSlowMotion) );
            this.PanicIfNullObject(_playerHookshot, nameof(_playerHookshot));
            this.PanicIfNullObject(_playerRailGrind, nameof(_playerRailGrind));
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
