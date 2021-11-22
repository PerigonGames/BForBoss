using System;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.UserInterface
{
    public class SliderBehaviour : MonoBehaviour
    {
        /// <summary>
        /// ECM2's sensitivity normally goes through 0.01 -> 2.0
        /// It looks too small and sensitive, so multiplying by 10 to go through 0.1 -> 25 
        /// </summary>
        private const float MappedSensitivityMultiplier = 10f;
        private Slider _customSlider = null;
        private TMP_InputField _inputField = null;

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

        private TMP_InputField CustomInputField
        {
            get
            {
                if (_inputField == null)
                {
                    _inputField = GetComponentInChildren<TMP_InputField>();
                }

                return _inputField;
            }
        }

        public float SliderValue
        {
             get=> CustomSlider.value / MappedSensitivityMultiplier;
             
             set => CustomSlider.value = value * MappedSensitivityMultiplier;
        }

        public Action OnValueChangedAction;

        private void Awake()
        {
            CustomSlider.onValueChanged.AddListener(HandleOnSliderValueChanged);
            CustomInputField.onEndEdit.AddListener(HandleOnInputFieldEnded);
        }

        private void OnDestroy()
        {
            CustomSlider.onValueChanged.RemoveListener(HandleOnSliderValueChanged);
            CustomInputField.onEndEdit.RemoveListener(HandleOnInputFieldEnded);
        }

        private void HandleOnSliderValueChanged(float value)
        {
            CustomInputField.text = value.ToString("F");
            OnValueChangedAction?.Invoke();
        }

        private void HandleOnInputFieldEnded(string value)
        {
            if (value.IsNullOrWhitespace())
            {
                CustomInputField.text = CustomSlider.value.ToString("F");
            }
            else
            {
                CustomSlider.value = float.Parse(value);
                OnValueChangedAction?.Invoke();
            }
        }
    }
}