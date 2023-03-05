namespace Perigon.Weapons
{
    public readonly struct WeaponData
    {
        public readonly float ElapsedRateOfFire;
        public readonly float ElapsedReloadDuration;
        public readonly int ElapsedAmmunitionAmount;
        public readonly bool IsReloading;

        private WeaponData(
            float elapsedRateOfFire, 
            float elapsedReloadDuration, 
            int elapsedAmmunitionAmount,
            bool isReloading)
        {
            ElapsedRateOfFire = elapsedRateOfFire;
            ElapsedReloadDuration = elapsedReloadDuration;
            ElapsedAmmunitionAmount = elapsedAmmunitionAmount;
            IsReloading = isReloading;
        }

        public WeaponData Apply(
            float? elapsedRateOfFire = null,
            float? elapsedReloadDuration = null,
            int? elapsedAmmunitionAmount = null,
            bool? isReloading = null)
        {
            return new WeaponData(
                elapsedRateOfFire: elapsedRateOfFire ?? ElapsedRateOfFire,
                elapsedReloadDuration: elapsedReloadDuration ?? ElapsedReloadDuration,
                elapsedAmmunitionAmount: elapsedAmmunitionAmount ?? ElapsedAmmunitionAmount,
                isReloading: isReloading ?? IsReloading
            );
        }
    }
}
