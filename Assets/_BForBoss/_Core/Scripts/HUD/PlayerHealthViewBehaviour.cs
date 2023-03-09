using System;
using Perigon.Utility;
using TMPro;
using UnityEngine;

namespace Perigon.Entities
{
    public class PlayerHealthViewBehaviour : HealthViewBehaviour
    {
        [SerializeField]
        [Resolve] private TMP_Text _healthPercentageLabel = null;

        private string HealthAmountText => Mathf.Clamp(GetHealthPercentage() * 100, 0, float.MaxValue).ToString();

        public override void Reset()
        {
            _healthPercentageLabel.text = "100%";
        }

        protected override void OnHealthChanged()
        {
            _healthPercentageLabel.text = HealthAmountText + "%";
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
