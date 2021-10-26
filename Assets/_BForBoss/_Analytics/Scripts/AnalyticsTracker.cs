using System;
using UnityEngine;

namespace BForBoss
{
    public class AnalyticsTracker : MonoBehaviour
    {
        private void Start()
        {
            PerigonAnalytics.StartSession();
        }

        // this also works in Editor when playmode is stopped
        private void OnApplicationQuit()
        {
            PerigonAnalytics.EndSession();
        }
    }
}
