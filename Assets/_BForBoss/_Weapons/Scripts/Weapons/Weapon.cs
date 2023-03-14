using System;
using PerigonGames;
using UnityEngine;

namespace Perigon.Weapons
{
    public enum WeaponEffect
    {
        FireWeapon
    }
    
    public class Weapon
    {
        private const float MIN_TO_MAX_RANGE_OF_SPREAD = 2;
        private const float MAP_TO_RAYCAST_RANGE_SPREAD = 0.1F;
        private readonly IRandomUtility _randomUtility;
        private readonly float _rateOfFire;
        private readonly float _bulletSpread;

        private WeaponData _data;
        private WeaponData Data
        {
            set => _data = value;
            get => _data;
        }

        public event Action<WeaponEffect> OnWeaponEffectEmit;

        private bool CanShoot => _data.ElapsedRateOfFire <= 0;

        public Weapon(
            float rateOfFire, 
            float bulletSpread, 
            IRandomUtility randomUtility = null)
        {
            _randomUtility = randomUtility ?? new RandomUtility();
            Data = _data.Apply(rateOfFire);
            _rateOfFire = rateOfFire;
            _bulletSpread = bulletSpread;
        }

        public void DecrementElapsedTimeRateOfFire(float deltaTime, float timeScale)
        {
            var scaledDeltaTime = ScaledDeltaTime(deltaTime, timeScale);
            Data = Data.Apply(elapsedRateOfFire: Mathf.Clamp(
                Data.ElapsedRateOfFire - scaledDeltaTime,
                0,
                float.PositiveInfinity));
        }

        public float ScaledDeltaTime(float deltaTime, float timeScale)
        {
            var clampedTimeScale = Mathf.Clamp(timeScale, 0.01f, float.MaxValue);
            return 1 / clampedTimeScale * deltaTime;
        }

        public Vector3 GetShootDirection(Vector3 from, Vector3 to, float bulletSpreadRate)
        {
            var directionWithoutSpread = to - from;
            var spread = GenerateSpread(bulletSpreadRate);
            var directionWithSpread = GenerateSpreadAngle(spread) * directionWithoutSpread;
            return directionWithSpread.normalized;
        }

        public Vector3 GetSpreadDirection(float bulletSpreadRate)
        {
            var bulletSpread = GenerateSpread(bulletSpreadRate);
            var xPosition = RandomDoubleIncludingNegative() * MAP_TO_RAYCAST_RANGE_SPREAD * bulletSpread;
            var yPosition = RandomDoubleIncludingNegative() * MAP_TO_RAYCAST_RANGE_SPREAD * bulletSpread;
            return new Vector3(xPosition, yPosition, 1);
        }

        public bool TryFire()
        {
            if (CanShoot)
            {
                Data = Data.Apply(elapsedRateOfFire: _rateOfFire);
                OnWeaponEffectEmit?.Invoke(WeaponEffect.FireWeapon);
                return true;
            }

            return false;
        }

        private Quaternion GenerateSpreadAngle(float spread)
        {
            var spreadRange = spread * MIN_TO_MAX_RANGE_OF_SPREAD;
            var randomizedSpread = -spread + (float)_randomUtility.NextDouble() * spreadRange;
            var randomizedDirection = new Vector3(
                RandomDoubleIncludingNegative(),
                RandomDoubleIncludingNegative(),
                RandomDoubleIncludingNegative());
            return Quaternion.AngleAxis(randomizedSpread, randomizedDirection);
        }

        private float GenerateSpread(float bulletSpreadRate)
        {
            return _bulletSpread * bulletSpreadRate;
        }

        private float RandomDoubleIncludingNegative()
        {
            return (float)_randomUtility.NextDouble() * (_randomUtility.CoinFlip() ? 1 : -1);
        }
    }
}