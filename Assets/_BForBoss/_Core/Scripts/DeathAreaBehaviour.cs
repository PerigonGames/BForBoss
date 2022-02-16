using System;
using System.Collections;
using Perigon.Analytics;
using UnityEngine;

namespace BForBoss
{
    public class DeathAreaBehaviour : MonoBehaviour
    {
        [SerializeField] private string _deathAreaName = "deathArea";
        public const string PlayerDeath = "Player - Death";

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
                    BForBossAnalytics.Instance.LogDeathEvent(_deathAreaName);
                    break;
            }
        }
    }
}
