namespace Perigon.Weapons
{
    public readonly struct WeaponData
    {
        public readonly float ElapsedRateOfFire;

        private WeaponData(
            float elapsedRateOfFire)
        {
            ElapsedRateOfFire = elapsedRateOfFire;
        }

        public WeaponData Apply(
            float? elapsedRateOfFire = null)
        {
            return new WeaponData(
                elapsedRateOfFire: elapsedRateOfFire ?? ElapsedRateOfFire
            );
        }
    }
}
