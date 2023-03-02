using System;
using System.Dynamic;
using PerigonGames;
using UnityEngine;

namespace Perigon.Weapons
{
    public class Weapon
    {
        private const float MIN_TO_MAX_RANGE_OF_SPREAD = 2;
        private const float MAP_TO_RAYCAST_RANGE_SPREAD = 0.1F;
        private readonly IRandomUtility _randomUtility;
        private readonly int _ammunitionAmount;
        private readonly float _rateOfFire;
        private readonly float _reloadDuration;
        private readonly float _bulletSpread;
        

        private float _elapsedRateOfFire;
        private float _elapsedReloadDuration;
        private int _elapsedAmmunitionAmount;
        private bool _isReloading = false;

        public bool IsReloading
        {
            get => _isReloading;
            set
            {
                _isReloading = value;
                OnReloadingStateUpdate?.Invoke(value);
            }
        }

        public event Action OnFireWeapon;
        public event Action<bool> OnReloadingStateUpdate;

        private bool CanShoot => _elapsedRateOfFire <= 0 && _elapsedAmmunitionAmount > 0;

        public Weapon(
            int ammunitionAmount, 
            float rateOfFire, 
            float reloadDuration, 
            float bulletSpread, 
            IRandomUtility randomUtility = null)
        {
            _randomUtility = randomUtility ?? new RandomUtility();
            _elapsedAmmunitionAmount = _ammunitionAmount = ammunitionAmount;
            _elapsedReloadDuration = _reloadDuration = reloadDuration;
            _elapsedRateOfFire = _rateOfFire = rateOfFire;
            _bulletSpread = bulletSpread;
        }

        public void DecrementElapsedTimeRateOfFire(float deltaTime, float timeScale)
        {
            var scaledDeltaTime = ScaledDeltaTime(deltaTime, timeScale);
            _elapsedRateOfFire = Mathf.Clamp(_elapsedRateOfFire - scaledDeltaTime, 0, float.PositiveInfinity);
        }

        public float ScaledDeltaTime(float deltaTime, float timeScale)
        {
            var clampedTimeScale = Mathf.Clamp(timeScale, 0.01f, float.MaxValue);
            return 1 / clampedTimeScale * deltaTime;
        }

        public void ReloadWeaponCountDownIfNeeded(float deltaTime, float timeScale)
        {
            if (_elapsedAmmunitionAmount <= 0)
            {
                IsReloading = true;
            }

            if (IsReloading)
            {
                _elapsedReloadDuration -= ScaledDeltaTime(deltaTime, timeScale);
            }

            if (_elapsedReloadDuration <= 0)
            {
                ResetWeaponState();
            }
        }

        public void ReloadWeaponIfPossible()
        {
            if (_elapsedAmmunitionAmount < _ammunitionAmount)
            {
                IsReloading = true;
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
            _elapsedAmmunitionAmount--;
            _elapsedRateOfFire = _rateOfFire;
            OnFireWeapon?.Invoke();
            return true;
        }

        private void ResetWeaponState()
        {
            StopReloading();
            _elapsedAmmunitionAmount = _ammunitionAmount;
        }

        private void StopReloading()
        {
            IsReloading = false;
            _elapsedReloadDuration = _reloadDuration;
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