using System;
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

        public PlayerMovementBehaviour PlayerMovement => _playerMovement;

        public void Initialize(InputSettings inputSettings, Action onDeath)
        {
            _playerMovement.Initialize(inputSettings);
            if (_playerLifeCycle != null)
            {
                _playerLifeCycle.Initialize(onDeath);
            }
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
            if (_playerMovement == null)
            {
                PanicHelper.Panic(new Exception("PlayerMovementBehaviour missing from PlayerBehaviour"));
            }

            _playerLifeCycle = GetComponent<PlayerLifeCycleBehaviour>();
            if (_playerLifeCycle == null)
            {
                Debug.LogWarning("Player Life Cycle is missing from Player Behaviour");
            }
        }
    }
}
