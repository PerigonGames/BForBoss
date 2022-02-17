using System;
using System.Collections;
using System.Collections.Generic;
using mixpanel;

namespace Perigon.Analytics
{
    public interface IPerigonAnalytics
    {
        void StartSession(String uniqueId);
        void EndSession();
        void LogEvent(String eventName);
        void LogEventWithParams(String eventName, Hashtable parameters);
        void ForceSendEvents();
        void SetUserProperties(Hashtable properties);
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

        public void LogEvent(String eventName)
        {
            Mixpanel.Track(eventName);
        }
        
        public void LogEventWithParams(String eventName, Hashtable parameters)
        {
            var props = new Value();
            foreach (DictionaryEntry pair in parameters)
            {
                props[pair.Key.ToString()] = pair.Value.ToString();
            }
            Mixpanel.Track(eventName, props);
        }

        public void SetUserProperties(Hashtable properties)
        {
            foreach (DictionaryEntry pair in properties)
            {
                Mixpanel.People.Set(pair.Key.ToString(), pair.Value.ToString());
            }
        }

        public void ForceSendEvents()
        {
            Mixpanel.Flush();
        }
    }
}
