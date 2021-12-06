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
         
         public event Action<int> OnFireWeapon;

         private bool CanShoot => _elapsedRateOfFire <= 0;
         
         public Weapon(IWeaponProperties weaponProperties, IRandomUtility randomUtility = null)
         {
             _weaponProperties = weaponProperties;
             _randomUtility = randomUtility ?? new RandomUtility();
         }

         public void DecrementElapsedTimeRateOfFire(float deltaTime)
         {
             _elapsedRateOfFire = Mathf.Clamp(_elapsedRateOfFire - deltaTime, 0, float.PositiveInfinity);
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
                 ResetRateOfFire();
                 Fire();
             }
         }

         private void Fire()
         { 
             OnFireWeapon?.Invoke(_weaponProperties.NumberOfBullets);
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
