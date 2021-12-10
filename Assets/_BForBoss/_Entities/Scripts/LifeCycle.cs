using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Entities
{
    public class LifeCycle
    {
        private readonly float _maxHealth;
        private float _currentHealth;

        public event Action OnDamageTaken;
        public event Action OnHeal;
        public event Action OnDeath;
        
        public LifeCycle(IHealth health)
        {
            _maxHealth = _currentHealth = health.Health;
        }
        
        public void HealBy(float amount)
        {
            OnHeal?.Invoke();
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        }

        public void DamageBy(float amount)
        {
            _currentHealth -= amount;
            OnDamageTaken?.Invoke();
            if (_currentHealth <= 0f)
            {
                OnDeath?.Invoke();
            }
        }
    }
}
