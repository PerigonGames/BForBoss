using System;
using UnityEngine;

namespace Perigon.Entities
{
    public interface ILifeCycle
    {
        float MaxHealth { get; }
        float CurrentHealth { get; }
        bool IsAlive { get; }
        bool IsInvincible { get; set; }

        event Action OnDamageTaken;
        event Action OnHeal;
        event Action OnDeath;
    }
    
    
    public class LifeCycle : ILifeCycle
    {
        private float _maxHealth;
        private float _currentHealth;

        public event Action OnDamageTaken;
        public event Action OnHeal;
        public event Action OnDeath;

        public bool IsAlive { get; private set; }
        public bool IsInvincible { get; set; }

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;

        public LifeCycle(float health = 100f)
        {
            _maxHealth = _currentHealth = health;
            IsAlive = true;
        }

        public void SetHealth(float health)
        {
            _maxHealth = _currentHealth = health;
        }

        public void Reset()
        {
            _currentHealth = MaxHealth;
            IsAlive = true;
            OnHeal?.Invoke();
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

            if (!IsInvincible)
            {
                _currentHealth -= amount;
            }
            
            if (_currentHealth <= 0f)
            {
                IsAlive = false;
                OnDeath?.Invoke();
            }
            
            OnDamageTaken?.Invoke();
        }
    }
}
