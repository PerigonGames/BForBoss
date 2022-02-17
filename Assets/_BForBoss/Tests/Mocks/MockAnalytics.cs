using Perigon.Analytics;

namespace Tests
{
    public class MockAnalytics : IBForBossAnalytics
    {
        public void LogDeathEvent(WorldNameAnalyticsName world, string deathAreaName) { }
        public void LogCheckpointEvent(WorldNameAnalyticsName world, float time, string checkpointName) { }
        public void SetUsername(string username) {}
        public void SetControllerSettings(float horizontal, float vertical, bool isInverted) { }
        public void SetMouseKeyboardSettings(float horizontal, float vertical, bool isInverted) { }
    }
}
