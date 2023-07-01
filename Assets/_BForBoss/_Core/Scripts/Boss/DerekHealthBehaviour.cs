using System;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class DerekHealthBehaviour : MonoBehaviour, IBulletCollision
    {
        [SerializeField] private int _maxHealth = 12;
        [SerializeField] private int _damagePerShot = 1;

        private int _elapsedHealth;
        private float _threshold;
        private DerekContextManager.Vulnerability _vulnerabilityState = DerekContextManager.Vulnerability.Invulnerable;
        
        private event Action<DerekHealthViewState> OnStateChanged;
        private event Action OnThresholdHit;

        public DerekContextManager.Vulnerability VulnerabilityState
        {
            get => _vulnerabilityState;
            set
            {
                _vulnerabilityState = value;
                OnStateChanged?.Invoke(MapToState());
            } 
        }

        public void Initialize(
            Action onThresholdHit, 
            Action<DerekHealthViewState> onStateChanged)
        {
            OnThresholdHit = onThresholdHit;
            OnStateChanged = onStateChanged;
        }
        
        private void Awake()
        {
            _elapsedHealth = _maxHealth;
        }

        private float HealthPercentage => _elapsedHealth / _maxHealth;

        public void OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            if (VulnerabilityState == DerekContextManager.Vulnerability.Invulnerable)
            {
                return;
            }
            _elapsedHealth -= _damagePerShot;
            if (HealthPercentage <= _threshold)
            {
                OnThresholdHit?.Invoke();
            }
            OnStateChanged?.Invoke(MapToState());
        }

        public void SetNextThreshold(float healthPercentage)
        {
            _threshold = healthPercentage;
        }

        private DerekHealthViewState MapToState()
        {
            return new DerekHealthViewState(
                healthPercentage: HealthPercentage,
                isInvulnerable: VulnerabilityState == DerekContextManager.Vulnerability.Invulnerable);
        }
    }
}
