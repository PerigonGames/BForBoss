namespace Perigon.Weapons
{
    public interface IEquipmentData
    {
        int AmmunitionAmount { get; }
        int MaxAmmunitionAmount { get; }
        string NameOfWeapon { get; }
        float MaxReloadDuration { get; }
        float ElapsedReloadDuration { get; }
    }

    public partial class EquipmentBehaviour: IEquipmentData
    {
        private Weapon _currentWeapon => _weapons[_currentWeaponIndex];
        public int AmmunitionAmount => _currentWeapon.AmmunitionAmount;
        public int MaxAmmunitionAmount => _currentWeapon.MaxAmmunitionAmount;
        public string NameOfWeapon => _currentWeapon.NameOfWeapon;

        public float MaxReloadDuration => _currentWeapon.MaxReloadDuration;
        public float ElapsedReloadDuration => _currentWeapon.ElapsedReloadDuration;
    }
}
