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
        public int AmmunitionAmount => 0;
        public int MaxAmmunitionAmount => 0;
        public string NameOfWeapon => "";

        public float MaxReloadDuration => 1;
        public float ElapsedReloadDuration => 1;

        private WeaponBehaviour CurrentWeapon => _weaponBehaviours[_currentWeaponIndex];
    }
}
