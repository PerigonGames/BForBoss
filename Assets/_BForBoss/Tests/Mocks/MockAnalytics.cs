using System.Collections;
using Perigon.Analytics;

namespace Tests
{
    public class MockAnalytics : IPerigonAnalytics
    {
        public void StartSession(string uniqueId) {}

        public void EndSession() {}

        public void LogDeathEvent(string name) {}

        public void LogEvent(string eventName) {}

        public void LogEventWithParams(string eventName, Hashtable parameters) {}

        public void ForceSendEvents() {}

        public void SetUsername(string username) {}

        public void SetControlSettings(float horizontalMouse, float verticalMouse, float horizontalController,
            float verticalController, bool isInverted) { }
    }
}
