using System;
using PerigonGames;
using UnityEngine;

namespace Perigon.Weapons
{
    public readonly struct WeaponData
    {
        public readonly float ElapsedRateOfFire;
        public readonly float ElapsedReloadDuration;
        public readonly int ElapsedAmmunitionAmount;
        public readonly bool IsReloading;

        private WeaponData(
            float elapsedRateOfFire, 
            float elapsedReloadDuration, 
            int elapsedAmmunitionAmount,
            bool isReloading)
        {
            ElapsedRateOfFire = elapsedRateOfFire;
            ElapsedReloadDuration = elapsedReloadDuration;
            ElapsedAmmunitionAmount = elapsedAmmunitionAmount;
            IsReloading = isReloading;
        }

        public WeaponData Apply(
            float? elapsedRateOfFire = null,
            float? elapsedReloadDuration = null,
            int? elapsedAmmunitionAmount = null,
            bool? isReloading = null)
        {
            return new WeaponData(
                elapsedRateOfFire: elapsedRateOfFire ?? ElapsedRateOfFire,
                elapsedReloadDuration: elapsedReloadDuration ?? ElapsedReloadDuration,
                elapsedAmmunitionAmount: elapsedAmmunitionAmount ?? ElapsedAmmunitionAmount,
                isReloading: isReloading ?? IsReloading
            );
        }
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

        private WeaponData _weaponData;
        public WeaponData WeaponData
        {
            private set
            {
                _weaponData = value;
                OnWeaponDataStateChange?.Invoke(value);
                Debug.Log("Elapsed Rate Of Fire: " + value.ElapsedRateOfFire);
            }
            get => _weaponData;
        }

        public event Action<WeaponData> OnWeaponDataStateChange;
        public event Action OnFireWeapon;

        private bool CanShoot => _weaponData.ElapsedRateOfFire <= 0 && _weaponData.ElapsedAmmunitionAmount > 0;

        public Weapon(
            int ammunitionAmount, 
            float rateOfFire, 
            float reloadDuration, 
            float bulletSpread, 
            IRandomUtility randomUtility = null)
        {
            _randomUtility = randomUtility ?? new RandomUtility();
            WeaponData = _weaponData.Apply(rateOfFire, reloadDuration, ammunitionAmount);
            _ammunitionAmount = ammunitionAmount;
            _reloadDuration = reloadDuration;
            _rateOfFire = rateOfFire;
            _bulletSpread = bulletSpread;
        }

        public void DecrementElapsedTimeRateOfFire(float deltaTime, float timeScale)
        {
            Debug.Log("Decrement Elapsed Time");
            var scaledDeltaTime = ScaledDeltaTime(deltaTime, timeScale);
            WeaponData = WeaponData.Apply(elapsedRateOfFire: Mathf.Clamp(
                WeaponData.ElapsedRateOfFire - scaledDeltaTime,
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
            if (WeaponData.ElapsedAmmunitionAmount <= 0)
            {
                WeaponData = WeaponData.Apply(isReloading: true);
            }

            if (WeaponData.IsReloading)
            {
                var reloadDuration = _weaponData.ElapsedReloadDuration - ScaledDeltaTime(deltaTime, timeScale);
                WeaponData = WeaponData.Apply(elapsedReloadDuration: reloadDuration);
            }

            if (_weaponData.ElapsedReloadDuration <= 0)
            {
                ResetWeaponState();
            }
        }

        public void ReloadWeaponIfPossible()
        {
            if (WeaponData.ElapsedAmmunitionAmount < _ammunitionAmount)
            {
                WeaponData = WeaponData.Apply(isReloading: true);
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
            WeaponData = WeaponData.Apply(
                elapsedAmmunitionAmount: WeaponData.ElapsedAmmunitionAmount - 1,
                elapsedRateOfFire: _rateOfFire);
            OnFireWeapon?.Invoke();
            return true;
        }

        private void ResetWeaponState()
        {
            StopReloading();
            WeaponData = WeaponData.Apply(elapsedAmmunitionAmount: _ammunitionAmount);
        }

        private void StopReloading()
        {
            WeaponData = WeaponData.Apply(
                isReloading: false,
                elapsedReloadDuration: _reloadDuration);
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