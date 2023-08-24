using System;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class DerekHealthBehaviour : MonoBehaviour, IBulletCollision
    {
        [SerializeField] private int _maxHealth = 20;
        [SerializeField] private int _damagePerShot = 1;

        [SerializeField] private DerekHealthView _healthView;

        private int _elapsedHealth;
        private float _threshold;
        private DerekContextManager.Vulnerability _vulnerabilityState = DerekContextManager.Vulnerability.Invulnerable;

        private bool _canReceiveDamage = false;

        private event Action<DerekHealthViewState> OnStateChanged;
        private event Action OnThresholdHit;

        public bool CanReceiveDamage
        {
            get => _canReceiveDamage;
            set
            {
                _canReceiveDamage = value;
                _healthView.SetState(new DerekHealthViewState(HealthPercentage, !_canReceiveDamage));
            }
        }

        private float HealthPercentage => (float)_elapsedHealth / _maxHealth;

        public void Initialize(Action onThresholdHit)
        {
            OnThresholdHit = onThresholdHit;
        }

        public void Reset()
        {
            _elapsedHealth = _maxHealth;
            CanReceiveDamage = false;
        }

        private void Awake()
        {
            this.PanicIfNullObject(_healthView, nameof(_healthView));

            if (_maxHealth <= 0)
            {
                PanicHelper.Panic(new Exception($"{nameof(_maxHealth)} needs to be a positive number, instead it is set to {_maxHealth}"));
            }

            _elapsedHealth = _maxHealth;
        }

        public void OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            if (!CanReceiveDamage)
            {
                return;
            }

            _elapsedHealth -= _damagePerShot;
            _healthView.SetState(new DerekHealthViewState(HealthPercentage, !CanReceiveDamage));
            
            if (HealthPercentage <= _threshold)
            {
                OnThresholdHit?.Invoke();
            }
        }

        public void SetThreshold(float healthPercentage, Color healthBarColor)
        {
            if (healthPercentage < 0.0f || healthPercentage > 1.0f)
            {
                Perigon.Utility.Logger.LogWarning("Percentage is not between 0 and 1, clamping", LoggerColor.Yellow, "derekboss");
                healthPercentage = Math.Clamp(healthPercentage, 0.0f, 1.0f);
            }
            
            _threshold = healthPercentage;
            _healthView.SetHealthBarColor(healthBarColor);
        }
    }
}
