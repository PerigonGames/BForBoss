using System;
using Perigon.Utility;
using TMPro;
using UnityEngine;

namespace Perigon.Entities
{
    public class PlayerHealthViewBehaviour : MonoBehaviour
    {
        [SerializeField]
        [Resolve] private TMP_Text _healthPercentageLabel = null;
        private ILifeCycle _lifeCycle = null;

        public void Initialize(ILifeCycle lifeCycle)
        {
            _lifeCycle = lifeCycle;
            _lifeCycle.OnDamageTaken += OnHealthChanged;
            _lifeCycle.OnHeal += OnHealthChanged;
            OnHealthChanged();
        }
        
        private void OnHealthChanged()
        {
            _healthPercentageLabel.text = Math.Floor(GetHealthAmount()) + "%";
        }
        
        private float GetHealthAmount()
        {
            if (_lifeCycle != null)
            {
                return _lifeCycle.CurrentHealth / _lifeCycle.MaxHealth * 100;
            }
            
            return 0;
        }

        private void Awake()
        {
            if (_healthPercentageLabel == null)
            {
                PanicHelper.Panic(new Exception("Health percentage label missing from PlayerHealthViewBehaviour"));
            }
        }
    }
}
