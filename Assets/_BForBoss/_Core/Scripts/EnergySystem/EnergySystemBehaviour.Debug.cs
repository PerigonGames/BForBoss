#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace BForBoss
{
    public partial class EnergySystemBehaviour
    {
        private bool _useDebugEnergySystemConfig = false;
        public bool UseDebugEnergySystemConfig
        {
            get => _useDebugEnergySystemConfig;
            set
            {
                _useDebugEnergySystemConfig = value;
                _energySystemConfigurationData = value ? new EnergySystemConfigurationData() : _energySystemConfiguration.MapToData();
            }
        }

        public void SetMaxEnergy()
        {
            EnergyData = _energyData.Apply(_energyData.MaxEnergyValue);
        }
    }
}
#endif
