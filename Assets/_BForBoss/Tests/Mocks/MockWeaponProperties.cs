using Perigon.Weapons;
using UnityEngine;

namespace Tests
{
    public class MockWeaponProperties : IWeaponProperties
    {
        private float _rateOfFire;
        private float _bulletSpread;
        private int _numberOfBullets;
        
        public float RateOfFire => _rateOfFire;
        public Sprite Crosshair => null;
        public float BulletSpread => _bulletSpread;
        public int NumberOfBullets => _numberOfBullets;

        public BulletTypes TypeOfBullet => BulletTypes.NoPhysics;

        public MockWeaponProperties(float rateOfFire = 0.1f, float bulletSpread = 1f, int numberOfBullets = 1)
        {
            _rateOfFire = rateOfFire;
            _bulletSpread = bulletSpread;
            _numberOfBullets = numberOfBullets;
        }
    }
}
