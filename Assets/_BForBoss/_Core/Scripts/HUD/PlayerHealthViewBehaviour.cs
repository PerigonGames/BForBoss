using System;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.Entities
{
    public class PlayerHealthViewBehaviour : HealthViewBehaviour
    {
        [SerializeField]
        [Resolve] private Image _healthBar;
        private float _targetFillAmount = 0;
        
        public override void Reset()
        {
            _targetFillAmount = 1;
        }

        protected override void OnHealthChanged()
        {
            _targetFillAmount = Mathf.Clamp(GetHealthPercentage(), 0, 1);;
        }

        private void Update()
        {
            _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount, _targetFillAmount, Time.unscaledDeltaTime * 10f);
        }

        private void Awake()
        {
            if (_healthBar == null)
            {
                PanicHelper.Panic(new Exception("_healthBar missing from PlayerHealthViewBehaviour"));
            }
        }
    }
}
