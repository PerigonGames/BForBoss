namespace BForBoss
{
    public readonly struct EnergyData
    {
        public readonly float Value;
        public readonly float MaxEnergyValue;
        public readonly float RateOfTransaction;
        
        public EnergyData(
            float value,
            float maxEnergyValue,
            float rateOfTransaction
        )
        {
            Value = value;
            MaxEnergyValue = maxEnergyValue;
            RateOfTransaction = rateOfTransaction;
        }

        public EnergyData Apply(float value) => new EnergyData(value, MaxEnergyValue, RateOfTransaction);
    }
}
