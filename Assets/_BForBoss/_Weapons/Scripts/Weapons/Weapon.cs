using System;
using FMODUnity;
using PerigonGames;
using UnityEngine;

namespace Perigon.Weapons
{
    public class Weapon
    {
        private const float MIN_TO_MAX_RANGE_OF_SPREAD = 2;
        private const float MAP_TO_RAYCAST_RANGE_SPREAD = 0.1F;
        private readonly IRandomUtility _randomUtility;
        private readonly IWeaponProperties _weaponProperties;

         private float _elapsedRateOfFire;
         private float _elapsedReloadDuration;
         private int _ammunitionAmount;

         public bool IsReloading { get; set; }

         public bool ActivateWeapon
         {
             set
             {
                 StopReloading();
                 OnSetWeaponActivate?.Invoke(value);
             }
         }

         public event Action<int> OnFireWeapon;
         public event Action<bool> OnSetWeaponActivate;
         public event Action OnStopReloading;

         public int AmmunitionAmount => _ammunitionAmount;
         public int MaxAmmunitionAmount => _weaponProperties.AmmunitionAmount;
         public float MaxReloadDuration => _weaponProperties.ReloadDuration;
         public float ElapsedReloadDuration => _elapsedReloadDuration;
         public string NameOfWeapon => _weaponProperties.NameOfWeapon;
         public bool IsRayCastingWeapon => _weaponProperties.IsRayCastingWeapon;
         public float DamagePerRayCast => IsRayCastingWeapon ? _weaponProperties.DamagePerRayCast : 0;
         public BulletTypes TypeOfBullet => _weaponProperties.TypeOfBullet;
         public Sprite Crosshair => _weaponProperties.Crosshair;
         public WeaponAnimationType AnimationType => _weaponProperties.AnimationType;
         public EventReference ShotAudio => _weaponProperties.WeaponShotAudio;
         private bool CanShoot => _elapsedRateOfFire <= 0 && _ammunitionAmount > 0;

         public Weapon(IWeaponProperties weaponProperties, IRandomUtility randomUtility = null)
         {
             _weaponProperties = weaponProperties;
             _randomUtility = randomUtility ?? new RandomUtility();
             _elapsedReloadDuration = weaponProperties.ReloadDuration;
             _ammunitionAmount = weaponProperties.AmmunitionAmount;
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
             if (_ammunitionAmount <= 0)
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
             if (_ammunitionAmount < _weaponProperties.AmmunitionAmount)
             {
                 IsReloading = true;
             }
         }

         public Vector3 GetShootDirection(Vector3 from, Vector3 to, float timeSinceFiring)
         {
             var directionWithoutSpread = to - from;
             var spread = GenerateSpread(timeSinceFiring);
             var directionWithSpread = GenerateSpreadAngle(spread) * directionWithoutSpread;
             return directionWithSpread.normalized;
         }

         public Vector3 GetShootDirection(float timeSinceFiring)
         {
             var bulletSpread = GenerateSpread(timeSinceFiring);
             var xPosition = RandomDoubleIncludingNegative() * MAP_TO_RAYCAST_RANGE_SPREAD * bulletSpread;
             var yPosition = RandomDoubleIncludingNegative() * MAP_TO_RAYCAST_RANGE_SPREAD * bulletSpread;
             return new Vector3(xPosition, yPosition, 1);
         }

         public void FireIfPossible()
         {
             if (!CanShoot)
             {
                 return;
             }

             StopReloading();
             _ammunitionAmount--;
             ResetRateOfFire();
             Fire();
         }

         private void ResetWeaponState()
         {
             StopReloading();
             _ammunitionAmount = _weaponProperties.AmmunitionAmount;
         }

         private void StopReloading()
         {
             IsReloading = false;
             _elapsedReloadDuration = _weaponProperties.ReloadDuration;
             OnStopReloading?.Invoke();
         }

         private void Fire()
         {
             OnFireWeapon?.Invoke(_weaponProperties.BulletsPerShot);
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

         private float GenerateSpread(float timeSinceFiring)
         {
             float spreadRate = _weaponProperties.GetBulletSpreadRate(timeSinceFiring);
             return _weaponProperties.BulletSpread * spreadRate;
         }

         private float RandomDoubleIncludingNegative()
         {
             return (float) _randomUtility.NextDouble() * (_randomUtility.CoinFlip() ? 1 : -1);
         }

         private void ResetRateOfFire()
         {
             _elapsedRateOfFire = _weaponProperties.RateOfFire;
         }
    }
}
