using System;
using System.Collections;
using mixpanel;

namespace BForBoss
{
    #region STRUCTS
    public readonly struct Event
    {
        public const String PlayerDeath = "Player - Death";
        public const String CheckpointReached = "Checkpoint - Reached";
    }

    public readonly struct EventAttribute
    {
        public const String Course = "course";
        public const String Name = "name";
        public const String Time = "timer";
    }

    public readonly struct EventConstant
    {
        public const String RaceCourse = "racecourse";
    }
    #endregion

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

        #region CONSTRUCTORS / SETTERS
        static PerigonAnalytics()
        {
        }

        private PerigonAnalytics()
        {
        }

        public void SetSessionUsername(String username)
        {
            Mixpanel.Identify(username);
            Mixpanel.People.Set("$name", username);
        }
        
        #endregion
        
        public void StartSession()
        {
            Mixpanel.Track(SessionStart);
        }

        public void EndSession()
        {
            Mixpanel.Track(SessionEnd);
            Mixpanel.Flush();
        }

        #region CUSTOM EVENTS

        public void LogDeathEvent(String deathAreaName)
        {
            var props = new Value();
            
            props[EventAttribute.Course] = EventConstant.RaceCourse;
            props[EventAttribute.Name] = deathAreaName;
            Mixpanel.Track(Event.PlayerDeath, props);
        }

        public void LogCheckpointEvent(float time, String checkpointName)
        {
            var props = new Value();
            
            props[EventAttribute.Time] = time;
            props[EventAttribute.Course] = EventConstant.RaceCourse;
            props[EventAttribute.Name] = checkpointName;
            Mixpanel.Track(Event.CheckpointReached, props);
        }
        
        #endregion

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
