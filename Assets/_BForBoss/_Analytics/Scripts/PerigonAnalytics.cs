using System;
using System.Collections;
using mixpanel;

namespace BForBoss
{
    public static class PerigonAnalytics
    {
        private const String PlayerDeathEvent = "Player - Death";
        private const String SessionStart = "Session Start";
        private const String SessionEnd = "Session End";
        
        public static void StartSession()
        {
            Mixpanel.Track(SessionStart);
        }

        public static void EndSession()
        {
            Mixpanel.Track(SessionEnd);
            Mixpanel.Flush();
        }
        
        public static void LogDeathEvent(String course, String name)
        {
            var props = new Value();
            props["course"] = course;
            props["name"] = name;
            Mixpanel.Track(PlayerDeathEvent, props);
        }

        public static void LogEvent(String eventName)
        {
            Mixpanel.Track(eventName);
        }

        public static void LogEventWithParams(String eventName, Hashtable parameters)
        {
            // convert hashtable into MixPanel Value
            var props = new Value();
            foreach (DictionaryEntry pair in parameters)
            {
                props[pair.Key.ToString()] = pair.Value.ToString();
            }
            Mixpanel.Track(eventName, props);
        }

        public static void ForceSendEvents()
        {
            Mixpanel.Flush();
        }
    }
}
