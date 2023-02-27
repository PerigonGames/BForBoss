namespace BForBoss
{
    public enum EnergyAccruementType
    {
        WallRun,
        Dash,
        DoubleJump,
        Slide
    }

    public enum EnergyExpenseType
    {
        Shot,
        SlowMo
    }
    
    public readonly struct EnergySystemConfigurationData
    {
        //Accruements
        public readonly float WallRunEnergy;
        public readonly float DashEnergy;
        public readonly float DoubleJumpEnergy;
        public readonly float SlideEnergy;
        
        //Expenses
        public readonly float ShotEnergy;
        public readonly float SlowMoEnergy;

        public EnergySystemConfigurationData(
            float wallRunEnergy,
            float dashEnergy,
            float doubleJumpEnergy,
            float slideEnergy,
            float shotEnergy,
            float slowMoEnergy
        )
        {
            WallRunEnergy = wallRunEnergy;
            DashEnergy = dashEnergy;
            DoubleJumpEnergy = doubleJumpEnergy;
            SlideEnergy = slideEnergy;

            ShotEnergy = shotEnergy;
            SlowMoEnergy = slowMoEnergy;
        }
    }
}
