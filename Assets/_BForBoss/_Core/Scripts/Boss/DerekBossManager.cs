using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    [DisallowMultipleComponent]
    public class DerekBossManager : MonoBehaviour
    {
        [SerializeField, Resolve] private DerekShieldBehaviour _shieldBehaviour;
        [SerializeField, Resolve] private BossWipeOutWallsManager _wipeoutWallsManager;
        [SerializeField] private DerekMissileLauncherBehaviour[] _missileLauncherBehaviours;
        [SerializeField, Tooltip("Temporary timer to space out Missile shots for testing purposes")] private float _timeBetweenMissileShots = 5.0f; 

        private bool _canShootAtPlayer = false;
        private float _shootTimer = 0.0f;
        
        public void Reset()
        {
            //Reset Derek Position
            //Reset Derek Death Walls
            
            //Reset Missile Behavior to default Params
            
            //Raise Shields
            _shieldBehaviour.ToggleShield(false);
        }

        public void Initialize(PlayerMovementBehaviour playerMovementBehaviour)
        {
            _wipeoutWallsManager.Initialize(playerMovementBehaviour);
            foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
            {
                if (missileLauncher == null)
                {
                    continue;
                }
                
                missileLauncher.Initialize(playerMovementBehaviour);
            }

            _shootTimer = _timeBetweenMissileShots;
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

                    {
                        Debug.Log("Raising Shields");
                        _shieldBehaviour.ToggleShield(true);
                    }

                    {
                        Debug.Log("Starting Homing Missiles");
                        _canShootAtPlayer = true;
                    }

                    Debug.Log("Resetting Position");

                    {
                        Debug.Log("Activating Death Wall Closest to player");
                        Debug.Log("Rotating to make sure death wall gets closer to player");
                        _wipeoutWallsManager.ActivateClosestLongWallAndRotate();
                    }
                    
                    
                    Debug.Log("------------------------");
                    
                    break;
                case DerekContextManager.Vulnerability.Vulnerable:
                    //End Missile Behavior
                    //Deactivate all DeathWalls
                    //Lower Shields
                    //Ensure Derek is susceptible to damage

                    {
                        Debug.Log("Stopping Missiles");
                        _canShootAtPlayer = false;
                    }


                    {
                        Debug.Log("Deactivating Death Walls");
                        _wipeoutWallsManager.DeactivateWallAndRotation();
                    }
                    
                    {
                        Debug.Log("Lowering Shields");
                        _shieldBehaviour.ToggleShield(false);
                    }
                    
                    Debug.Log("------------------------");
                    break;
            }
        }

        private void OnValidate()
        {
            if (_shieldBehaviour == null)
            {
                PanicHelper.Panic(new Exception($"{nameof(_shieldBehaviour)} is null on Derek Boss Manager"));
            }

            if (_missileLauncherBehaviours == null)
            {
                PanicHelper.Panic(new Exception($"{nameof(_missileLauncherBehaviours)} is null on Derek Boss Manager"));
            }

            if (_wipeoutWallsManager == null)
            {
                PanicHelper.Panic(new Exception($"{nameof(_wipeoutWallsManager)} is null on Derek Boss Manager"));
            }
        }

        private void Update()
        {
            if (!_canShootAtPlayer)
            {
                return;
            }

            if (_shootTimer > 0.0f)
            {
                _shootTimer -= Time.deltaTime;
                return;
            }
            
            foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
            {
                missileLauncher.ShootMissile();
            }

            _shootTimer = _timeBetweenMissileShots;
        }
        
        /*
         * Todo:
         *  - Add Reset Derek Position on 
         *  - Add corresponding reset functionality
         *  - Create interim DamageReceiverComponent for OnHit
         *    - Call onPhaseComplete
         *
         *  - Create Tasks for:
         *    - Adding Missile Timing and detection logic
         *    - Adding MissileLauncher Params method
         */
    }
}
