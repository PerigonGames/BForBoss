using System;
using UnityEngine;

namespace Perigon.Entities
{
    public class LifeCycle
    {
        private readonly float _maxHealth;
        private float _currentHealth;

        private bool _isAlive;
        
        public event Action OnDamageTaken;
        public event Action OnHeal;
        public event Action OnDeath;
        
        public LifeCycle(IHealth health)
        {
            _maxHealth = _currentHealth = health.Health;
            _isAlive = true;
        }
        
        public void HealBy(float amount)
        {
            OnHeal?.Invoke();
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        }

        public void DamageBy(float amount)
        {
            if (!_isAlive) return;
            _currentHealth -= amount;
            OnDamageTaken?.Invoke();
            if (_currentHealth <= 0f)
            {
                _isAlive = false;
                OnDeath?.Invoke();
            }
        }
    }
}
