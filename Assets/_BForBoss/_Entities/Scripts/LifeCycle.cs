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

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;

        public LifeCycle(IHealth health)
        {
            _maxHealth = _currentHealth = health.Health;
            IsAlive = true;
        }

        public void Reset()
        {
            _currentHealth = MaxHealth;
            IsAlive = true;
        }
        
        public void HealBy(float amount)
        {
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            OnHeal?.Invoke();
        }

        public void DamageBy(float amount)
        {
            if (!IsAlive)
            {
                return;
            }
            _currentHealth -= amount;
            if (_currentHealth <= 0f)
            {
                IsAlive = false;
                OnDeath?.Invoke();
            }
            
            OnDamageTaken?.Invoke();
        }
    }
}
