using System;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.UserInterface
{
    public class SliderBehaviour : MonoBehaviour
    {

        private Slider _customSlider = null;
        private TMP_InputField _inputField = null;

        [SerializeField] bool _wholeNumbers = false;

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
             get=> CustomSlider.value;
             
             set => CustomSlider.value = value;
        }

        private string FormatString => _wholeNumbers ? "F0" : "F";

        public Action OnValueChangedAction;

        private void Awake()
        {
            CustomSlider.onValueChanged.AddListener(HandleOnSliderValueChanged);
            CustomInputField.onEndEdit.AddListener(HandleOnInputFieldEnded);
            CustomSlider.wholeNumbers = _wholeNumbers;
            CustomInputField.contentType = _wholeNumbers
                ? TMP_InputField.ContentType.IntegerNumber
                : TMP_InputField.ContentType.DecimalNumber;
        }

        private void OnDestroy()
        {
            CustomSlider.onValueChanged.RemoveListener(HandleOnSliderValueChanged);
            CustomInputField.onEndEdit.RemoveListener(HandleOnInputFieldEnded);
        }

        private void HandleOnSliderValueChanged(float value)
        {
            CustomInputField.text = value.ToString(FormatString);
            OnValueChangedAction?.Invoke();
        }

        private void HandleOnInputFieldEnded(string value)
        {
            if (value.IsNullOrWhitespace())
            {
                CustomInputField.text = CustomSlider.value.ToString(FormatString);
            }
            else
            {
                CustomSlider.value = float.Parse(value);
                OnValueChangedAction?.Invoke();
            }
        }

        private void OnValidate()
        {
            CustomSlider.wholeNumbers = _wholeNumbers;
            CustomInputField.contentType = _wholeNumbers
                ? TMP_InputField.ContentType.IntegerNumber
                : TMP_InputField.ContentType.DecimalNumber;
        }
    }
}