using Perigon.Character;
using Perigon.Entities;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Collider))]
    public class TriggerAreaEffect : MonoBehaviour
    {
        private enum PlayerEffect
        {
            Damage,
            Heal
        }

        private enum PlayerModifier
        {
            SlowDown,
            SpeedUp
        }

        [Title("Player Effect", "Effect on Player over time", TitleAlignments.Split)]
        [SerializeField] private PlayerEffect _playerEffect = PlayerEffect.Damage;
        [SerializeField, Min(0), Tooltip("The amount to either damage or heal the player")] private int _effectAmount;
        [SerializeField, Min(0.0f), Tooltip("Duration between player getting effected on")] private float _secondsBetweenEffect = 1.0f;

        [Title("Player Modifier", "Modification on Player's stats", TitleAlignments.Split)]
        [SerializeField] private PlayerModifier _playerModifier = PlayerModifier.SlowDown;
        [SerializeField, Range(0, 100), Tooltip("Modification on Player stats as a percentage")]
        private int _modificationAmount = 50;  

        private PlayerLifeCycleBehaviour _playerLifeCycle;
        private PlayerMovementBehaviour _playerMovementBehaviour;
        private float _elapsedTime;
        private float _originalPlayerSpeed;

        private void Awake()
        {
            _elapsedTime = _secondsBetweenEffect;
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
                _elapsedTime = _secondsBetweenEffect;
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
            _elapsedTime = _secondsBetweenEffect;
        }

        private void EffectPlayer()
        {
            switch (_playerEffect)
            {
                case PlayerEffect.Damage:
                    _playerLifeCycle.DamageBy(_effectAmount);
                    break;
                case PlayerEffect.Heal:
                    _playerLifeCycle.HealBy(_effectAmount);
                    break;
            }
        }

        private void ModifyPlayerStats()
        {
            switch (_playerModifier)
            {
                case PlayerModifier.SlowDown:
                    _playerMovementBehaviour.ModifyPlayerSpeed(-1* _modificationAmount, out _originalPlayerSpeed);
                    break;
                case PlayerModifier.SpeedUp:
                    _playerMovementBehaviour.ModifyPlayerSpeed(_modificationAmount, out _originalPlayerSpeed);
                    break;
            }
        }

        private void ResetPlayerStats()
        {
            switch (_playerModifier)
            {
                case PlayerModifier.SlowDown:
                case PlayerModifier.SpeedUp:
                    _playerMovementBehaviour.ResetPlayerSpeed(_originalPlayerSpeed);
                    break;
            }
        }
    }
}
