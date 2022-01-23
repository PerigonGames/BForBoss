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
        int IEquipmentData.AmmunitionAmount => CurrentWeapon.AmmunitionAmount;
        int IEquipmentData.MaxAmmunitionAmount => CurrentWeapon.MaxAmmunitionAmount;
        string IEquipmentData.NameOfWeapon => CurrentWeapon.NameOfWeapon;

        float IEquipmentData.MaxReloadDuration => CurrentWeapon.MaxReloadDuration;
        float IEquipmentData.ElapsedReloadDuration => CurrentWeapon.ElapsedReloadDuration;

        public Weapon[] Weapons => _weapons;
        public Weapon CurrentWeapon => _weapons[_currentWeaponIndex];
    }
}
