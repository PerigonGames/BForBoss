using System;
using System.Collections;
using mixpanel;

namespace BForBoss
{
    public readonly struct PlayerDeathEvent
    {
        public PlayerDeathEvent(String course, String name)
        {
            EventName = "Player - Death";
            Course = course;
            Name = name;
        }
        
        public String EventName { get; }
        public String Course { get; }
        public String Name { get; }
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
        private const String DefaultCourse = "racecourse";
        private const String Course = "course";
        private const String Name = "name";
        
        private const String SessionStart = "Session Start";
        private const String SessionEnd = "Session End";
        
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
            
            PlayerDeathEvent playerDeathEvent = new PlayerDeathEvent(DefaultCourse, name);
            
            props[Course] = playerDeathEvent.Course;
            props[Name] = playerDeathEvent.Name;
            Mixpanel.Track(playerDeathEvent.EventName, props);
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
