using System;
using Perigon.Character;
using Perigon.Entities;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Collider))]
    public class PlayerTriggerAreaEffect : MonoBehaviour
    {
        private enum HealthEffect
        {
            Damage,
            Heal
        }

        private enum SpeedModifier
        {
            SlowDown,
            SpeedUp
        }
        
        
        [Serializable]
        private class PlayerMutation
        {
            [Title("Player Effects", "Effect on Player over time", TitleAlignments.Split)]
            [SerializeField, Min(0.0f), Tooltip("Duration between player getting effected on")] private float _secondsBetweenEffect = 1.0f;
            [Title("Health Effect", horizontalLine: false)]
            [SerializeField] private HealthEffect _playerHealthEffect = HealthEffect.Damage;
            [SerializeField, Min(0), Tooltip("The amount to change the player's health by")] private int _healthChangeAmount = 0;

            [Title("Player Modifications", "Immediate modification on Player's stats", TitleAlignments.Split)]
            [Title("Speed Modification", horizontalLine: false)]
            [SerializeField, Tooltip("Effect on Player's max walk speed")] private SpeedModifier _playerSpeedModification = SpeedModifier.SlowDown;
            [SerializeField, Range(0.0f, 1.0f), Tooltip("modifer to multiply player's speed by")] private float _speedMultiplier = 0f;

            public float secondsBetweenEffect => _secondsBetweenEffect;
            public HealthEffect playerHealthEffect => _playerHealthEffect;
            public int healthChangeAmount => _healthChangeAmount;

            public SpeedModifier playerSpeedModification => _playerSpeedModification;
            public float speedMultiplier => _speedMultiplier;
        }
        
        [SerializeField] private PlayerMutation playerMutation;

        private PlayerLifeCycleBehaviour _playerLifeCycle;
        private PlayerMovementBehaviour _playerMovementBehaviour;
        private float _elapsedTime;

        private void Awake()
        {
            _elapsedTime = playerMutation.secondsBetweenEffect;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_playerLifeCycle != null && _playerMovementBehaviour != null)
            {
                return;
            }

            _playerLifeCycle = other.GetComponent<PlayerLifeCycleBehaviour>();
            _playerMovementBehaviour = other.GetComponent<PlayerMovementBehaviour>();

            if (_playerLifeCycle == null || _playerMovementBehaviour == null)
            {
                return;
            }
            
            EffectPlayer();
            ModifyPlayerStats();
        }

        private void Update()
        {
            if (_playerLifeCycle == null)
            {
                return;
            }
            
            _elapsedTime -= Time.deltaTime;
            if (_elapsedTime <= 0.0f)
            {
                EffectPlayer();
                _elapsedTime = playerMutation.secondsBetweenEffect;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(TagsAndLayers.Tags.Player))
            {
                return;
            }
            
            _playerLifeCycle = null;
            ResetPlayerStats();
            _elapsedTime = playerMutation.secondsBetweenEffect;
        }

        private void EffectPlayer()
        {
            switch (playerMutation.playerHealthEffect)
            {
                case HealthEffect.Damage:
                    _playerLifeCycle.DamageBy(playerMutation.healthChangeAmount);
                    break;
                case HealthEffect.Heal:
                    _playerLifeCycle.HealBy(playerMutation.healthChangeAmount);
                    break;
            }
        }

        private void ModifyPlayerStats()
        {
            switch (playerMutation.playerSpeedModification)
            {
                case SpeedModifier.SlowDown:
                    _playerMovementBehaviour.ModifyPlayerSpeed(-1 * playerMutation.speedMultiplier);
                    break;
                case SpeedModifier.SpeedUp:
                    _playerMovementBehaviour.ModifyPlayerSpeed(playerMutation.speedMultiplier);
                    break;
            }
        }

        private void ResetPlayerStats()
        {
            switch (playerMutation.playerSpeedModification)
            {
                case SpeedModifier.SlowDown:
                case SpeedModifier.SpeedUp:
                    _playerMovementBehaviour.ResetPlayerSpeed();
                    break;
            }
        }
    }
}
