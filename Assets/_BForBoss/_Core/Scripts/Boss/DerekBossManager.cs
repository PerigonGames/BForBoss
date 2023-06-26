using System;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class DerekBossManager : MonoBehaviour, IBulletCollision
    {
        [SerializeField, Resolve] private DerekShieldBehaviour _shieldBehaviour;
        [SerializeField, Resolve] private BossWipeOutWallsManager _wipeoutWallsManager;
        [SerializeField] private DerekMissileLauncherBehaviour[] _missileLauncherBehaviours;
        [SerializeField, Tooltip("Temporary timer to space out Missile shots for testing purposes"), Min(0.0f)] private float _timeBetweenMissileShots = 5.0f;
        [SerializeField, Tooltip("Temporary timer to track how long Derek has been in its vulnerable state"), Min(0.0f)] private float _vulnerabilityDuration = 10.0f; 
        

        private Action<bool> _onVulnerabilityExpired;
        private bool _isVulnerable = false;
        private float _vulnerabilityTimer;
        
        public void Reset()
        {
            _isVulnerable = false;
            _wipeoutWallsManager.Reset();
            foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
            {
                missileLauncher.Reset();
            }
            _shieldBehaviour.ToggleShield(false);
        }

        public void Initialize(PlayerMovementBehaviour playerMovementBehaviour, Action<bool> onVulnerabilityExpired)
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

            _onVulnerabilityExpired = onVulnerabilityExpired;
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
                    Perigon.Utility.Logger.LogString("Phase 1: Will transition at 75% health", LoggerColor.Green, "derekboss");
                    break;
                case DerekContextManager.Phase.SecondPhase:
                    //_healthBarViewBehavior.SetPhaseBoundary(0.25f, _onPhaseComplete.Invoke());
                    //_derekMissileBehavior.UpdateParameters(AdjustedParameters[])
                    Perigon.Utility.Logger.LogString("Phase 2: Will transition at 25% health", LoggerColor.Green, "derekboss");
                    break;
                case DerekContextManager.Phase.FinalPhase:
                    //_healthBarViewBehavior.SetPhaseBoundary(0f, _onPhaseComplete.Invoke());
                    //_derekMissileBehavior.UpdateParameters(AdjustedParameters[])
                    Perigon.Utility.Logger.LogString("Phase 3: Will transition at 0% health", LoggerColor.Green, "derekboss");
                    break;
                default:
                    return;
            }
        }

        public void UpdateVulnerability(DerekContextManager.Vulnerability vulnerability)
        {
            switch (vulnerability)
            {
                //Raise Shields
                //Start Missile Launching
                //Find Closest Death Wall and activate - 
                //Start Movement with new given rotation (i.e. clockwise/anticlockwise)
                case DerekContextManager.Vulnerability.Invulnerable:
                    _isVulnerable = false;
                    _shieldBehaviour.ToggleShield(true);
                    foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
                    {
                        missileLauncher.StartShooting(_timeBetweenMissileShots);
                    }
                    _wipeoutWallsManager.ActivateClosestLongWallAndRotate();
                    break;
                //End Missile Behavior
                //Deactivate all DeathWalls
                //Lower Shields
                //Ensure Derek is susceptible to damage
                case DerekContextManager.Vulnerability.Vulnerable:
                    _isVulnerable = true;
                    _vulnerabilityTimer = _vulnerabilityDuration;
                    foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
                    {
                        missileLauncher.StopShooting();
                    }
                    _wipeoutWallsManager.DeactivateWallAndRotation();
                    _shieldBehaviour.ToggleShield(false);
                    break;
            }
        }
        
        //Temporary Damage Receiving Functionality - Will be removed in BFB-516 with Derek Health Behaviour component
        public void OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            if (_shieldBehaviour.IsActive())
            {
                Perigon.Utility.Logger.LogWarning("Boss was shot even with shields up, which shouldn't be possible, ignoring damage", LoggerColor.Yellow, "derekboss");
                return;
            }
            
            _onVulnerabilityExpired?.Invoke(true);
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
            if (!_isVulnerable)
            {
                return;
            }

            if (_vulnerabilityTimer >= 0.0f)
            {
                _vulnerabilityTimer -= Time.deltaTime;
                return;
            }

            _vulnerabilityTimer = _vulnerabilityDuration;
            _onVulnerabilityExpired?.Invoke(false);
        }
    }
}
