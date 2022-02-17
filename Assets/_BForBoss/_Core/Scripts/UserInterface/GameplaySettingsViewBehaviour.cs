using Perigon.Analytics;
using Perigon.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class GameplaySettingsViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _povDropdown = null;
        [SerializeField] private Toggle _showFPSToggle = null;
        [SerializeField] private Toggle _showRAMToggle = null;
        [SerializeField] private Toggle _showPCSpecsToggle = null;

        private GameplaySettingsViewModel _viewModel = null;
        
        public void Initialize(IThirdPerson thirdPersonSettings)
        {
            _viewModel = new GameplaySettingsViewModel(thirdPersonSettings);
            SetViews();
            BindModel();
#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
            HideSettingsForProduction();
#endif
        }

        private void BindModel()
        {
            _showFPSToggle.onValueChanged.RemoveAllListeners();
            _showFPSToggle.onValueChanged.AddListener(isOn =>
            {
                _viewModel.SetShowFPS(isOn);
            });
            
            _showRAMToggle.onValueChanged.RemoveAllListeners();
            _showRAMToggle.onValueChanged.AddListener(isOn =>
            {
                _viewModel.SetShowRAMUsage(isOn);
            });
            
            _showPCSpecsToggle.onValueChanged.RemoveAllListeners();
            _showPCSpecsToggle.onValueChanged.AddListener(isOn =>
            {
                _viewModel.SetShowPCSpecifications(isOn);
            });
            
            _povDropdown.onValueChanged.RemoveAllListeners();
            _povDropdown.onValueChanged.AddListener(value =>
            {
                _viewModel.SetPOV(value);
            });
        }

        private void SetViews()
        {
            _showFPSToggle.isOn = true;
            _showPCSpecsToggle.isOn = false;
            _showRAMToggle.isOn = false;
            _povDropdown.value = _viewModel.IsThirdPersonView ? 1 : 0;
        }

        private void HideSettingsForProduction()
        {
            _viewModel.SetShowPCSpecifications(false);
            _viewModel.SetShowRAMUsage(false);
            _showPCSpecsToggle.gameObject.SetActive(false);
            _showRAMToggle.gameObject.SetActive(false);
        }
    }
    
    public class GameplaySettingsViewModel
    {
        private ISpecification _graphyManager = null;
        private IThirdPerson _thirdPersonSettings = null;
        
        public bool IsThirdPersonView => _thirdPersonSettings.IsThirdPerson;

        public GameplaySettingsViewModel(IThirdPerson thirdPersonSettings, ISpecification specification = null)
        {
            _thirdPersonSettings = thirdPersonSettings;
            _graphyManager = specification ?? new GraphyAdapter();
        }

        public void SetShowFPS(bool isOn)
        {
            _graphyManager.SetShowFPSActive(isOn);
        }

        public void SetShowRAMUsage(bool isOn)
        {
            _graphyManager.SetShowRAMUsageActive(isOn);
        }

        public void SetShowPCSpecifications(bool isOn)
        {
            _graphyManager.SetShowPCSpecificationsActive(isOn);
        }

        public void SetPOV(int dropDownValue)
        {
            var isThirdPerson = DropDownToIsThirdPerson(dropDownValue);
            _thirdPersonSettings.SetThirdPersonActive(isThirdPerson);
        }

        private bool DropDownToIsThirdPerson(int value)
        {
            return value != 0;
        }
    }
}