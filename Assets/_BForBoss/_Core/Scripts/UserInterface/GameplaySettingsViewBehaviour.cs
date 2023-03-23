using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class GameplaySettingsViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Toggle _showFPSToggle = null;
        [SerializeField] private Toggle _showRAMToggle = null;
        [SerializeField] private Toggle _showPCSpecsToggle = null;

        private GameplaySettingsViewModel _viewModel = null;
        
        public void Initialize()
        {
            _viewModel = new GameplaySettingsViewModel();
            SetViews();
            BindModel();
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            ShowSettingsForDevelopment();
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
        }

        private void SetViews()
        {
            _showFPSToggle.isOn = true;
            _showPCSpecsToggle.isOn = false;
            _showRAMToggle.isOn = false;
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private void ShowSettingsForDevelopment()
        {
            _showPCSpecsToggle.gameObject.SetActive(true);
            _showRAMToggle.gameObject.SetActive(true);
        }
#endif
    }
    
    public class GameplaySettingsViewModel
    {
        private ISpecification _graphyManager = null;
        
        public GameplaySettingsViewModel(ISpecification specification = null)
        {
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
    }
}