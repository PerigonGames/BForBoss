using FMODUnity;
using Perigon.Entities;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Collider))]
    public class PlayerTriggerAreaEffect : MonoBehaviour
    {
        [SerializeField] private PlayerTriggerAreaMutation playerTriggerAreaMutation;
        [SerializeField] private EventReference _eventReference;

        private PlayerLifeCycleBehaviour _playerLifeCycle;
        private PlayerMovementBehaviour _playerMovementBehaviour;
        private float _elapsedTime;

        private void Awake()
        {
            _elapsedTime = playerTriggerAreaMutation.secondsBetweenEffect;
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
            
            PerformOneTimePlayerStatChange(other.transform.position);
            PerformRepeatedPlayerStatChange();
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
                PerformRepeatedPlayerStatChange();
                _elapsedTime = playerTriggerAreaMutation.secondsBetweenEffect;
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
            _elapsedTime = playerTriggerAreaMutation.secondsBetweenEffect;
        }

        private void PerformRepeatedPlayerStatChange()
        {
            int healthChangeAmount = playerTriggerAreaMutation.healthChangeAmount;
            if (healthChangeAmount < 0)
            {
                _playerLifeCycle.DamageBy(-1 * healthChangeAmount);
            }
            else if (healthChangeAmount > 0)
            {
                _playerLifeCycle.HealBy(healthChangeAmount);
            }
        }

        private void PerformOneTimePlayerStatChange(Vector3 contactPoint)
        {
            if(!_eventReference.IsNull)
                RuntimeManager.PlayOneShot(_eventReference, contactPoint);
            _playerMovementBehaviour.ModifyPlayerSpeed(playerTriggerAreaMutation.speedMultiplier);
        }

        private void ResetPlayerStats()
        {
            _playerMovementBehaviour.RevertPlayerSpeed();
        }
    }
}
