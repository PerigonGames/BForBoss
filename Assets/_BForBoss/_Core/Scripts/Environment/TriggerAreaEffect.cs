using Perigon.Entities;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Collider))]
    public class TriggerAreaEffect : MonoBehaviour
    {
        private enum Effector
        {
            Damage,
            Heal
        }

        [SerializeField] private Effector _effectOnPlayer = Effector.Damage;
        [SerializeField, Min(0), Tooltip("The amount to either damage or heal the player")] private int _effectAmount;
        [SerializeField, Min(0.0f), Tooltip("Duration between player getting effected on")] private float _secondsBetweenEffect = 1.0f;
        
        private PlayerLifeCycleBehaviour _playerLifeCycle;
        private float _elapsedTime;

        private void Awake()
        {
            _elapsedTime = _secondsBetweenEffect;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_playerLifeCycle != null)
            {
                return;
            }

            _playerLifeCycle = other.GetComponent<PlayerLifeCycleBehaviour>();

            if (_playerLifeCycle == null)
            {
                return;
            }
            
            EffectPlayer();
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
            if (_playerLifeCycle == null)
            {
                return;
            }

            _playerLifeCycle = null;
            _elapsedTime = _secondsBetweenEffect;
        }

        private void EffectPlayer()
        {
            switch (_effectOnPlayer)
            {
                case Effector.Damage:
                    _playerLifeCycle.DamageBy(_effectAmount);
                    break;
                case Effector.Heal:
                    _playerLifeCycle.HealBy(_effectAmount);
                    break;
            }
        }
    }
}
