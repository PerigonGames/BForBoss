#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace BForBoss
{
    public partial class EnergySystemBehaviour
    {
        public bool UseDebugEnergyCost = false;

        public void SetMaxEnergy()
        {
            EnergyData = _energyData.Apply(_energyData.MaxEnergyValue);
        }
    }
}
#endif
