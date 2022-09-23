using System;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.Entities
{
    public class HealthBarViewBehaviour : MonoBehaviour
    {
        [Resolve][SerializeField]private Image _healthBarImage;
        private ILifeCycle _lifeCycle = null;

        private void Awake()
        {
            if (_healthBarImage == null)
            {
                PanicHelper.Panic(new Exception("HealthBarImage missing from HealthBarViewBehaviour"));
            }
        }

        public void Initialize(ILifeCycle lifeCycle)
        {
            _lifeCycle = lifeCycle;
            _lifeCycle.OnDamageTaken += OnHealthChanged;
            _lifeCycle.OnHeal += OnHealthChanged;
            OnHealthChanged();
        }
        
        public void Reset()
        {
            _healthBarImage.fillAmount = 1;
        }
        
        private float GetHealthBarAmount()
        {
            if (_lifeCycle != null)
            {
                return _lifeCycle.CurrentHealth / _lifeCycle.MaxHealth;
            }
            return 0f;
        }

        private void OnHealthChanged()
        {
            if (_healthBarImage != null)
            {
                _healthBarImage.fillAmount = GetHealthBarAmount();
            }
        }
        
        private void OnEnable()
        {
            if (_lifeCycle != null)
            {
                _lifeCycle.OnHeal += OnHealthChanged;
                _lifeCycle.OnDamageTaken += OnHealthChanged;
            }
        }

        private void OnDisable()
        {
            if (_lifeCycle != null)
            {
                _lifeCycle.OnHeal -= OnHealthChanged;
                _lifeCycle.OnDamageTaken -= OnHealthChanged;
            }
        }
    }
}
