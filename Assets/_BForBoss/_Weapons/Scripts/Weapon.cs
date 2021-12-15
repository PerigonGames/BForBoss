using System;
using PerigonGames;
using UnityEngine;

namespace Perigon.Weapons
{
    public class Weapon 
    {
         private readonly IRandomUtility _randomUtility;
         private readonly IWeaponProperties _weaponProperties;

         private float _elapsedRateOfFire;
         private float _elapsedReloadDuration;
         private int _ammunitionAmount;
         
         public bool IsReloading { get; set; }

         public bool ActivateWeapon
         {
             set => OnSetWeaponActivate?.Invoke(value);
         }
         
         public event Action<int> OnFireWeapon;
         public event Action<bool> OnSetWeaponActivate;
         
         public int AmmunitionAmount => _ammunitionAmount;
         public int MaxAmmunitionAmount => _weaponProperties.AmmunitionAmount;
         public string NameOfWeapon => _weaponProperties?.NameOfWeapon;
         public Sprite Crosshair => _weaponProperties?.Crosshair;
         private bool CanShoot => _elapsedRateOfFire <= 0 && _ammunitionAmount > 0 && !IsReloading;
         
         public Weapon(IWeaponProperties weaponProperties, IRandomUtility randomUtility = null)
         {
             _weaponProperties = weaponProperties;
             _randomUtility = randomUtility ?? new RandomUtility();
             _elapsedReloadDuration = weaponProperties.ReloadDuration;
             _ammunitionAmount = weaponProperties.AmmunitionAmount;
         }

         public void DecrementElapsedTimeRateOfFire(float deltaTime)
         {
             _elapsedRateOfFire = Mathf.Clamp(_elapsedRateOfFire - deltaTime, 0, float.PositiveInfinity);
         }

         public void ReloadWeaponCountDownIfNeeded(float deltaTime)
         {
             if (_ammunitionAmount <= 0)
             {
                 IsReloading = true;
             }
             
             if (IsReloading)
             {
                 _elapsedReloadDuration -= deltaTime;
             }
             
             if (_elapsedReloadDuration <= 0)
             {
                 ResetWeaponState();
             }
         }
         
         public Vector3 GetShootDirection(Vector3 from, Vector3 to)
         {
             var directionWithoutSpread = to - from;
             var directionWithSpread = directionWithoutSpread + GenerateSpreadAmount();
             return directionWithSpread.normalized;
         }
         
         public void FireIfPossible()
         {
             if (CanShoot)
             {
                 StopReloading();
                 _ammunitionAmount--;
                 ResetRateOfFire();
                 Fire();
             }
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
         }

         private void Fire()
         { 
             OnFireWeapon?.Invoke(_weaponProperties.BulletsPerShot);
         }

         private Vector3 GenerateSpreadAmount()
         {
             var spread = _weaponProperties.BulletSpread;
             var spreadRange = spread * 2;
             var x = -spread + (float)_randomUtility.NextDouble() * spreadRange;
             var y = -spread + (float)_randomUtility.NextDouble() * spreadRange;
             var z = -spread + (float) _randomUtility.NextDouble() * spreadRange;
             return new Vector3(x, y, z);
         }
         
         private void ResetRateOfFire()
         {
             _elapsedRateOfFire = _weaponProperties.RateOfFire;
         }
    }
}
