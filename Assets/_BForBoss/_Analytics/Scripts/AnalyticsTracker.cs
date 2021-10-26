using System;
using UnityEngine;

namespace BForBoss
{
    public class AnalyticsTracker : MonoBehaviour
    {
        private const String SessionStart = "Session Start";
        private const String SessionEnd = "Session End";
        private PerigonAnalytics _perigonAnalytics = new PerigonAnalytics();
        
        private void Start()
        {
            _perigonAnalytics.LogEvent(SessionStart);
        }

        // this also works in Editor when playmode is stopped
        private void OnApplicationQuit()
        {
            _perigonAnalytics.LogEvent(SessionEnd);
            // before the game ends, make sure any analytics collected is sent to Mixpanel
            _perigonAnalytics.ForceSendEvents();
        }
    }
}
