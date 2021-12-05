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
            weapon.OnFireWeapon += () =>
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
            weapon.OnFireWeapon += () =>
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
            weapon.OnFireWeapon += () =>
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
            var normalizedResult = expectedResult.normalized;
            
            
            //When
            var actualResult = weapon.GetShootDirection(from, to);
            
            //Then
            Assert.AreEqual(normalizedResult, actualResult, "Should shoot forward with spread up right and normalized");
        }
    }
}
