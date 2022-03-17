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
            _lifeCycle.OnDamageTaken += OnHealthChanged;
            _lifeCycle.OnHeal += OnHealthChanged;
            OnHealthChanged();
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
            if (_healthbarImage != null)
            {
                _healthbarImage.fillAmount = GetHealthBarAmount();
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

        public void Reset()
        {
            OnHealthChanged();
        }
    }
}
