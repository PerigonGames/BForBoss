using System;
using System.Collections;
using mixpanel;

namespace BForBoss
{

    public interface IPerigonAnalytics
    {
        public void LogEvent(String eventName);
        public void LogEventWithParams(String eventName, Hashtable parameters);

        public void ForceSendEvents();
    }
    
    public class PerigonAnalytics : IPerigonAnalytics
    {
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
