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
        private ILifeCycle _lifeCycle;
        private bool _isAlive;

        private event Action OnDamageTaken;
        private event Action OnHeal;
        private event Action OnDeath;

        event Action ILifeCycle.OnDamageTaken
        {
            add => OnDamageTaken += value;
            remove => OnDamageTaken -= value;
        }
        
        event Action ILifeCycle.OnHeal
        {
            add => OnHeal += value;
            remove => OnHeal -= value;
        }

        event Action ILifeCycle.OnDeath
        {
            add => OnDeath += value;
            remove => OnDeath -= value;
        }

        bool ILifeCycle.IsAlive => _isAlive;

        float ILifeCycle.MaxHealth => _maxHealth;
        float ILifeCycle.CurrentHealth => _currentHealth;

        public LifeCycle(IHealth health)
        {
            _maxHealth = _currentHealth = health.Health;
            _isAlive = true;
            _lifeCycle = this;
        }

        public void Reset()
        {
            _currentHealth = MaxHealth;
            IsAlive = true;
        }
        
        public void HealBy(float amount)
        {
            OnHeal?.Invoke();
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        }

        public void DamageBy(float amount)
        {
            if (!_lifeCycle.IsAlive) return;
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
