using NUnit.Framework;
using Perigon.Weapons;
using PerigonGames;
using UnityEngine;

namespace Tests.Weapons
{
    public class WeaponTests
    {
        
        [Test]
        public void Test_FireIfPossible_CanShootInBeginning()
        {
            // Given
            var mockProperties = new MockWeaponProperties();
            var weapon = new Weapon(mockProperties);
            var shootTimes = 0;
            weapon.OnFireWeapon += bullets =>
            {
                shootTimes++;
            };
            
            //When
            weapon.FireIfPossible();
            
            //Then
            Assert.AreEqual(shootTimes, 1);
        }
        
        [Test]
        public void Test_FireIfPossible_CanOnlyShootOncePerFrame()
        {
            // Given
            var mockProperties = new MockWeaponProperties();
            var weapon = new Weapon(mockProperties);
            var shootTimes = 0;
            weapon.OnFireWeapon += bullets =>
            {
                shootTimes++;
            };
            weapon.FireIfPossible();
            
            //When
            weapon.FireIfPossible();
            
            //Then
            Assert.AreEqual(shootTimes, 1);
        }

        [Test]
        public void Test_FireIfPossible_ShootAfterElapsedRateOfTimeHitsZero()
        {
            // Given
            var mockProperties = new MockWeaponProperties();
            var weapon = new Weapon(mockProperties);
            var shootTimes = 0;
            weapon.OnFireWeapon += bullets =>
            {
                shootTimes++;
            };
            weapon.FireIfPossible();
            weapon.DecrementElapsedTimeRateOfFire(100);
            
            //When
            weapon.FireIfPossible();
            
            //Then
            Assert.AreEqual(shootTimes, 2);
        }
        
        [Test]
        public void Test_GetShootDirection_NoSpread_ShootForward()
        {
            // Given
            var mockProperties = new MockWeaponProperties(bulletSpread: 0);
            var mockRandom = new MockRandom();
            var weapon = new Weapon(mockProperties, mockRandom);
            var from = Vector3.zero;
            var to = new Vector3(0, 0, 15);
            
            
            //When
            var actualResult = weapon.GetShootDirection(from, to);
            
            //Then
            Assert.AreEqual(Vector3.forward, actualResult, "Should shoot forward");
        }
        
        [Test]
        public void Test_GetShootDirection_WithSpread_ShootForward()
        {
            // Given
            var mockProperties = new MockWeaponProperties(bulletSpread: 1);
            var mockRandom = new MockRandom {RandomDouble = 1};
            var weapon = new Weapon(mockProperties, mockRandom);
            var from = Vector3.zero;
            var to = new Vector3(0, 0, 15);
            var expectedResult = new Vector3(1, 1, 15);
            var normalizedExpectedResult = expectedResult.normalized;
            
            
            //When
            var actualResult = weapon.GetShootDirection(from, to);
            
            //Then
            Assert.IsTrue(TestUtilities.WithinBounds(normalizedExpectedResult, actualResult), "Should shoot forward with spread up right and normalized");
        }
        
        [Test]
        public void Test_GetShootDirection_WithTenBullets_FireTenBullets()
        {
            // Given
            var expectedNumberOfBullets = 10;
            var mockProperties = new MockWeaponProperties(bulletsPerShot: expectedNumberOfBullets);
            var mockRandom = new MockRandom {RandomDouble = 1};
            var weapon = new Weapon(mockProperties, mockRandom);
            var actualNumberOfBullets = 0;
            weapon.OnFireWeapon += bullets =>
            {
                actualNumberOfBullets = bullets;
            };
            
            //When
            weapon.FireIfPossible();
            
            //Then
            Assert.AreEqual(expectedNumberOfBullets, actualNumberOfBullets, "Number of bullets shot");
        }

        [Test]
        public void Test_ReloadWeaponCountDownIfNeeded_WithNoAmmunition()
        {
            //Given 
            var mockProperties = new MockWeaponProperties(ammoAmount: 1);
            var weapon = new Weapon(mockProperties);
            weapon.FireIfPossible();
            
            //When
            weapon.ReloadWeaponCountDownIfNeeded(0);
            
            //Then
            Assert.IsTrue(weapon.IsReloading, "Weapon should be reloading when out of ammo");
        }

        [Test]
        public void Test_ReloadWeaponCountDownIfNeeded_WithSomeAmmunition_CompletesReload()
        {
            //Given 
            var mockProperties = new MockWeaponProperties(ammoAmount: 5, reloadDuration: 0.5f);
            var weapon = new Weapon(mockProperties);
            weapon.FireIfPossible();
            weapon.IsReloading = true;
            
            //When
            weapon.ReloadWeaponCountDownIfNeeded(0.5f);
            
            //Then
            Assert.IsFalse(weapon.IsReloading, "Weapon should have completed reloading");
        }
    }
}
