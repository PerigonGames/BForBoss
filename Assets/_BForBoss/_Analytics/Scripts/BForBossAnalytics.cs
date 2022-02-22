using System;
using System.Collections;

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
        public const String MS1RaceCourse = "ms1_racecourse";
        public const String MS2RaceCourse = "ms2_racecourse";
        public const string WeaponsRaceCourse = "weapons_racecourse";
        public const String Unknown = "unknown";
    }
    public readonly struct Profile
    {
        public const string Name = "$name";
        public readonly struct Controls
        {
            public const string MouseHorizontalSens = "mouse_horizontal_sens";
            public const string MouseVerticalSens = "mouse_vertical_sens";
            public const string ControllerHorizontalSens = "controller_horizontal_sens";
            public const string ControllerVerticalSens = "controller_vertical_sens";
            public const string Inverted = "inverted";
        }
    }
    #endregion
   
    public interface IBForBossAnalytics
    {
        void LogDeathEvent(string world, string deathAreaName);
        void LogCheckpointEvent(string world, float time, string checkpointName);
        void SetUsername(string username);
        void SetMouseKeyboardSettings(float horizontal, float vertical, bool isInverted);
        void SetControllerSettings(float horizontal, float vertical, bool isInverted);
    }

    public class BForBossAnalytics : IBForBossAnalytics
    {
        private readonly IPerigonAnalytics _perigonAnalytics = new PerigonAnalytics();
        private static readonly BForBossAnalytics _instance = new BForBossAnalytics();
        public static BForBossAnalytics Instance => _instance;

        #region CONSTRUCTORS
        static BForBossAnalytics()
        {
        }

        private BForBossAnalytics()
        {
        }
        #endregion

        public void SetUsername(string username)
        {
            var properties = new Hashtable();
            properties.Add(Profile.Name, username);
            _perigonAnalytics.SetUserProperties(properties);
        }

        public void SetControllerSettings(float horizontal, float vertical, bool isInverted)
        {
            var properties = new Hashtable();
            properties.Add(Profile.Controls.ControllerHorizontalSens, horizontal);
            properties.Add(Profile.Controls.ControllerVerticalSens, vertical);
            properties.Add(Profile.Controls.Inverted, isInverted);
            _perigonAnalytics.SetUserProperties(properties);
        }
        
        public void SetMouseKeyboardSettings(float horizontal, float vertical, bool isInverted)
        {
            var properties = new Hashtable();
            properties.Add(Profile.Controls.MouseHorizontalSens, horizontal);
            properties.Add(Profile.Controls.MouseVerticalSens, vertical);
            properties.Add(Profile.Controls.Inverted, isInverted);
            _perigonAnalytics.SetUserProperties(properties);
        }

        public void LogDeathEvent(string world, string deathAreaName)
        {
            var deathArea = new Hashtable();
            deathArea[EventAttribute.Course] = world.ToLower();
            deathArea[EventAttribute.Name] = deathAreaName;
            
            _perigonAnalytics.LogEventWithParams(Event.PlayerDeath, deathArea);
        }

        public void LogCheckpointEvent(string world, float time, string checkpointName)
        {
            var checkpoint = new Hashtable();
            checkpoint[EventAttribute.Time] = time;
            checkpoint[EventAttribute.Course] = world.ToLower();
            checkpoint[EventAttribute.Name] = checkpointName;
            
            _perigonAnalytics.LogEventWithParams(Event.CheckpointReached, checkpoint);
        }

        public void StartSession(string uniqueId)
        {
            _perigonAnalytics.StartSession(uniqueId);
        }

        public void EndSession()
        {
            _perigonAnalytics.EndSession();
        }
    }
}