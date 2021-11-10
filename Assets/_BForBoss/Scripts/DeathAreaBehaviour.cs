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
                    state.SetState(State.PreGame);
                    break;
                default:
                    state.SetState(State.Death);
                    PerigonAnalytics.Instance.LogDeathEvent(_deathAreaName);
                    break;
            }
        }
    }
}
