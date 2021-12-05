using Perigon.Weapons;
using UnityEngine;

namespace Tests
{
    public class MockWeaponProperties : IWeaponProperties
    {
        private float _rateOfFire;
        private float _bulletSpread;
        
        public float RateOfFire => _rateOfFire;
        public Sprite Crosshair => null;
        public float BulletSpread => _bulletSpread;

        public MockWeaponProperties(float rateOfFire = 0.1f, float bulletSpread = 1f)
        {
            _rateOfFire = rateOfFire;
            _bulletSpread = bulletSpread;
        }
    }
}
