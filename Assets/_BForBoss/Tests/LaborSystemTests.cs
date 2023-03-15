using BForBoss;
using BForBoss.Labor;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Labors
{
    public class LaborSystemTests
    {
        [Test]
        public void InitializeLaborSystemTest()
        {
            // Given
            var mock1 = new MockLabor();
            var mock2 = new MockLabor();
            var mock3 = new MockLabor();
            var mock4 = new MockLabor();
            var array = new ILabor[] {mock1, mock2, mock3, mock4};
            
            var system = new LaborSystem();
                
            // When
            system.Initialize(array);
            
            // Then
            Assert.IsTrue(mock1.IsActivated);
            Assert.IsFalse(mock2.IsActivated);
            Assert.AreEqual(system.CurrentLabor, mock1);
        }
        
        [Test]
        public void ProgressThroughLaborSystemTest()
        {
            // Given
            var mock1 = new MockLabor();
            var mock2 = new MockLabor();
            var mock3 = new MockLabor();
            var mock4 = new MockLabor();
            var array = new ILabor[] {mock1, mock2, mock3, mock4};
            
            var system = new LaborSystem();
                
            // When
            system.Initialize(array);
            mock1.CompleteLabor();
            
            // Then
            Assert.IsTrue(mock2.IsActivated);
            Assert.AreEqual(system.CurrentLabor, mock2);
        }
        
        [Test]
        public void FinishLaborSystemTest()
        {
            // Given
            var mock1 = new MockLabor();
            var mock2 = new MockLabor();
            var mock3 = new MockLabor();
            var mock4 = new MockLabor();
            var array = new ILabor[] {mock1, mock2, mock3, mock4};
            
            var system = new LaborSystem();
            var finishedQueue = false;
                
            // When
            system.onQueueCompleted += () => finishedQueue = true;
            system.Initialize(array);
            mock1.CompleteLabor();
            mock2.CompleteLabor();
            mock3.CompleteLabor();
            mock4.CompleteLabor();
            
            // Then
            Assert.IsTrue(finishedQueue);
        }
        
        [Test]
        public void CompleteInactiveLaborTest()
        {
            // Given
            var mock1 = new MockLabor();
            var mock2 = new MockLabor();
            var mock3 = new MockLabor();
            var mock4 = new MockLabor();
            var array = new ILabor[] {mock1, mock2, mock3, mock4};
            
            var system = new LaborSystem();
            var finishedQueue = false;
                
            // When
            system.onQueueCompleted += () => finishedQueue = true;
            system.Initialize(array);
            mock1.CompleteLabor();
            mock3.CompleteLabor();
            mock4.CompleteLabor();
            
            // Then
            Assert.IsFalse(finishedQueue);
            Assert.AreEqual(system.CurrentLabor, mock2);
            Assert.IsFalse(mock3.IsActivated);
        }
    }
}
