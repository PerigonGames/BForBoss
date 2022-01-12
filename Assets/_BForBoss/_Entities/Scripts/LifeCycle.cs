using System;
using UnityEngine;

namespace Perigon.Entities
{
    public interface ILifeCycle
    {
        public float MaxHealth { get; }
        public float CurrentHealth { get; }
        public bool Alive { get; }
    }
    
    
    public class LifeCycle : ILifeCycle
    {
        private readonly float _maxHealth;
        private float _currentHealth;

        public event Action OnDamageTaken;
        public event Action OnHeal;
        public event Action OnDeath;

        public bool Alive { get; private set; }

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;

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
