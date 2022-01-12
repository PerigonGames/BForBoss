using Perigon.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Character 
{
    public partial class FirstPersonPlayer : IInputSettings
    {

        private const string PlayerControlActionMapName = "Player Controls";
        private const string UIActionMapName = "UI";
        
        private const int Default_Is_Inverted = 0;
        private const float Default_Mouse_Sensitivity = 0.4f;
        private const float Default_Controller_Sensitivity = 0.4f;

        private InputActionMap UIActionMap => actions.FindActionMap(UIActionMapName);
        private InputActionMap PlayerControllerActionMap => actions.FindActionMap(PlayerControlActionMapName);
        
            
        private void SetupInput()
        {
            IsInverted = PlayerPrefs.GetInt(PlayerPrefKeys.InputSettings.IS_INVERTED, Default_Is_Inverted) == 1;
            MouseHorizontalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.MOUSE_HORIZONTAL_SENSITIVITY, Default_Mouse_Sensitivity);
            MouseVerticalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.MOUSE_VERTICAL_SENSITIVITY, Default_Mouse_Sensitivity);
            ControllerHorizontalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_HORIZONTAL_SENSITIVITY, Default_Controller_Sensitivity);
            ControllerVerticalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_VERTICAL_SENSITIVITY,
                Default_Controller_Sensitivity);
        }
        
        public bool IsInverted
        {
            get => !GetCharacterLook().invertLook;

            set
            { 
                var isInverted = value ? 1 : 0; 
                PlayerPrefs.SetInt(PlayerPrefKeys.InputSettings.IS_INVERTED, isInverted);
                GetCharacterLook().invertLook = !value;
            }
        }

        public float MouseHorizontalSensitivity
        {
            get => GetCharacterLook().mouseHorizontalSensitivity;

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.MOUSE_HORIZONTAL_SENSITIVITY, value);
                GetCharacterLook().mouseHorizontalSensitivity = value;
            }
        }

        public float MouseVerticalSensitivity
        {
            get => GetCharacterLook().mouseVerticalSensitivity;

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.MOUSE_VERTICAL_SENSITIVITY, value);
                GetCharacterLook().mouseVerticalSensitivity = value;
            }
        }

        public float ControllerHorizontalSensitivity
        {
            get => GetCharacterLook().controllerHorizontalSensitivity;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_HORIZONTAL_SENSITIVITY, value);
                GetCharacterLook().controllerHorizontalSensitivity = value;   
            }     
        }
        
        public float ControllerVerticalSensitivity
        {
            get => GetCharacterLook().controllerVerticalSensitivity;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_VERTICAL_SENSITIVITY, value);
                GetCharacterLook().controllerVerticalSensitivity = value;   
            }  
        }

        public void SwapToPlayerActions()
        {
            PlayerControllerActionMap.Enable();
            UIActionMap.Disable();
        }
        
        public void SwapToUIActions()
        {
            PlayerControllerActionMap.Disable();
            UIActionMap.Enable();
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