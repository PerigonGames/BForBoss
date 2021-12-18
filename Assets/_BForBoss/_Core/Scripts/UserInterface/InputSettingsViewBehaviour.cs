using Perigon.UserInterface;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class InputSettingsViewBehaviour : MonoBehaviour
    {
        private BaseInputSettingsViewModel _viewModel = null;
        
        [Title("Settings")]
        [SerializeField] protected SliderBehaviour _horizontalSlider = null;
        [SerializeField] protected SliderBehaviour _verticalSlider = null;
        [SerializeField] protected Toggle _toggle = null;
        
        [Title("Button")] 
        [SerializeField] protected Button _applyButton = null;
        [SerializeField] protected Button _revertButton = null;
        
        public void Initialize(BaseInputSettingsViewModel viewModel)
        {
            _viewModel = viewModel;
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
                    _horizontalSlider.SliderValue, 
                    _verticalSlider.SliderValue,
                    _toggle.isOn);
                _applyButton.interactable = false;
            });
        }
        
        private void BindSliders()
        {
            _horizontalSlider.OnValueChangedAction = () =>
            {
                _applyButton.interactable = true;
            };
            
            _verticalSlider.OnValueChangedAction = () =>
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
            _horizontalSlider.SliderValue = _viewModel.GetHorizontal;
            _verticalSlider.SliderValue = _viewModel.GetVertical;
            _toggle.isOn = _viewModel.GetIsInverted;
            _applyButton.interactable = false;
        }
    }
}
