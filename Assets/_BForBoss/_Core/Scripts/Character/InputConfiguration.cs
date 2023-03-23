using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class InputConfiguration : IInputConfiguration
    {
        private const int DEFAULT_IS_INVERTED = 0;
        private const float DEFAULT_MOUSE_SENSITIVITY = 0.05f;
        private const float DEFAULT_CONTROLLER_SENSITIVITY = 0.05f;
        
        public bool IsInverted
        {
            get => PlayerPrefs.GetInt(PlayerPrefKeys.InputSettings.IS_INVERTED, DEFAULT_IS_INVERTED) == 1;

            set
            { 
                var isInverted = value ? 1 : 0; 
                PlayerPrefs.SetInt(PlayerPrefKeys.InputSettings.IS_INVERTED, isInverted);
            }
        }

        public float MouseHorizontalSensitivity
        {
            get => PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.MOUSE_HORIZONTAL_SENSITIVITY, DEFAULT_MOUSE_SENSITIVITY);

            set => PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.MOUSE_HORIZONTAL_SENSITIVITY, value);
        }

        public float MouseVerticalSensitivity
        {
            get => PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.MOUSE_VERTICAL_SENSITIVITY, DEFAULT_MOUSE_SENSITIVITY);

            set => PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.MOUSE_VERTICAL_SENSITIVITY, value);
        }

        public float ControllerHorizontalSensitivity
        {
            get => PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_HORIZONTAL_SENSITIVITY, DEFAULT_CONTROLLER_SENSITIVITY);
            set => PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_HORIZONTAL_SENSITIVITY, value);
        }
        
        public float ControllerVerticalSensitivity
        {
            get => PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_VERTICAL_SENSITIVITY,
                DEFAULT_CONTROLLER_SENSITIVITY);
            set => PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_VERTICAL_SENSITIVITY, value);
        }

        public void RevertAllSettings()
        {
            IsInverted = DEFAULT_IS_INVERTED == 1;
            MouseHorizontalSensitivity = DEFAULT_MOUSE_SENSITIVITY;
            MouseVerticalSensitivity = DEFAULT_MOUSE_SENSITIVITY;
            ControllerHorizontalSensitivity = DEFAULT_CONTROLLER_SENSITIVITY;
            ControllerVerticalSensitivity = DEFAULT_CONTROLLER_SENSITIVITY;
        }
    }
}
