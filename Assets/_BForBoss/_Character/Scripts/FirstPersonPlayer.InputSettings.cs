using UnityEngine;

namespace BForBoss {
    public interface IInputSettings
    {
        bool IsInverted { get; set; }
        float MouseHorizontalSensitivity { get; set; }
        float MouseVerticalSensitivity { get; set; }
        float ControllerHorizontalSensitivity { get; set; }
        float ControllerVerticalSensitivity { get; set; }
        void RevertAllSettings();
        void DisableActions();
        void EnableActions();
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
        private const float Default_Mouse_Sensitivity = 0.4f;
        private const float Default_Controller_Sensitivity = 0.4f;
        
            
        private void SetupInput()
        {
            IsInverted = PlayerPrefs.GetInt(PlayerPrefKey.Is_Inverted, Default_Is_Inverted) == 1;
            MouseHorizontalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKey.Mouse_Horizontal_Sensitivity, Default_Mouse_Sensitivity);
            MouseVerticalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKey.Mouse_Vertical_Sensitivity, Default_Mouse_Sensitivity);
            ControllerHorizontalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKey.Controller_Horizontal_Sensitivity, Default_Controller_Sensitivity);
            ControllerVerticalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKey.Controller_Vertical_Sensitivity,
                Default_Controller_Sensitivity);
        }
        
        public bool IsInverted
        {
            get => !GetCharacterLook().invertLook;

            set
            { 
                var isInverted = value ? 1 : 0; 
                PlayerPrefs.SetInt(PlayerPrefKey.Is_Inverted, isInverted);
                GetCharacterLook().invertLook = !value;
            }
        }

        public float MouseHorizontalSensitivity
        {
            get => GetCharacterLook().mouseHorizontalSensitivity;

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKey.Mouse_Horizontal_Sensitivity, value);
                GetCharacterLook().mouseHorizontalSensitivity = value;
            }
        }

        public float MouseVerticalSensitivity
        {
            get => GetCharacterLook().mouseVerticalSensitivity;

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKey.Mouse_Vertical_Sensitivity, value);
                GetCharacterLook().mouseVerticalSensitivity = value;
            }
        }

        public float ControllerHorizontalSensitivity
        {
            get => GetCharacterLook().controllerHorizontalSensitivity;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKey.Controller_Horizontal_Sensitivity, value);
                GetCharacterLook().controllerHorizontalSensitivity = value;   
            }     
        }
        
        public float ControllerVerticalSensitivity
        {
            get => GetCharacterLook().controllerVerticalSensitivity;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKey.Controller_Vertical_Sensitivity, value);
                GetCharacterLook().controllerVerticalSensitivity = value;   
            }  
        }

        public void DisableActions()
        {
            _dashBehaviour?.DisableAction();
            crouchInputAction?.Disable();
            jumpInputAction?.Disable();
            movementInputAction?.Disable();
            cursorLockInputAction?.Disable();
            controllerLookInputAction?.Disable();
            mouseLookInputAction?.Disable();
        }

        public void EnableActions()
        {
            _dashBehaviour?.EnableAction();
            crouchInputAction?.Enable();
            jumpInputAction?.Enable();
            movementInputAction?.Enable();
            cursorLockInputAction?.Enable();
            controllerLookInputAction?.Enable();
            mouseLookInputAction?.Enable();
        }


        public void RevertAllSettings()
        {
            IsInverted = Default_Is_Inverted == 1;
            MouseHorizontalSensitivity = Default_Mouse_Sensitivity;
            MouseVerticalSensitivity = Default_Mouse_Sensitivity;
            ControllerHorizontalSensitivity = Default_Controller_Sensitivity;
            ControllerVerticalSensitivity = Default_Controller_Sensitivity;
        }
    }
}