namespace BForBoss
{
    public readonly struct EnergyData
    {
        public readonly float Value;
        public readonly float MaxEnergyValue;
        public readonly float RateOfAccruement;
        
        public EnergyData(
            float value,
            float maxEnergyValue,
            float rateOfAccruement
        )
        {
            Value = value;
            MaxEnergyValue = maxEnergyValue;
            RateOfAccruement = rateOfAccruement;
        }
    }
}
