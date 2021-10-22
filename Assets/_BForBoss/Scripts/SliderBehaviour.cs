using System;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class SliderBehaviour : MonoBehaviour
    {
        private Action<float> _action = null;
        private Slider _customSlider = null;

        private Slider CustomSlider
        {
            get
            {
                if (_customSlider == null)
                {
                    _customSlider = GetComponentInChildren<Slider>();
                }

                return _customSlider;
            }
        }

        public float SliderValue
        {
             get=> CustomSlider.value;
             
             set => CustomSlider.value = value;
        }

        public void OnValueChanged(Action<float> action)
        {
            _action= action;
        }

        private void Awake()
        {
            CustomSlider.onValueChanged.AddListener(HandleOnValueChanged);
        }

        private void OnDestroy()
        {
            CustomSlider.onValueChanged.RemoveListener(HandleOnValueChanged);
        }

        private void HandleOnValueChanged(float value)
        {
            _action?.Invoke(value);
        }
    }
}