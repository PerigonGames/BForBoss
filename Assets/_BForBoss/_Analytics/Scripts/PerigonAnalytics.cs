using System;
using System.Collections;
using mixpanel;

namespace Perigon.Analytics
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

    public readonly struct Profile
    {
        public const String Name = "$name";
        public readonly struct Controls
        {
            public const String MouseHorizontalSens = "mouse_horizontal_sens";
            public const String MouseVerticalSens = "mouse_vertical_sens";
            public const String ControllerHorizontalSens = "controller_horizontal_sens";
            public const String ControllerVerticalSens = "controller_vertical_sens";
            public const String Inverted = "inverted";
        }

        public const String PointOfView = "point_of_view";
    }

    public readonly struct PointOfView
    {
        public const String FirstPerson = "first_person";
        public const String ThirdPerson = "third_person";
    }

    #endregion

    public interface IPerigonAnalytics
    {
        void StartSession(String uniqueId);
        void EndSession();
        void LogDeathEvent(String name);
        void LogEvent(String eventName);
        void LogEventWithParams(String eventName, Hashtable parameters);
        void ForceSendEvents();
        void SetUsername(string username);
        void SetControllerSettings(float horizontal, float vertical, bool isInverted);
        void SetMouseKeyboardSettings(float horizontal, float vertical, bool isInverted);

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

        void IPerigonAnalytics.SetUsername(String username)
        {
            Mixpanel.People.Set(Profile.Name, username);
        }

        void IPerigonAnalytics.SetControllerSettings(float horizontal, float vertical, bool isInverted)
        {
            Mixpanel.People.Set(Profile.Controls.ControllerHorizontalSens, horizontal);
            Mixpanel.People.Set(Profile.Controls.ControllerVerticalSens, vertical);
            Mixpanel.People.Set(Profile.Controls.Inverted, isInverted);
        }

        void IPerigonAnalytics.SetMouseKeyboardSettings(float horizontal, float vertical, bool isInverted)
        {
            Mixpanel.People.Set(Profile.Controls.MouseHorizontalSens, horizontal);
            Mixpanel.People.Set(Profile.Controls.MouseVerticalSens, vertical);
            Mixpanel.People.Set(Profile.Controls.Inverted, isInverted);
        }

        public void SetPOV(bool isThirdPersonActive)
        {
            Mixpanel.People.Set(Profile.PointOfView, isThirdPersonActive ? PointOfView.ThirdPerson : PointOfView.FirstPerson);
        }
        
        #endregion

        public void StartSession(String uniqueId)
        {
            Mixpanel.Identify(uniqueId);
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

        void IPerigonAnalytics.LogEvent(String eventName)
        {
            Mixpanel.Track(eventName);
        }

        void IPerigonAnalytics.LogEventWithParams(String eventName, Hashtable parameters)
        {
            // convert hashtable into MixPanel Value
            var props = new Value();
            foreach (DictionaryEntry pair in parameters)
            {
                props[pair.Key.ToString()] = pair.Value.ToString();
            }
            Mixpanel.Track(eventName, props);
        }

        void IPerigonAnalytics.ForceSendEvents()
        {
            Mixpanel.Flush();
        }
    }
}
