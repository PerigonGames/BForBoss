using System;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.Entities
{
    [RequireComponent(typeof(Image))]
    public class HealthbarViewBehaviour : MonoBehaviour
    {
        private Image _healthbarImage;
        private ILifeCycle _lifeCycle = null;

        private void Awake()
        {
            _healthbarImage = GetComponent<Image>();
        }

        public void Initialize(ILifeCycle lifeCycle)
        {
            _lifeCycle = lifeCycle;
            
            
        }

        private void Update()
        {
            _healthbarImage.fillAmount = GetHealthBarAmount();
        }

        private float GetHealthBarAmount()
        {
            if (_lifeCycle != null)
            {
                return _lifeCycle.CurrentHealth / _lifeCycle.MaxHealth;
            }
            return 1f;
        }
    }
}
