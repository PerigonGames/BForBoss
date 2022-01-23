using System;
using UnityEngine;

namespace Perigon.Entities
{
    public interface ILifeCycle
    {
        float MaxHealth { get; }
        float CurrentHealth { get; }
        bool IsAlive { get; }

        event Action OnDamageTaken;
        event Action OnHeal;
        event Action OnDeath;
    }
    
    
    public class LifeCycle : ILifeCycle
    {
        private readonly float _maxHealth;
        private float _currentHealth;

        public event Action OnDamageTaken;
        public event Action OnHeal;
        public event Action OnDeath;

        public bool IsAlive { get; private set; }

        float ILifeCycle.MaxHealth => _maxHealth;
        float ILifeCycle.CurrentHealth => _currentHealth;

        public LifeCycle(IHealth health)
        {
            _maxHealth = _currentHealth = health.Health;
            IsAlive = true;
        }
        
        public void HealBy(float amount)
        {
            OnHeal?.Invoke();
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        }

        public void DamageBy(float amount)
        {
            if (!IsAlive) return;
            _currentHealth -= amount;
            OnDamageTaken?.Invoke();
            if (_currentHealth <= 0f)
            {
                IsAlive = false;
                OnDeath?.Invoke();
            }
        }
    }
}
