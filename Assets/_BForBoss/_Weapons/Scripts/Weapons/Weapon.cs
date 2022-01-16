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
             set
             {
                 StopReloading();
                 OnSetWeaponActivate?.Invoke(value);
             }
         }
         
         public event Action<int> OnFireWeapon;
         public event Action<bool> OnSetWeaponActivate;
         
         public int AmmunitionAmount => _ammunitionAmount;
         public int MaxAmmunitionAmount => _weaponProperties.AmmunitionAmount;
         public float MaxReloadDuration => _weaponProperties.ReloadDuration;
         public float ElapsedReloadDuration => _elapsedReloadDuration;
         public string NameOfWeapon => _weaponProperties?.NameOfWeapon;
         public Sprite Crosshair => _weaponProperties?.Crosshair;
         public float VisualRecoilForce => _weaponProperties.VisualRecoilForce;
         private bool CanShoot => _elapsedRateOfFire <= 0 && _ammunitionAmount > 0;
         
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

         public void ReloadWeaponIfPossible()
         {
             if (_ammunitionAmount < _weaponProperties.AmmunitionAmount)
             {
                 IsReloading = true;
             }
         }
         
         public Vector3 GetShootDirection(Vector3 from, Vector3 to)
         {
             Vector3 directionWithoutSpread = to - from;
             var directionWithSpread = GenerateSpreadAngle() * directionWithoutSpread;
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

         private Quaternion GenerateSpreadAngle()
         {
             var spread = _weaponProperties.BulletSpread;
             var spreadRange = spread * 2;
             var randomizedSpread = -spread + (float)_randomUtility.NextDouble() * spreadRange;
             var randomizedDirection = new Vector3(RandomDirection(), RandomDirection(), RandomDirection());
             return Quaternion.AngleAxis(randomizedSpread, randomizedDirection);
         }

         private float RandomDirection()
         {
             return (float) _randomUtility.NextDouble() * (_randomUtility.CoinFlip() ? 1 : -1);
         }
         
         private void ResetRateOfFire()
         {
             _elapsedRateOfFire = _weaponProperties.RateOfFire;
         }
    }
}
