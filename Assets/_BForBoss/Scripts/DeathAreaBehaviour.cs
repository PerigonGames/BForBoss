using System;
using UnityEngine;

namespace BForBoss
{
    public class DeathAreaBehaviour : MonoBehaviour
    {
        [SerializeField] private String _deathAreaName = "deathArea";
        private void OnCollisionEnter(Collision other)
        {
            StateManager.Instance.SetState(State.Death);
            PerigonAnalytics.Instance.LogDeathEvent(_deathAreaName);        
        }
    }
}
