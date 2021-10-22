using NUnit.Framework;
using BForBoss;

namespace Tests.Manager
{
    public class TimerManagerTests
    {
        [Test]
        public void Test_UpdateTime_StartTimer_10SecondPasses()
        {
            bool didEventGetCalled = false;
            void ManagerOnOnTimeChanged(float obj)
            {
                didEventGetCalled = true;
            }
            
            // Arrange
            var manager = new TimeManagerViewModel();
            manager.StartTimer();
            manager.OnTimeChanged += ManagerOnOnTimeChanged;
            
            // Act
            manager.Update(10);
            
            // Assert
            Assert.AreEqual(manager.CurrentGameTime, 10, "10 Seconds should pass");
            Assert.IsTrue(didEventGetCalled, "Event should have gotten called");
        }


        [Test]
        public void Test_UpdateTime_StopTimer_NoSecondsPasses()
        {
            // Arrange
            var manager = new TimeManagerViewModel();
            manager.StopTimer();
            
            // Act
            manager.Update(10);
            
            // Assert
            Assert.AreEqual(manager.CurrentGameTime, 0, "0 Seconds should pass");
        }
        
        [Test]
        public void Test_UpdateTime_ThenReset_NoSecondsPasses()
        {
            // Arrange
            var manager = new TimeManagerViewModel();
            manager.StartTimer();
            
            // Act
            manager.Update(10);
            manager.Reset();
            
            // Assert
            Assert.AreEqual(manager.CurrentGameTime, 0, "0 Seconds should pass");
        }
        
        [Test]
        public void Test_UpdateTime_ThenReset_ThenUpdate_NoSecondsPasses()
        {
            // Arrange
            var manager = new TimeManagerViewModel();
            manager.StartTimer();
            
            // Act
            manager.Update(10);
            manager.Reset();
            manager.Update(100);
            
            // Assert
            Assert.AreEqual(manager.CurrentGameTime, 0, "0 Seconds should pass");
        }
    }
}