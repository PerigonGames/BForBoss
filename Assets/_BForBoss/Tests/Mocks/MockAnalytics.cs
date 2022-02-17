using System.Collections;
using Perigon.Analytics;

namespace Tests
{
    public class MockAnalytics : IBForBossAnalytics
    {
        public void StartSession(string uniqueId) {}

        public void EndSession() {}

        public void LogDeathEvent(string name) {}

        public void LogEvent(string eventName) {}

        public void LogEventWithParams(string eventName, Hashtable parameters) {}

        public void ForceSendEvents() {}

        public void SetUsername(string username) {}
        public void SetControllerSettings(float horizontal, float vertical, bool isInverted)
        {
        }

        public void SetMouseKeyboardSettings(float horizontal, float vertical, bool isInverted)
        {
        }

        public void LogCheckpointEvent(float time, string checkpointName) { }
    }
}
