using System;
using Perigon.Entities;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class EnemyHealthBarViewBehaviour : HealthViewBehaviour
    {
        [Resolve][SerializeField]private Image _healthBarImage;
        
        public override void Reset()
        {
            _healthBarImage.fillAmount = 1;
        }
        
        protected override void OnHealthChanged()
        {
            if (_healthBarImage != null)
            {
                _healthBarImage.fillAmount = GetHealthPercentage();
            }
        }
        
        private void Awake()
        {
            if (_healthBarImage == null)
            {
                PanicHelper.Panic(new Exception("HealthBarImage missing from HealthBarViewBehaviour"));
            }
        }
    }
}
