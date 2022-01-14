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
        public int AmmunitionAmount => CurrentWeapon.AmmunitionAmount;
        public int MaxAmmunitionAmount => CurrentWeapon.MaxAmmunitionAmount;
        public string NameOfWeapon => CurrentWeapon.NameOfWeapon;

        public float MaxReloadDuration => CurrentWeapon.MaxReloadDuration;
        public float ElapsedReloadDuration => CurrentWeapon.ElapsedReloadDuration;

        public Weapon[] Weapons => _weapons;
        public Weapon CurrentWeapon => _weapons[_currentWeaponIndex];
    }
}
