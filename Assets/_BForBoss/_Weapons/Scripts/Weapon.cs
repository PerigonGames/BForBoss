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
         private bool _isReloading;
         
         public event Action<int> OnFireWeapon;

         private bool CanShoot => _elapsedRateOfFire <= 0 && _ammunitionAmount > 0 && !_isReloading;
         
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

         public void StartReloading()
         {
             _isReloading = true;
         }

         public void ReloadWeaponCountDown(float deltaTime)
         {
             if (_isReloading)
             {
                 _elapsedReloadDuration -= deltaTime;
             }
             ///TODO - DOuble check how it works in other games when running out of bullets
             
             if (_ammunitionAmount <= 0)
             {
                 
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
                 _ammunitionAmount--;
                 Debug.Log("Ammo :"+ _ammunitionAmount);
                 ResetRateOfFire();
                 Fire();
             }
         }

         private void ResetWeaponState()
         {
             _ammunitionAmount = _weaponProperties.AmmunitionAmount;
             _elapsedReloadDuration = _weaponProperties.ReloadDuration;
             Debug.Log("Ammo :"+ _ammunitionAmount);
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
