using System;
using System.Collections;
using mixpanel;

namespace BForBoss
{
    public readonly struct Event
    {
        public const String PlayerDeath = "Player - Death";
    }

    public readonly struct EventAttribute
    {
        public const String Course = "course";
        public const String Name = "name";
    }

    public readonly struct EventConstant
    {
        public const String RaceCourse = "racecourse";
    }

    public interface IPerigonAnalytics
    {
        void StartSession();
        void EndSession();
        void LogDeathEvent(String name);
        void LogEvent(String eventName);
        void LogEventWithParams(String eventName, Hashtable parameters);
        void ForceSendEvents();
    }
    
    public class PerigonAnalytics : IPerigonAnalytics
    {
        private const String SessionStart = "$session_start";
        private const String SessionEnd = "$session_end";
        
        private static readonly PerigonAnalytics _instance = new PerigonAnalytics();

        public static PerigonAnalytics Instance => _instance;

        static PerigonAnalytics()
        {
        }

        private PerigonAnalytics()
        {
        }

        public void StartSession()
        {
            Mixpanel.Track(SessionStart);
        }

        public void EndSession()
        {
            Mixpanel.Track(SessionEnd);
            Mixpanel.Flush();
        }
        
        public void LogDeathEvent(String name)
        {
            var props = new Value();
            
            props[EventAttribute.Course] = EventConstant.RaceCourse;
            props[EventAttribute.Name] = name;
            Mixpanel.Track(Event.PlayerDeath, props);
        }

        public void LogEvent(String eventName)
        {
            Mixpanel.Track(eventName);
        }

        public void LogEventWithParams(String eventName, Hashtable parameters)
        {
            // convert hashtable into MixPanel Value
            var props = new Value();
            foreach (DictionaryEntry pair in parameters)
            {
                props[pair.Key.ToString()] = pair.Value.ToString();
            }
            Mixpanel.Track(eventName, props);
        }

        public void ForceSendEvents()
        {
            Mixpanel.Flush();
        }
    }
}
