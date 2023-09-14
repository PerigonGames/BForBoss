using System;
using FMODUnity;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class DerekHealthBehaviour : MonoBehaviour, IBulletCollision
    {
        [SerializeField] private int _maxHealth = 20;
        [SerializeField] private int _damagePerShot = 1;
        [SerializeField] private EventReference _hitAudio;

        [SerializeField] private DerekHealthView _healthView;
        private DerekHealthViewState _healthViewState;

        private int _elapsedHealth;
        private float _threshold;

        private bool _isVulnerable = false;
        private event Action OnThresholdHit;

        public bool IsVulnerable
        {
            get => _isVulnerable;
            set
            {
                _isVulnerable = value;
                _healthViewState = _healthViewState.Apply(HealthPercentage, !_isVulnerable);
                _healthView.SetState(_healthViewState);
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
            IsVulnerable = false;
            _healthView.gameObject.SetActive(false);
        }

        public void ShowHealthView()
        {
            _healthView.gameObject.SetActive(true);
        }

        private void Awake()
        {
            this.PanicIfNullObject(_healthView, nameof(_healthView));

            if (_maxHealth <= 0)
            {
                PanicHelper.Panic(new Exception($"{nameof(_maxHealth)} needs to be a positive number, instead it is set to {_maxHealth}"));
            }

            _elapsedHealth = _maxHealth;
            _healthViewState = new DerekHealthViewState();
        }

        public void OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            if (!IsVulnerable)
            {
                return;
            }

            if(!_hitAudio.IsNull) RuntimeManager.PlayOneShot(_hitAudio, collisionPoint);
            _elapsedHealth -= _damagePerShot;
            _healthViewState = _healthViewState.Apply(HealthPercentage, !IsVulnerable);
            _healthView.SetState(_healthViewState);
            
            if (HealthPercentage <= _threshold)
            {
                OnThresholdHit?.Invoke();
            }
        }

        public void SetThreshold(float threshold, Color healthBarColor)
        {
            threshold = Math.Clamp(threshold, 0.0f, 1.0f);
            _threshold = threshold;
            _healthViewState = _healthViewState.Apply(null, null, healthBarColor);
            _healthView.SetState(_healthViewState);
        }
    }
}
