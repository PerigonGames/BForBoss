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
        void LogEventWithParams(String eventName, Hashtable parameters = null);
        void ForceSendEvents();
        void SetUserProperties(Hashtable properties);
    }
    
    public class PerigonAnalytics : IPerigonAnalytics
    {
        private const String SessionStart = "$session_start";
        private const String SessionEnd = "$session_end";
        
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

        public void LogEventWithParams(String eventName, Hashtable parameters = null)
        {
            var props = new Value();
            foreach (DictionaryEntry pair in parameters ?? new Hashtable())
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
