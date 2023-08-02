using System;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class DerekBossManager : MonoBehaviour, IBulletCollision
    {
        private const string POWER_UP = "Power Up";
        private const string POWER_DOWN = "Power Down";

        private static readonly int POWER_UP_KEY = Animator.StringToHash(POWER_UP);
        private static readonly int POWER_DOWN_KEY = Animator.StringToHash(POWER_DOWN);
        
        [SerializeField, Resolve] private DerekShieldBehaviour _shieldBehaviour;
        [SerializeField, Resolve] private BossWipeOutWallsManager _wipeoutWallsManager;
        [SerializeField] private DerekMissileLauncherBehaviour[] _missileLauncherBehaviours;
        [SerializeField, Tooltip("Temporary timer to space out Missile shots for testing purposes"), Min(0.0f)] private float _timeBetweenMissileShots = 5.0f;
        [SerializeField, Tooltip("Temporary timer to track how long Derek has been in its vulnerable state"), Min(0.0f)] private float _vulnerabilityDuration = 10.0f;

        //Todo: Temporary variables used to simulate receiving and responding to damage. Will Remove once DerekHealthBehavior has been implemented
        private Action<bool> _onVulnerabilityExpired;
        private DerekContextManager.Vulnerability _vulnerability = DerekContextManager.Vulnerability.Invulnerable;
        private float _vulnerabilityTimer;
        private Animator _animator;

        public void Reset()
        {
            _vulnerability = DerekContextManager.Vulnerability.Invulnerable;
            _wipeoutWallsManager.Reset();
            foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
            {
                missileLauncher.Reset();
            }
            _shieldBehaviour.ToggleShield(false);
        }

        public void Initialize(IGetPlayerTransform playerMovementBehaviour, Action<bool> onVulnerabilityExpired)
        {
            _wipeoutWallsManager.Initialize(playerMovementBehaviour);
            foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
            {
                missileLauncher.Initialize(playerMovementBehaviour);
            }
            _onVulnerabilityExpired = onVulnerabilityExpired;
        }

        public void UpdatePhase(DerekContextManager.Phase phase, DerekPhaseDataSO phaseData)
        {
            switch (phase)
            {
                case DerekContextManager.Phase.Tutorial:
                case DerekContextManager.Phase.Death:
                    break;
                case DerekContextManager.Phase.FirstPhase when phaseData != null:
                    //_healthBarViewBehavior.SetPhaseBoundary(0.75f, _onPhaseComplete.Invoke());
                    //_derekMissileBehavior.UpdateParameters(AdjustedParameters[])
                    Perigon.Utility.Logger.LogString("Phase 1: Will transition at 75% health", LoggerColor.Green, "derekboss");
                    break;
                case DerekContextManager.Phase.SecondPhase when phaseData != null:
                    //_healthBarViewBehavior.SetPhaseBoundary(0.25f, _onPhaseComplete.Invoke());
                    //_derekMissileBehavior.UpdateParameters(AdjustedParameters[])
                    Perigon.Utility.Logger.LogString("Phase 2: Will transition at 25% health", LoggerColor.Green, "derekboss");
                    break;
                case DerekContextManager.Phase.FinalPhase when phaseData != null:
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

                    _shieldBehaviour.ToggleShield(true);
                    foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
                    {
                        missileLauncher.StartShooting(_timeBetweenMissileShots);
                    }
                    _wipeoutWallsManager.ActivateClosestLongWallAndRotate();
                    _animator.SetTrigger(POWER_UP_KEY);
                    break;
                //End Missile Behavior
                //Deactivate all DeathWalls
                //Lower Shields
                //Ensure Derek is susceptible to damage
                case DerekContextManager.Vulnerability.Vulnerable:
                    _vulnerabilityTimer = _vulnerabilityDuration;
                    foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
                    {
                        missileLauncher.StopShooting();
                    }
                    _wipeoutWallsManager.DeactivateWallAndRotation();
                    _shieldBehaviour.ToggleShield(false);
                    _animator.SetTrigger(POWER_DOWN_KEY);
                    break;
            }

            _vulnerability = vulnerability;
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

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            this.PanicIfNullObject(_shieldBehaviour, nameof(_shieldBehaviour));
            this.PanicIfNullOrEmptyList(_missileLauncherBehaviours, nameof(_missileLauncherBehaviours));

            for (int i = 0; i < _missileLauncherBehaviours.Length; i++)
            {
                if (_missileLauncherBehaviours[i] == null)
                {
                    PanicHelper.Panic(new Exception($"Element number {i} of {nameof(_missileLauncherBehaviours)} is null on Derek Boss Manager"));
                }
            }
            
            this.PanicIfNullObject(_wipeoutWallsManager, nameof(_wipeoutWallsManager));
        }

        //TODO: Remove temp damage receiving component once DerekHealthBehaviour is implemented
        private void Update()
        {
            if (_vulnerability != DerekContextManager.Vulnerability.Vulnerable)
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
