using System;
using PerigonGames;
using UnityEngine;

namespace Perigon.Weapons
{
    public enum WeaponEffect
    {
        FireWeapon,
        StartReloading,
        StopReloading
    }
    
    public class Weapon
    {
        private const float MIN_TO_MAX_RANGE_OF_SPREAD = 2;
        private const float MAP_TO_RAYCAST_RANGE_SPREAD = 0.1F;
        private readonly IRandomUtility _randomUtility;
        private readonly int _ammunitionAmount;
        private readonly float _rateOfFire;
        private readonly float _reloadDuration;
        private readonly float _bulletSpread;

        private WeaponData _data;
        public WeaponData Data
        {
            private set
            {
                _data = value;
                OnWeaponDataStateChange?.Invoke(value);
            }
            get => _data;
        }

        public event Action<WeaponData> OnWeaponDataStateChange;
        public event Action<WeaponEffect> OnWeaponEffectEmit;

        private bool CanShoot => _data.ElapsedRateOfFire <= 0 && _data.ElapsedAmmunitionAmount > 0;

        public Weapon(
            int ammunitionAmount, 
            float rateOfFire, 
            float reloadDuration, 
            float bulletSpread, 
            IRandomUtility randomUtility = null)
        {
            _randomUtility = randomUtility ?? new RandomUtility();
            Data = _data.Apply(rateOfFire, reloadDuration, ammunitionAmount);
            _ammunitionAmount = ammunitionAmount;
            _reloadDuration = reloadDuration;
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

        public void ReloadWeaponCountDownIfNeeded(float deltaTime, float timeScale)
        {
            if (Data.ElapsedAmmunitionAmount <= 0)
            {
                Data = Data.Apply(isReloading: true);
                OnWeaponEffectEmit?.Invoke(WeaponEffect.StartReloading);
            }

            if (Data.IsReloading)
            {
                var reloadDuration = _data.ElapsedReloadDuration - ScaledDeltaTime(deltaTime, timeScale);
                Data = Data.Apply(elapsedReloadDuration: reloadDuration);
            }

            if (_data.ElapsedReloadDuration <= 0)
            {
                ResetWeaponState();
            }
        }

        public void ReloadWeaponIfPossible()
        {
            if (Data.ElapsedAmmunitionAmount < _ammunitionAmount)
            {
                Data = Data.Apply(isReloading: true);
                OnWeaponEffectEmit?.Invoke(WeaponEffect.StartReloading);
            }
        }

        public Vector3 GetShootDirection(Vector3 from, Vector3 to, float bulletSpreadRate)
        {
            var directionWithoutSpread = to - from;
            var spread = GenerateSpread(bulletSpreadRate);
            var directionWithSpread = GenerateSpreadAngle(spread) * directionWithoutSpread;
            return directionWithSpread.normalized;
        }

        public Vector3 GetShootDirection(float bulletSpreadRate)
        {
            var bulletSpread = GenerateSpread(bulletSpreadRate);
            var xPosition = RandomDoubleIncludingNegative() * MAP_TO_RAYCAST_RANGE_SPREAD * bulletSpread;
            var yPosition = RandomDoubleIncludingNegative() * MAP_TO_RAYCAST_RANGE_SPREAD * bulletSpread;
            return new Vector3(xPosition, yPosition, 1);
        }

        public bool TryFire()
        {
            if (!CanShoot)
            {
                return false;
            }

            StopReloading();
            Data = Data.Apply(
                elapsedAmmunitionAmount: Data.ElapsedAmmunitionAmount - 1,
                elapsedRateOfFire: _rateOfFire);
            OnWeaponEffectEmit?.Invoke(WeaponEffect.FireWeapon);
            return true;
        }

        private void ResetWeaponState()
        {
            StopReloading();
            Data = Data.Apply(elapsedAmmunitionAmount: _ammunitionAmount);
        }

        private void StopReloading()
        {
            Data = Data.Apply(
                isReloading: false,
                elapsedReloadDuration: _reloadDuration);
            OnWeaponEffectEmit?.Invoke(WeaponEffect.StopReloading);
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