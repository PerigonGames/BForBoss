using UnityEngine;

namespace BForBoss {
    public interface IInputSettings
    {
        void SetInverted(bool isInverted);
        void SetMouseHorizontalSensitivity(float sensitivity);
        void SetMouseVerticalSensitivity(float sensitivity);
        void SetControllerHorizontalSensitivity(float sensitivity);
        void SetControllerVerticalSensitivity(float sensitivity);
        void RevertAllSettings();
    }

    public partial class FirstPersonPlayer : IInputSettings
    {
        private struct PlayerPrefKey
        {
            public const string Is_Inverted = "is_inverted";
            public const string Mouse_Horizontal_Sensitivity = "mouse_horizontal_sensitivity";
            public const string Mouse_Vertical_Sensitivity = "mouse_vertical_sensitivity";
            public const string Controller_Horizontal_Sensitivity = "controller_horizontal_sensitivity";
            public const string Controller_Vertical_Sensitivity = "controller_vertical_sensitivity";
        }
        
        private const int Default_Is_Inverted = 0;
        private const float Default_Mouse_Sensitivity = 0.5f;
        private const float Default_Controller_Sensitivity = 0.5f;
        
            
        private void SetupInput()
        {
            var inverted = PlayerPrefs.GetInt(PlayerPrefKey.Is_Inverted, Default_Is_Inverted);
            SetInverted(inverted == 1);
            
            var mouseHorizontal = PlayerPrefs.GetFloat(PlayerPrefKey.Mouse_Horizontal_Sensitivity, Default_Mouse_Sensitivity);
            SetMouseHorizontalSensitivity(mouseHorizontal);
            var mouseVertical = PlayerPrefs.GetFloat(PlayerPrefKey.Mouse_Vertical_Sensitivity, Default_Mouse_Sensitivity);
            SetMouseVerticalSensitivity(mouseVertical);

            var controllerHorizontal =
                PlayerPrefs.GetFloat(PlayerPrefKey.Controller_Horizontal_Sensitivity, Default_Controller_Sensitivity);
            SetControllerHorizontalSensitivity(controllerHorizontal);
            var controllerVertical =
                PlayerPrefs.GetFloat(PlayerPrefKey.Controller_Vertical_Sensitivity, Default_Controller_Sensitivity);
            SetControllerVerticalSensitivity(controllerVertical);
        }
        
        public void SetInverted(bool isInverted)
        {
            var value = isInverted ? 1 : 0;
            PlayerPrefs.SetInt(PlayerPrefKey.Is_Inverted, value);
            GetCharacterLook().invertLook = isInverted;
        }

        public void SetMouseHorizontalSensitivity(float sensitivity)
        {
            PlayerPrefs.SetFloat(PlayerPrefKey.Mouse_Horizontal_Sensitivity, sensitivity);
            GetCharacterLook().mouseHorizontalSensitivity = sensitivity;
        }

        public void SetMouseVerticalSensitivity(float sensitivity)
        {
            PlayerPrefs.SetFloat(PlayerPrefKey.Mouse_Vertical_Sensitivity, sensitivity);
            GetCharacterLook().mouseVerticalSensitivity = sensitivity;
        }

        public void SetControllerHorizontalSensitivity(float sensitivity)
        {
            PlayerPrefs.SetFloat(PlayerPrefKey.Controller_Horizontal_Sensitivity, sensitivity);
            GetCharacterLook().controllerHorizontalSensitivity = sensitivity;        
        }

        public void SetControllerVerticalSensitivity(float sensitivity)
        {
            PlayerPrefs.SetFloat(PlayerPrefKey.Controller_Vertical_Sensitivity, sensitivity);
            GetCharacterLook().controllerVerticalSensitivity = sensitivity;     
        }

        public void RevertAllSettings()
        {
            SetInverted(Default_Is_Inverted == 1);
            SetMouseHorizontalSensitivity(Default_Mouse_Sensitivity);
            SetMouseVerticalSensitivity(Default_Mouse_Sensitivity);
            SetControllerHorizontalSensitivity(Default_Controller_Sensitivity);
            SetControllerVerticalSensitivity(Default_Controller_Sensitivity);
        }
    }
}