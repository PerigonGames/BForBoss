using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public interface ICharacterSpeed
    {
        float Speed { get; }
    }
    
    public class SpeedometerBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _meter = null;
        [SerializeField] private TMP_Text _speedLabel = null;
        
        private ICharacterSpeed _characterSpeed = null;
        private float _maxSpeed = 50f;
        
        public void Initialize(ICharacterSpeed characterSpeed)
        {
            _characterSpeed = characterSpeed;
        }


        // Placeholder
        private void Awake()
        {
            var character = FindObjectOfType<FirstPersonPlayer>();
            Initialize(character);
        }

        private void Update()
        {
            if (_characterSpeed == null)
            {
                return;
            }
            
            SetMeter();
            SetSpeedLabel();
        }

        private void SetMeter()
        {
            var percentage = _characterSpeed.Speed / _maxSpeed;
            _meter.fillAmount = percentage;
        }

        private void SetSpeedLabel()
        {
            _speedLabel.text = _characterSpeed.Speed.ToString("F1");
        }
    }
}
