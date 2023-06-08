using System;
using UnityEngine;

namespace BForBoss
{
    [DisallowMultipleComponent]
    public class DerekBossManager : MonoBehaviour
    {
        private Action _onPhaseComplete;
        
        public void Initialize(Action onPhaseComplete)
        {
            _onPhaseComplete = onPhaseComplete;
        }
        
        public void Reset()
        {
            //Reset Derek Position
            //Reset Derek Death Walls
            //Reset Missile Behavior to default Params
            //Raise Shields
        }
        
        public void UpdatePhase(DerekContextManager.Phase phase)
        {
            switch (phase)
            {
                case DerekContextManager.Phase.Tutorial:
                case DerekContextManager.Phase.Death:
                    break;
                case DerekContextManager.Phase.FirstPhase:
                    //_healthBarViewBehavior.SetPhaseBoundary(0.75f, _onPhaseComplete.Invoke());
                    //_derekMissileBehavior.UpdateParameters(AdjustedParameters[])
                    Debug.Log("Phase 1: Will transition at 75% health");
                    break;
                case DerekContextManager.Phase.SecondPhase:
                    //_healthBarViewBehavior.SetPhaseBoundary(0.25f, _onPhaseComplete.Invoke());
                    //_derekMissileBehavior.UpdateParameters(AdjustedParameters[])
                    Debug.Log("Phase 2: Will transition at 25% health");
                    break;
                case DerekContextManager.Phase.FinalPhase:
                    //_healthBarViewBehavior.SetPhaseBoundary(0f, _onPhaseComplete.Invoke());
                    //_derekMissileBehavior.UpdateParameters(AdjustedParameters[])
                    Debug.Log("Phase 3: Will transition at 0% health");
                    break;
                default:
                    break;
            }
        }

        public void UpdateVulnerability(DerekContextManager.Vulnerability vulnerability)
        {
            switch (vulnerability)
            {
                case DerekContextManager.Vulnerability.Invulnerable:
                    //Raise Shields
                    //Recenter Derek Position
                    //Find Closest Death Wall and activate - 
                    //Start Movement with new given rotation (i.e. clockwise/anticlockwise)
                    //Give small buffer of wait time - then StartMissile behavior
                    //Ensure Derek is invincible in this case (edge case)
                    
                    Debug.Log("Raising Shields");
                    Debug.Log("Resetting Position");
                    Debug.Log("Activating Death Wall Closest to player");
                    Debug.Log("Rotating to make sure death wall gets closer to player");
                    Debug.Log("Starting Homing Missiles");
                    Debug.Log("------------------------");
                    
                    break;
                case DerekContextManager.Vulnerability.Vulnerable:
                    //End Missile Behavior
                    //Deactivate all DeathWalls
                    //Lower Shields
                    //Ensure Derek is susceptible to damage
                    
                    Debug.Log("Stopping Missiles");
                    Debug.Log("Deactivating Death Walls");
                    Debug.Log("Lowering Shields");
                    Debug.Log("------------------------");
                    break;
            }
        }
    }
}
