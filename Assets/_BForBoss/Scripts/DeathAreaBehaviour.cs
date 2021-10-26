using System;
using System.Collections;
using UnityEngine;

namespace BForBoss
{
    public class DeathAreaBehaviour : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            StateManager.Instance.SetState(State.Death);
            PerigonAnalytics.LogDeathEvent("racecourse", gameObject.name);        
        }
    }
}
