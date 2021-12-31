using Perigon.Analytics;
using Perigon.Character;
using Perigon.Utility;
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
            _viewModel.SetGraphy();
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
            _showFPSToggle.isOn = _viewModel.IsShowingFPS;
            _showPCSpecsToggle.isOn = _viewModel.IsShowingPCSpecs;
            _showRAMToggle.isOn = _viewModel.IsShowingRAM;
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
        private readonly PerigonAnalytics _perigonAnalytics = PerigonAnalytics.Instance;
        
        public bool IsShowingFPS => PlayerPrefs.GetInt(PlayerPrefKeys.GameplaySettings.SHOW_FPS, 0) == 1;
        public bool IsShowingRAM => PlayerPrefs.GetInt(PlayerPrefKeys.GameplaySettings.SHOW_RAM_USAGE, 0) == 1;
        public bool IsShowingPCSpecs => PlayerPrefs.GetInt(PlayerPrefKeys.GameplaySettings.SHOW_PC_SPECS, 0) == 1;
        public bool IsThirdPersonView => _thirdPersonSettings.IsThirdPerson;

        public GameplaySettingsViewModel(IThirdPerson thirdPersonSettings, ISpecification specification = null)
        {
            _thirdPersonSettings = thirdPersonSettings;
            _graphyManager = specification ?? new GraphyAdapter();
            
            _perigonAnalytics.SetPOV(IsThirdPersonView);
        }

        public void SetGraphy()
        {
            SetShowFPS(IsShowingFPS);
            SetShowRAMUsage(IsShowingRAM);
            SetShowPCSpecifications(IsShowingPCSpecs);
            _graphyManager.SetAudioUsage(false);
        }

        public void SetShowFPS(bool isOn)
        {
            var storedValue = isOn ? 1 : 0;
            PlayerPrefs.SetInt(PlayerPrefKeys.GameplaySettings.SHOW_FPS, storedValue);
            _graphyManager.SetShowFPSActive(isOn);
        }

        public void SetShowRAMUsage(bool isOn)
        {
            var storedValue = isOn ? 1 : 0;
            PlayerPrefs.SetInt(PlayerPrefKeys.GameplaySettings.SHOW_RAM_USAGE, storedValue);
            _graphyManager.SetShowRAMUsageActive(isOn);
        }

        public void SetShowPCSpecifications(bool isOn)
        {
            var storedValue = isOn ? 1 : 0;
            PlayerPrefs.SetInt(PlayerPrefKeys.GameplaySettings.SHOW_PC_SPECS, storedValue);
            _graphyManager.SetShowPCSpecificationsActive(isOn);
        }

        public void SetPOV(int dropDownValue)
        {
            var isThirdPerson = DropDownToIsThirdPerson(dropDownValue);
            _thirdPersonSettings.SetThirdPersonActive(isThirdPerson);
            
            _perigonAnalytics.SetPOV(isThirdPerson);
        }

        private bool DropDownToIsThirdPerson(int value)
        {
            return value != 0;
        }
    }
}