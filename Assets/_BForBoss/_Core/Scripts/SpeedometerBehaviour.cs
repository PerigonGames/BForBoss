using System;
using Perigon.Character;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class SpeedometerBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _meter = null;
        [SerializeField] private TMP_Text _speedLabel = null;
        [Title("Properties")]
        [MinValue(1)]
        [SerializeField] private float _maxSpeed = 50f;

        private Func<float> _characterSpeed = null;
        
        public void Initialize(Func<float> characterSpeed)
        {
            _characterSpeed = characterSpeed;
        }
        
        // Placeholder
        private void Awake()
        {
            var character = FindObjectOfType<Perigon.Character.PlayerMovementBehaviour>();
            Initialize(() => character.SpeedMagnitude);
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
            var percentage = _characterSpeed() / _maxSpeed;
            _meter.fillAmount = percentage;
        }

        private void SetSpeedLabel()
        {
            _speedLabel.text = _characterSpeed().ToString("F1");
        }
    }
}
