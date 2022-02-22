using Perigon.Analytics;
using UnityEngine;

namespace BForBoss
{
    public class DeathAreaBehaviour : MonoBehaviour
    {
        [SerializeField] private string _deathAreaName = "deathArea";
        private WorldNameAnalyticsName _worldNameAnalytics = WorldNameAnalyticsName.Unknown;
        
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
                    BForBossAnalytics.Instance.LogDeathEvent(gameObject.scene.name, _deathAreaName);
                    break;
            }
        }
    }
}
