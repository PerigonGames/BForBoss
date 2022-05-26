using FMODUnity;

namespace Perigon.Weapons
{
    public interface IEquipmentData
    {
        int AmmunitionAmount { get; }
        int MaxAmmunitionAmount { get; }
        string NameOfWeapon { get; }
        float MaxReloadDuration { get; }
        float ElapsedReloadDuration { get; }
        EventReference WeaponShotAudio { get; }
        float CurrentMeleeCooldown { get; }
        float MaxMeleeCooldown { get; }
        bool CanMelee { get; }
    }

    public partial class EquipmentBehaviour: IEquipmentData
    {
        public int AmmunitionAmount => CurrentWeapon.AmmunitionAmount;
        public int MaxAmmunitionAmount => CurrentWeapon.MaxAmmunitionAmount;
        public string NameOfWeapon => CurrentWeapon.NameOfWeapon;

        public float MaxReloadDuration => CurrentWeapon.MaxReloadDuration;
        public float ElapsedReloadDuration => CurrentWeapon.ElapsedReloadDuration;
        public EventReference WeaponShotAudio => CurrentWeapon.ShotAudio;

        public Weapon[] Weapons => _weapons;
        public Weapon CurrentWeapon => _weapons[_currentWeaponIndex];

        public float CurrentMeleeCooldown => _meleeBehaviour != null ? _meleeBehaviour.CurrentCooldown : 0f;
        public float MaxMeleeCooldown => _meleeBehaviour != null ? _meleeBehaviour.MaxCooldown : 1f;
        public bool CanMelee => _meleeBehaviour != null && _meleeBehaviour.CanMelee;
    }
}
