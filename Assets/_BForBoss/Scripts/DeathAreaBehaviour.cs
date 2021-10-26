using System;
using System.Collections;
using UnityEngine;

namespace BForBoss
{
    public class DeathAreaBehaviour : MonoBehaviour
    {
        private const String PlayerDeathEvent = "Player - Death";
        private void OnCollisionEnter(Collision other)
        {
            StateManager.Instance.SetState(State.Death);
            SendPlayerDeathEvent();
        }

        private void SendPlayerDeathEvent()
        {
            PerigonAnalytics perigonAnalytics = new PerigonAnalytics();
            
            Hashtable parameters = new Hashtable();
            parameters.Add("course", "racecourse");
            parameters.Add("name", gameObject.name);

            perigonAnalytics.LogEventWithParams(PlayerDeathEvent, parameters);
        }
    }
}
