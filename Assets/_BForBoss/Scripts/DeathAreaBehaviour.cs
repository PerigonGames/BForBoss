using System;
using UnityEngine;

namespace BForBoss
{
    public class DeathAreaBehaviour : MonoBehaviour
    {
        [SerializeField] private String _deathAreaName = "deathArea";
        private void OnCollisionEnter(Collision other)
        {
            var state = StateManager.Instance;
            switch (state.GetState())
            {
                case State.EndRace:
                    StateManager.Instance.SetState(State.PreGame);
                    break;
                default:
                    StateManager.Instance.SetState(State.Death);
                    PerigonAnalytics.Instance.LogDeathEvent(_deathAreaName);
                    break;
            }
        }
    }
}
