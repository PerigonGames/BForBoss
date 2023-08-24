using System;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class DerekBossManager : MonoBehaviour
    {
        private const string POWER_UP = "Power Up";
        private const string POWER_DOWN = "Power Down";

        private static readonly int POWER_UP_KEY = Animator.StringToHash(POWER_UP);
        private static readonly int POWER_DOWN_KEY = Animator.StringToHash(POWER_DOWN);

        [SerializeField, Resolve] private DerekHealthBehaviour _healthBehaviour;
        [SerializeField, Resolve] private DerekShieldBehaviour _shieldBehaviour;
        [SerializeField, Resolve] private BossWipeOutWallsManager _wipeoutWallsManager;
        [SerializeField, Resolve] private RotationalMovementBehaviour _rotationalMovementBehaviour;
        [SerializeField] private DerekMissileLauncherBehaviour[] _missileLauncherBehaviours;
        private float _vulnerabilityDuration = 10.0f;
        
        private Action<bool> _onVulnerabilityExpired;
        private DerekContextManager.Vulnerability _vulnerability = DerekContextManager.Vulnerability.Invulnerable;
        private float _vulnerabilityTimer;
        private Animator _animator;

        public void Reset()
        {
            _vulnerability = DerekContextManager.Vulnerability.Invulnerable;
            _wipeoutWallsManager.Reset();
            _animator.SetTrigger(POWER_DOWN_KEY);
            foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
            {
                missileLauncher.Reset();
            }
            _shieldBehaviour.ToggleShield(false);
            _healthBehaviour.Reset();
            _vulnerabilityTimer = 0;
        }

        public void Initialize(IGetPlayerTransform playerMovementBehaviour, Action<bool> onVulnerabilityExpired)
        {
            _wipeoutWallsManager.Initialize(playerMovementBehaviour);
            foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
            {
                missileLauncher.Initialize(playerMovementBehaviour);
            }
            _onVulnerabilityExpired = onVulnerabilityExpired;
            _healthBehaviour.Initialize(() => _onVulnerabilityExpired?.Invoke(true));
        }

        public void UpdatePhase(DerekContextManager.Phase phase, DerekPhaseDataSO phaseData)
        {
            switch (phase)
            {
                case DerekContextManager.Phase.FirstPhase:
                    Perigon.Utility.Logger.LogString($"Phase 1: Will transition at {phaseData.HealthThreshold * 100}% health", LoggerColor.Green, "derekboss");
                    break;
                case DerekContextManager.Phase.SecondPhase:
                    Perigon.Utility.Logger.LogString($"Phase 2: Will transition at {phaseData.HealthThreshold * 100}% health", LoggerColor.Green, "derekboss");
                    break;
                case DerekContextManager.Phase.FinalPhase:
                    Perigon.Utility.Logger.LogString($"Phase 3: Will transition at {phaseData.HealthThreshold * 100}% health", LoggerColor.Green, "derekboss");
                    break;
                case DerekContextManager.Phase.Tutorial:
                case DerekContextManager.Phase.Death:
                default:
                    return;
            }

            if (phaseData == null)
            {
                Perigon.Utility.Logger.LogError("Phase Data should not be invalid", LoggerColor.Red, "derekboss");
                return;
            }
            
            _healthBehaviour.SetThreshold(phaseData.HealthThreshold, phaseData.HealthBarColor);
            _rotationalMovementBehaviour.SetRotationRate(phaseData.RotationRate);
            _vulnerabilityDuration = phaseData.VulnerabilityDuration;
            foreach (DerekMissileLauncherBehaviour missileLauncher in _missileLauncherBehaviours)
            {
                missileLauncher.UpdateMissileSettings(phaseData.MissileSpeedMultiplier , phaseData.IntervalBetweenShots);
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
                        missileLauncher.StartShooting();
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
            _healthBehaviour.CanReceiveDamage = _vulnerability == DerekContextManager.Vulnerability.Vulnerable;
        }

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            this.PanicIfNullObject(_shieldBehaviour, nameof(_shieldBehaviour));
            this.PanicIfNullOrEmptyList(_missileLauncherBehaviours, nameof(_missileLauncherBehaviours));
            this.PanicIfNullObject(_wipeoutWallsManager, nameof(_wipeoutWallsManager));
            this.PanicIfNullObject(_rotationalMovementBehaviour, nameof(_rotationalMovementBehaviour));
            this.PanicIfNullObject(_animator, nameof(_animator));
            this.PanicIfNullObject(_healthBehaviour, nameof(_healthBehaviour));
        }
        
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
