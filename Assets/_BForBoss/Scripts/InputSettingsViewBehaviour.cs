using DG.Tweening;
using PerigonGames;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BForBoss
{
    public class InputSettingsViewBehaviour : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField] private SliderBehaviour _mouseHorizontalSlider = null;
        [SerializeField] private SliderBehaviour _mouseVerticalSlider = null;
        [SerializeField] private SliderBehaviour _controllerHorizontalSlider = null;
        [SerializeField] private SliderBehaviour _controllerVerticalSlider = null;
        [SerializeField] private Toggle _toggle = null;

        [Title("Button")] 
        [SerializeField] private Button _applyButton = null;
        [SerializeField] private Button _revertButton = null;
        [SerializeField] private Button _backButton = null;

        private InputSettingsViewModel _viewModel = null;
        
        public void Initialize(InputSettingsViewModel viewModel)
        {
            _viewModel = viewModel;
            transform.localScale = Vector3.zero;
            SetupSliders();
            BindSliders();

            _revertButton.onClick.AddListener(() =>
            {
                _viewModel.RevertSettings();
                SetupSliders();
            });
            
            _applyButton.onClick.AddListener(() =>
            {
                _viewModel.ApplySettings(
                    _mouseHorizontalSlider.SliderValue, 
                    _mouseVerticalSlider.SliderValue,
                    _controllerHorizontalSlider.SliderValue,
                    _controllerVerticalSlider.SliderValue,
                    _toggle.isOn);
                _applyButton.interactable = false;
            });
            
            _backButton.onClick.AddListener(() =>
            {
                transform.DOScale(Vector3.zero, 0.5f);
                _viewModel.SetMouseLock(true);
            });
        }

        private void BindSliders()
        {
            _mouseHorizontalSlider.OnValueChangedAction = () =>
            {
                _applyButton.interactable = true;
            };
            
            _mouseVerticalSlider.OnValueChangedAction = () =>
            {
                _applyButton.interactable = true;
            };
            
            _controllerHorizontalSlider.OnValueChangedAction = () =>
            {
                _applyButton.interactable = true;
            };
            
            _controllerVerticalSlider.OnValueChangedAction = () =>
            {
                _applyButton.interactable = true;
            };
            
            _toggle.onValueChanged.AddListener((_) =>
            {
                _applyButton.interactable = true;
            });
        }

        private void SetupSliders()
        {
            _mouseHorizontalSlider.SliderValue = _viewModel.GetMouseHorizontal;
            _mouseVerticalSlider.SliderValue = _viewModel.GetMouseVertical;
            _controllerHorizontalSlider.SliderValue = _viewModel.GetControllerHorizontal;
            _controllerVerticalSlider.SliderValue = _viewModel.GetControllerVeritcal;
            _toggle.isOn = _viewModel.GetIsInverted;
            _applyButton.interactable = false;
        }

        private void Update()
        {
            if (Keyboard.current.digit0Key.wasPressedThisFrame)
            {
                _viewModel.SetMouseLock(false);
                transform.ResetScale();
            }
        }
    }

    public class InputSettingsViewModel
    {
        private readonly IInputSettings _settings = null;
        private readonly PerigonAnalytics _perigonAnalytics = PerigonAnalytics.Instance;

        public float GetMouseHorizontal => _settings.MouseHorizontalSensitivity;
        public float GetMouseVertical => _settings.MouseVerticalSensitivity;
        public float GetControllerHorizontal => _settings.ControllerHorizontalSensitivity;
        public float GetControllerVeritcal => _settings.ControllerVerticalSensitivity;
        public bool GetIsInverted => _settings.IsInverted;
        
        public InputSettingsViewModel(IInputSettings settings)
        {
            _settings = settings;
            SetControlSettings();
        }

        public void RevertSettings()
        {
            _settings.RevertAllSettings();
            SetControlSettings();
        }

        public void SetMouseLock(bool isLocked)
        {
            if (isLocked)
            {
                LockMouseUtility.Instance.LockMouse();
            }
            else
            {
                LockMouseUtility.Instance.UnlockMouse();
            }
        }

        public void ApplySettings(float horizontalMouse, float verticalMouse, float horizontalController, float verticalController, bool isInverted)
        {
            _settings.MouseHorizontalSensitivity = horizontalMouse;
            _settings.MouseVerticalSensitivity = verticalMouse;
            _settings.ControllerHorizontalSensitivity = horizontalController;
            _settings.ControllerVerticalSensitivity = verticalController;
            _settings.IsInverted = isInverted;

            SetControlSettings();
        }

        public void SetControlSettings()
        {
            _perigonAnalytics.SetControlSettings(
                GetMouseHorizontal,
                GetMouseVertical,
                GetControllerHorizontal,
                GetControllerVeritcal,
                GetIsInverted);
        }
    }
}
