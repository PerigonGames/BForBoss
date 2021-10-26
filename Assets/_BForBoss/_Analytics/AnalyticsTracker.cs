using mixpanel;
using UnityEngine;

namespace BForBoss
{
    public class AnalyticsTracker : MonoBehaviour
    {
        private void Start()
        {
            Mixpanel.Track("Session Start");
        }

        // this also works in Editor when playmode is stopped
        private void OnApplicationQuit()
        {
            Mixpanel.Track("Session End");
        }
    }
}
