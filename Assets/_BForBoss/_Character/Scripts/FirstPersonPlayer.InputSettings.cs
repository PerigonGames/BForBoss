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
        private const float Default_Mouse_Sensitivity = 0.05f;
        private const float Default_Controller_Sensitivity = 0.05f;

        private InputActionMap UIActionMap => actions.FindActionMap(UIActionMapName);
        private InputActionMap PlayerControllerActionMap => actions.FindActionMap(PlayerControlActionMapName);
        
            
        private void SetupInput()
        {
            IInputSettings inputSettings = this;
            inputSettings.IsInverted = PlayerPrefs.GetInt(PlayerPrefKeys.InputSettings.IS_INVERTED, Default_Is_Inverted) == 1;
            inputSettings.MouseHorizontalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.MOUSE_HORIZONTAL_SENSITIVITY, Default_Mouse_Sensitivity);
            inputSettings.MouseVerticalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.MOUSE_VERTICAL_SENSITIVITY, Default_Mouse_Sensitivity);
            inputSettings.ControllerHorizontalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_HORIZONTAL_SENSITIVITY, Default_Controller_Sensitivity);
            inputSettings.ControllerVerticalSensitivity = PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_VERTICAL_SENSITIVITY, Default_Controller_Sensitivity);
        }

        bool IInputSettings.IsInverted
        {
            get => !GetCharacterLook().invertLook;

            set
            { 
                var isInverted = value ? 1 : 0; 
                PlayerPrefs.SetInt(PlayerPrefKeys.InputSettings.IS_INVERTED, isInverted);
                GetCharacterLook().invertLook = !value;
            }
        }

        float IInputSettings.MouseHorizontalSensitivity
        {
            get => GetCharacterLook().mouseHorizontalSensitivity;

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.MOUSE_HORIZONTAL_SENSITIVITY, value);
                GetCharacterLook().mouseHorizontalSensitivity = value;
            }
        }

        float IInputSettings.MouseVerticalSensitivity
        {
            get => GetCharacterLook().mouseVerticalSensitivity;

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.MOUSE_VERTICAL_SENSITIVITY, value);
                GetCharacterLook().mouseVerticalSensitivity = value;
            }
        }

        float IInputSettings.ControllerHorizontalSensitivity
        {
            get => GetCharacterLook().controllerHorizontalSensitivity;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_HORIZONTAL_SENSITIVITY, value);
                GetCharacterLook().controllerHorizontalSensitivity = value;   
            }     
        }

        float IInputSettings.ControllerVerticalSensitivity
        {
            get => GetCharacterLook().controllerVerticalSensitivity;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_VERTICAL_SENSITIVITY, value);
                GetCharacterLook().controllerVerticalSensitivity = value;   
            }  
        }

        void IInputSettings.SwapToPlayerActions()
        {
            PlayerControllerActionMap.Enable();
            UIActionMap.Disable();
        }

        void IInputSettings.SwapToUIActions()
        {
            PlayerControllerActionMap.Disable();
            UIActionMap.Enable();
        }

        void IInputSettings.RevertAllSettings()
        {
            IInputSettings inputSettings = this;
            inputSettings.IsInverted = Default_Is_Inverted == 1;
            inputSettings.MouseHorizontalSensitivity = Default_Mouse_Sensitivity;
            inputSettings.MouseVerticalSensitivity = Default_Mouse_Sensitivity;
            inputSettings.ControllerHorizontalSensitivity = Default_Controller_Sensitivity;
            inputSettings.ControllerVerticalSensitivity = Default_Controller_Sensitivity;
        }
    }
}