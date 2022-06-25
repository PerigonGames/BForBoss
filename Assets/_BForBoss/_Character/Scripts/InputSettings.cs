using ECM2.Components;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Character
{
    public class InputSettings : IInputSettings
    {
        private const string PlayerControlActionMapName = "Player Controls";
        private const string UIActionMapName = "UI";
        
        private const int Default_Is_Inverted = 0;
        private const float Default_Mouse_Sensitivity = 0.05f;
        private const float Default_Controller_Sensitivity = 0.05f;

        private InputActionAsset _inputActionAsset = null;
        private CharacterLook _characterLook = null;

        private InputActionMap UIActionMap => _inputActionAsset.FindActionMap(UIActionMapName);
        private InputActionMap PlayerControllerActionMap => _inputActionAsset.FindActionMap(PlayerControlActionMapName);


        public InputSettings(CharacterLook characterLook = null, InputActionAsset inputActionAsset = null)
        {
            _characterLook = characterLook;
            _inputActionAsset = inputActionAsset;
        }
        
        public void SetPlayerControls(CharacterLook characterLook, InputActionAsset inputActionAsset)
        {
            _characterLook = characterLook;
            _inputActionAsset = inputActionAsset;
            SetUpCharacterLook();
        }
        
        private void SetUpCharacterLook()
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
            get => PlayerPrefs.GetInt(PlayerPrefKeys.InputSettings.IS_INVERTED, Default_Is_Inverted) == 1;

            set
            { 
                var isInverted = value ? 1 : 0; 
                PlayerPrefs.SetInt(PlayerPrefKeys.InputSettings.IS_INVERTED, isInverted);
                _characterLook.invertLook = !value;
            }
        }

        public float MouseHorizontalSensitivity
        {
            get => PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.MOUSE_HORIZONTAL_SENSITIVITY, Default_Mouse_Sensitivity);

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.MOUSE_HORIZONTAL_SENSITIVITY, value);
                _characterLook.mouseHorizontalSensitivity = value;
            }
        }

        public float MouseVerticalSensitivity
        {
            get => PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.MOUSE_VERTICAL_SENSITIVITY, Default_Mouse_Sensitivity);

            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.MOUSE_VERTICAL_SENSITIVITY, value);
                _characterLook.mouseVerticalSensitivity = value;
            }
        }

        public float ControllerHorizontalSensitivity
        {
            get => PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_HORIZONTAL_SENSITIVITY, Default_Controller_Sensitivity);
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_HORIZONTAL_SENSITIVITY, value);
                _characterLook.controllerHorizontalSensitivity = value;   
            }     
        }
        
        public float ControllerVerticalSensitivity
        {
            get => PlayerPrefs.GetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_VERTICAL_SENSITIVITY,
                Default_Controller_Sensitivity);
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.InputSettings.CONTROLLER_VERTICAL_SENSITIVITY, value);
                _characterLook.controllerVerticalSensitivity = value;   
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
