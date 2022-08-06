using System;
using DG.Tweening;
using Perigon.Character;
using Perigon.Entities;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(PlayerMovementBehaviour))]
    public class PlayerBehaviour : MonoBehaviour
    {
        private PlayerMovementBehaviour _playerMovement = null;
        private PlayerLifeCycleBehaviour _playerLifeCycle = null;
        private PlayerSlowMotionBehaviour _playerSlowMotion = null;
        private VisualEffectsManager _visualEffectsManager = null;

        public PlayerMovementBehaviour PlayerMovement => _playerMovement;

        public void Initialize(InputSettings inputSettings, VisualEffectsManager visualEffectsManager, Action onDeath)
        {
            _visualEffectsManager = visualEffectsManager;
            _playerMovement.Initialize(inputSettings);
            _playerSlowMotion.OnSlowMotionStart = OnSlowMotionStart;
            _playerSlowMotion.OnSlowMotionStopped = OnSlowMotionStopped;
            if (_playerLifeCycle != null)
            {
                _playerLifeCycle.Initialize(onDeath);
                _playerLifeCycle.OnDamageTaken = HandleOnHeal;
            }
        }
        
        public void SpawnAt(Vector3 position, Quaternion facing)
        {
            _playerMovement.SetVelocity(Vector3.zero);
            _playerMovement.SetPosition(position);
            _playerMovement.rootPivot.rotation = facing;
            _playerMovement.eyePivot.rotation = facing;
        }

        private void HandleOnDamageTaken()
        {
            _visualEffectsManager.DistortAndRevert(HUDVisualEffect.Health);
        }

        private void HandleOnHeal()
        {
            
        }

        private Tween OnSlowMotionStart()
        {
            return _visualEffectsManager.Distort(HUDVisualEffect.SlowMotion);
        }
        
        private Tween OnSlowMotionStopped()
        {
            return _visualEffectsManager.Revert(HUDVisualEffect.SlowMotion);
        }

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovementBehaviour>();
            if (_playerMovement == null)
            {
                PanicHelper.Panic(new Exception("PlayerMovementBehaviour missing from PlayerBehaviour"));
            }

            _playerLifeCycle = GetComponent<PlayerLifeCycleBehaviour>();
            if (_playerLifeCycle == null)
            {
                PanicHelper.Panic(new Exception("Player Life Cycle is missing from Player Behaviour"));
            }

            _playerSlowMotion = GetComponent<PlayerSlowMotionBehaviour>();
            if (_playerSlowMotion == null)
            {
                PanicHelper.Panic(new Exception("Player Slow Motion is missing from Player Behaviour"));
            }
        }
    }
}
