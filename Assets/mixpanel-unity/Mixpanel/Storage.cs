using mixpanel.queue;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace mixpanel
{
    public static class MixpanelStorage
    {
        #region HasMigratedFrom1To2

        private const string HasMigratedFrom1To2Name = "Mixpanel.HasMigratedFrom1To2";

        internal static bool HasMigratedFrom1To2
        {
            get => Convert.ToBoolean(PlayerPrefs.GetInt(HasMigratedFrom1To2Name, 0));
            set => PlayerPrefs.SetInt(HasMigratedFrom1To2Name, Convert.ToInt32(value));
        }

        #endregion

        #region HasIntegratedLibrary

        private const string HasIntegratedLibraryName = "Mixpanel.HasIntegratedLibrary";

        internal static bool HasIntegratedLibrary
        {
            get => Convert.ToBoolean(PlayerPrefs.GetInt(HasIntegratedLibraryName, 0));
            set => PlayerPrefs.SetInt(HasIntegratedLibraryName, Convert.ToInt32(value));
        }

        #endregion

        #region DistinctId
        
        private const string DistinctIdName = "Mixpanel.DistinctId";
        
        private static string _distinctId;
        
        public static string DistinctId
        {
            get
            {
                if (!string.IsNullOrEmpty(_distinctId)) return _distinctId;
                if (PlayerPrefs.HasKey(DistinctIdName))
                {
                    _distinctId = PlayerPrefs.GetString(DistinctIdName);
                }
                // Generate a Unique ID for this client if still null or empty
                // https://devblogs.microsoft.com/oldnewthing/?p=21823
                if (string.IsNullOrEmpty(_distinctId)) DistinctId = Guid.NewGuid().ToString();
                return _distinctId;
            }
            set
            {
                _distinctId = value;
                PlayerPrefs.SetString(DistinctIdName, _distinctId);
            }
        }
        
        #endregion

        #region IsTracking

        private const string IsTrackingName = "Mixpanel.IsTracking";
        
        private static bool _isTracking;

        public static bool IsTracking
        {
            get
            {
                if (!PlayerPrefs.HasKey(IsTrackingName)) IsTracking = true;
                else _isTracking = PlayerPrefs.GetInt(IsTrackingName) == 1;
                return _isTracking;
            }
            set
            {
                _isTracking = value;
                PlayerPrefs.SetInt(IsTrackingName, _isTracking ? 1 : 0);
            }
        }

        #endregion

        #region PushDeviceToken

        private const string PushDeviceTokenName  = "Mixpanel.PushDeviceToken";

        private static string _pushDeviceTokenString;

        internal static string PushDeviceTokenString
        {
            get
            {
                if (!string.IsNullOrEmpty(_pushDeviceTokenString)) return _pushDeviceTokenString;
                if (!PlayerPrefs.HasKey(PushDeviceTokenName)) PushDeviceTokenString = "";
                else _pushDeviceTokenString = PlayerPrefs.GetString(PushDeviceTokenName);
                return _pushDeviceTokenString;
            }
            set
            {
                _pushDeviceTokenString = value;
                PlayerPrefs.SetString(PushDeviceTokenName, _pushDeviceTokenString);
            }
        }
        
        [Conditional("UNITY_IOS")]
        internal static void SavePushDeviceToken(string token)
        {
            PushDeviceTokenString = token;
        }

        #endregion
        
        #region OnceProperties
        
        private const string OncePropertiesName = "Mixpanel.OnceProperties";

        private static Value _onceProperties;

        internal static Value OnceProperties
        {
            get
            {
                if (_onceProperties != null) return _onceProperties;
                if (!PlayerPrefs.HasKey(OncePropertiesName)) OnceProperties = Mixpanel.ObjectPool.Get();
                else
                {
                    _onceProperties = Mixpanel.ObjectPool.Get();
                    JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(OncePropertiesName), _onceProperties);
                }
                return _onceProperties;
            }
            set
            {
                _onceProperties = value;
                PlayerPrefs.SetString(OncePropertiesName, JsonUtility.ToJson(_onceProperties));
            }
        }

        internal static void ResetOnceProperties()
        {
            Value properties = OnceProperties;
            properties.OnRecycle();
            OnceProperties = properties;
        }
        
        #endregion
        
        #region SuperProperties
        
        private const string SuperPropertiesName = "Mixpanel.SuperProperties";

        private static Value _superProperties;

        internal static Value SuperProperties
        {
            get
            {
                if (_superProperties != null) return _superProperties;
                if (!PlayerPrefs.HasKey(SuperPropertiesName)) SuperProperties = Mixpanel.ObjectPool.Get();
                else
                {
                    _superProperties = Mixpanel.ObjectPool.Get(); 
                    JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(SuperPropertiesName), _superProperties);
                }
                return _superProperties;
            }
            set
            {
                _superProperties = value;
                PlayerPrefs.SetString(SuperPropertiesName, JsonUtility.ToJson(_superProperties));
            }
        }
        
        internal static void ResetSuperProperties()
        {
            Value properties = SuperProperties;
            properties.OnRecycle();
            SuperProperties = properties;
        }
        
        #endregion
        
        #region TimedEvents
        
        private const string TimedEventsName = "Mixpanel.TimedEvents";

        private static Value _timedEvents;

        internal static Value TimedEvents
        {
            get
            {
                if (_timedEvents != null) return _timedEvents;
                if (!PlayerPrefs.HasKey(TimedEventsName)) TimedEvents = Mixpanel.ObjectPool.Get();
                else 
                {
                    _timedEvents = Mixpanel.ObjectPool.Get();
                    JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(TimedEventsName), _timedEvents);
                }
                return _timedEvents;
            }
            set
            {
                _timedEvents = value;
                PlayerPrefs.SetString(TimedEventsName, JsonUtility.ToJson(_timedEvents));
            }
        }
        
        internal static void ResetTimedEvents()
        {
            Value properties = TimedEvents;
            properties.OnRecycle();
            TimedEvents = properties;
        }
        
        #endregion

        #region PersistentQueue

        public static readonly PersistentQueue TrackPersistentQueue = new PersistentQueue(Path.Combine(Application.persistentDataPath, "mixpanel_track_queue"));

        public static readonly PersistentQueue EngagePersistentQueue = new PersistentQueue(Path.Combine(Application.persistentDataPath, "mixpanel_engage_queue"));

        #endregion
    }
}
