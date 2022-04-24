using FMODUnity;
using Perigon.Weapons;
using UnityEngine;

namespace Tests
{
    public class MockWeaponProperties : IWeaponProperties
    {
        private float _rateOfFire;
        private float _bulletSpread;
        private float _bulletSpreadRate;
        private float _reloadDuration;
        private int _bulletsPerShot;
        private int _ammoAmount;

        public string NameOfWeapon => "Mock Weapon";
        public float RateOfFire => _rateOfFire;
        public Sprite Crosshair => null;
        public float BulletSpread => _bulletSpread;
        
        public float GetBulletSpreadRate(float timeSinceFiring)
        {
            return _bulletSpreadRate;
        }

        public float ReloadDuration => _reloadDuration;
        public int BulletsPerShot => _bulletsPerShot;
        public int AmmunitionAmount => _ammoAmount;
        public bool IsRayCastingWeapon => true;
        public float VisualRecoilForce => 0;
        public EventReference WeaponShotAudio => new EventReference();
        public BulletTypes TypeOfBullet => BulletTypes.NoPhysics;

        public MockWeaponProperties (
            float rateOfFire = 0.1f,
            float bulletSpread = 1f,
            int bulletsPerShot = 1,
            int ammoAmount = 20,
            float reloadDuration = 0.5f)
        {
            _rateOfFire = rateOfFire;
            _bulletSpread = bulletSpread;
            _bulletsPerShot = bulletsPerShot;
            _ammoAmount = ammoAmount;
            _reloadDuration = reloadDuration;
        }
    }
}
