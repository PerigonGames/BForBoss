using System;
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

        public bool Alive { get; private set; }

        public LifeCycle(IHealth health)
        {
            _maxHealth = _currentHealth = health.Health;
            Alive = true;
        }
        
        public void HealBy(float amount)
        {
            OnHeal?.Invoke();
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        }

        public void DamageBy(float amount)
        {
            if (!Alive) return;
            _currentHealth -= amount;
            OnDamageTaken?.Invoke();
            if (_currentHealth <= 0f)
            {
                Alive = false;
                OnDeath?.Invoke();
            }
        }
    }
}
