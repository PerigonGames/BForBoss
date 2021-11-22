using Perigon.Utility;
using UnityEngine;

namespace Perigon.Character 
{
    public partial class FirstPersonPlayer : IInputSettings
    {

        private const string PlayerControlActionMap = "Player Controls";
        
        private const int Default_Is_Inverted = 0;
        private const float Default_Mouse_Sensitivity = 0.4f;
        private const float Default_Controller_Sensitivity = 0.4f;
        
            
        private void SetupInput()
        {
            IsInverted = PlayerPrefs.GetInt(PlayerPrefKeys.InputSettings.Is_Inverted, Default_Is_Inverted) == 1;
            MouseHorizontalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.Mouse_Horizontal_Sensitivity, Default_Mouse_Sensitivity);
            MouseVerticalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.Mouse_Vertical_Sensitivity, Default_Mouse_Sensitivity);
            ControllerHorizontalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.Controller_Horizontal_Sensitivity, Default_Controller_Sensitivity);
            ControllerVerticalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.Controller_Vertical_Sensitivity,
                Default_Controller_Sensitivity);
        }
        
        public bool IsInverted
        {
            get => !GetCharacterLook().invertLook;

            set
            { 
                var isInverted = value ? 1 : 0; 
                PlayerPrefs.SetInt(PlayerPrefKeys.InputSettings.Is_Inverted, isInverted);
                GetCharacterLook().invertLook = !value;
            }
        }

        public float MouseHorizontalSensitivity
        {
            get => GetCharacterLook().mouseHorizontalSensitivity;

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.Mouse_Horizontal_Sensitivity, value);
                GetCharacterLook().mouseHorizontalSensitivity = value;
            }
        }

        public float MouseVerticalSensitivity
        {
            get => GetCharacterLook().mouseVerticalSensitivity;

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.Mouse_Vertical_Sensitivity, value);
                GetCharacterLook().mouseVerticalSensitivity = value;
            }
        }

        public float ControllerHorizontalSensitivity
        {
            get => GetCharacterLook().controllerHorizontalSensitivity;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.Controller_Horizontal_Sensitivity, value);
                GetCharacterLook().controllerHorizontalSensitivity = value;   
            }     
        }
        
        public float ControllerVerticalSensitivity
        {
            get => GetCharacterLook().controllerVerticalSensitivity;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.Controller_Vertical_Sensitivity, value);
                GetCharacterLook().controllerVerticalSensitivity = value;   
            }  
        }
        
        public void DisableActions()
        {
            actions.FindActionMap(PlayerControlActionMap).Disable();
        }

        public void EnableActions()
        {
            actions.FindActionMap(PlayerControlActionMap).Enable();
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