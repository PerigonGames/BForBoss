using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public readonly struct WeaponConfigurationData
    {
        public readonly string NameOfWeapon;
        public readonly float RateOfFire;
        public readonly float BulletSpread;
        public readonly float ReloadDuration;
        public readonly int BulletsPerShot;
        public readonly int AmmunitionAmount;
        public readonly bool IsRayCastingWeapon;
        public readonly float DamagePerRayCast;
        public readonly BulletTypes BulletType;
        public readonly WeaponAnimationType AnimationType;
        public readonly EventReference WeaponShotAudio;
        public readonly EventReference WeaponReloadAudio;

        private readonly AnimationCurve _bulletSpreadRateCurve;

        public WeaponConfigurationData(
            string nameOfWeapon,
            float rateOfFire,
            float bulletSpread,
            float reloadDuration,
            int bulletsPerShot,
            int ammunitionAmount,
            bool isRayCastingWeapon,
            float damagePerRayCast,
            BulletTypes bulletType,
            WeaponAnimationType animationType,
            EventReference weaponShotAudio,
            EventReference weaponReloadAudio,
            AnimationCurve bulletSpreadRateCurve)
        {
            NameOfWeapon = nameOfWeapon;
            RateOfFire = rateOfFire;
            BulletSpread = bulletSpread;
            ReloadDuration = reloadDuration;
            BulletsPerShot = bulletsPerShot;
            AmmunitionAmount = ammunitionAmount;
            IsRayCastingWeapon = isRayCastingWeapon;
            DamagePerRayCast = damagePerRayCast;
            BulletType = bulletType;
            AnimationType = animationType;
            WeaponShotAudio = weaponShotAudio;
            WeaponReloadAudio = weaponReloadAudio;
            _bulletSpreadRateCurve = bulletSpreadRateCurve;
        }
        public float GetBulletSpreadRate(float timeSinceFiring)
        {
            float clampedTimeSinceFiring = Mathf.Clamp(timeSinceFiring, 0, 1);
            return _bulletSpreadRateCurve.Evaluate(clampedTimeSinceFiring);
        }
    }
    
    [CreateAssetMenu(fileName = "Weapon", menuName = "PerigonGames/Weapon", order = 2)]
    public class WeaponConfigurationSO : ScriptableObject
    {
        [SerializeField] private string _nameOfWeapon = "";
        [SerializeField] private float _rateOfFire = 0.1f;
        [SerializeField] private float _bulletSpread = 1f;
        [SerializeField] private AnimationCurve _bulletSpreadRateCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] 
        [Range(0.01f, 10f)] private float _reloadDuration = 0.5f;
        [SerializeField] private int _bulletsPerShot = 1;
        [SerializeField] 
        [Range(1, 1000)] private int _ammunitionAmount = 20;
        [Title("Weapon Shoot Style")]
        [SerializeField] private bool _isRaycastWeapon = true;

        [ShowIf("_isRaycastWeapon")] 
        [SerializeField] private float _damagePerRayCast = 1;
        [HideIf("_isRaycastWeapon")]
        [SerializeField] private BulletTypes _typeOfBullet = BulletTypes.NoPhysics;

        [Title("Effects")] 
        [SerializeField] private WeaponAnimationType _animationType = WeaponAnimationType.Rifle;
        [SerializeField] private EventReference _weaponShotAudio = new EventReference();
        [SerializeField] private EventReference _weaponReloadAudio = new EventReference();

        public WeaponConfigurationData MapToData()
        {
            return new WeaponConfigurationData
            (
                nameOfWeapon: _nameOfWeapon,
                rateOfFire: _rateOfFire,
                bulletSpread: _bulletSpread,
                reloadDuration: _reloadDuration,
                bulletsPerShot: _bulletsPerShot,
                ammunitionAmount: _ammunitionAmount,
                isRayCastingWeapon: _isRaycastWeapon,
                damagePerRayCast: _damagePerRayCast,
                bulletType: _typeOfBullet,
                animationType: _animationType,
                weaponShotAudio: _weaponShotAudio,
                weaponReloadAudio: _weaponReloadAudio,
                bulletSpreadRateCurve: _bulletSpreadRateCurve
            );
        }
    }
}
