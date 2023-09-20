using System;
using BForBoss.RingSystem;
using DG.Tweening;
using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = Perigon.Utility.Logger;

namespace BForBoss
{
    [DisallowMultipleComponent]
    public class DerekContextManager : MonoBehaviour
    {
        [SerializeField] private DerekBossManager _bossManager;
        [SerializeField] private RingLaborManager _ringLaborManager;

        [Header("Environment")]
        [SerializeField] private ShootAtInteractiveButtonBehaviour _endTutorialButton;
        [SerializeField] private Transform _floorTransform;
        [SerializeField, MinValue(0.0f), Tooltip("Time it takes for the floor to scale during phases")] private float _floorScaleTweenDuration = 6.0f;
        
        //Phase Information
        [Title("Derek Phase Data")]
        [SerializeField, InlineEditor] private DerekPhaseDataSO _firstPhaseData;
        [SerializeField, InlineEditor] private DerekPhaseDataSO _secondPhaseData;
        [SerializeField, InlineEditor] private DerekPhaseDataSO _finalPhaseData;

        //RingLabors configurations
        [Header("Ring Configurations")]
        
        [Header("Phase 1 - First Ring Labor")]
        [SerializeField]
        private RingGrouping _firstRingLaborConfiguration;
        [Header("Phase 1")]
        [SerializeField]
        private RingGrouping _ringConfigurationsFirstPhase;
        [Header("Phase 2")]
        [SerializeField]
        private RingGrouping _ringConfigurationsSecondPhase;
        [Header("Phase 3")]
        [SerializeField]
        private RingGrouping _ringConfigurationsFinalPhase;
        
        private DerekPhaseDataSO _currentPhaseDataSO = null;
        private Vector3 _originalFloorScale;

        private bool _hasCompletedFirstRingLabor = false;

        public enum Phase
        {
            Tutorial, // Battle hasn't begun just yet
            FirstPhase, // Health between 75% and 100%
            SecondPhase, // Health between 25% and 75%
            FinalPhase, // Health between 0% and 25%
            Death, // Derek is dead
        }

        public enum Vulnerability
        {
            Invulnerable, //Shields are up, impervious to damage
            Vulnerable, //Shields are down, susceptible to damage
        }

        private Phase _currentPhase = Phase.Tutorial;
        private Vulnerability _currentVulnerability = Vulnerability.Invulnerable;

        public void Reset()
        {
            _floorTransform.localScale = _originalFloorScale;
            _ringLaborManager.Reset();
            _currentPhase = Phase.Tutorial;
            _currentVulnerability = Vulnerability.Invulnerable;
            _bossManager.Reset();
            _endTutorialButton.Reset();
        }

        public void Initialize(IGetPlayerTransform playerMovementBehaviour)
        {
            _ringLaborManager.OnLaborCompleted = OnLaborCompleted;
            _ringLaborManager.Initialize();
            _bossManager.Initialize(playerMovementBehaviour, HandleVulnerabilityExpiring);
            _endTutorialButton.Initialize(OnEndTutorialButtonTriggered);
        }

        private void OnEndTutorialButtonTriggered()
        {
            Logger.LogString("Tutorial Phase has ended", LoggerColor.Yellow, "derekboss");
            HandleVulnerabilityExpiring(true);
        }

        private void OnLaborCompleted()
        {
            if (_currentPhase == Phase.Tutorial || _currentVulnerability != Vulnerability.Invulnerable)
            {
                Perigon.Utility.Logger.LogError("Labor should not have have been active during the tutorial phase, or when the boss is not invulnerable. Exiting Encounter", LoggerColor.Red, "derekboss");
                return;
            }
            
            _ringLaborManager.Reset();
            
            if (_currentPhase == Phase.FirstPhase && !_hasCompletedFirstRingLabor)
            {
                _ringLaborManager.SetRings(_ringConfigurationsFirstPhase);
                _hasCompletedFirstRingLabor = true;
            }
            
            _currentVulnerability = Vulnerability.Vulnerable;
            _bossManager.UpdateVulnerability(_currentVulnerability);
        }

        private void HandleVulnerabilityExpiring(bool wasPhaseCompleted)
        {
            if (wasPhaseCompleted)
            {
                switch (_currentPhase)
                {
                    case Phase.Tutorial:
                        _currentPhase = Phase.FirstPhase;
                        _currentPhaseDataSO = _firstPhaseData;
                        Logger.LogString("Set First Phase Ring system", key:"Labor");
                        _ringLaborManager.SetRings(_firstRingLaborConfiguration);
                        break;
                    case Phase.FirstPhase:
                        _currentPhase = Phase.SecondPhase;
                        _currentPhaseDataSO = _secondPhaseData;
                        _ringLaborManager.SetRings(_ringConfigurationsSecondPhase);
                        break;
                    case Phase.SecondPhase:
                        _currentPhase = Phase.FinalPhase;
                        _currentPhaseDataSO = _finalPhaseData;
                        _ringLaborManager.SetRings(_ringConfigurationsFinalPhase);                        
                        break;
                    case Phase.FinalPhase:
                        _currentPhase = Phase.Death;
                        HandleDeath();
                        return;
                    default:
                        return;
                }

                _bossManager.UpdatePhase(_currentPhase, _currentPhaseDataSO);
            }

            float floorScale = _currentPhaseDataSO.FloorSizeScale;
            DOTween.To(() => _floorTransform.localScale, floorScaleVector => _floorTransform.localScale = floorScaleVector, 
                new Vector3(floorScale * _originalFloorScale.x, _originalFloorScale.y, floorScale * _originalFloorScale.z),
                _floorScaleTweenDuration);
            
            _ringLaborManager.ActivateSystem();
            _currentVulnerability = Vulnerability.Invulnerable;
            _bossManager.UpdateVulnerability(_currentVulnerability);
        }

        public void HandleDeath()
        {
            Logger.LogString("Player wins -> Start Defeat animation and then open thank you for playing Text box", LoggerColor.Green, "derekboss");
            StateManager.Instance.SetState(State.EndGame);
        }

        private void Awake()
        {
            // Managers
            this.PanicIfNullObject(_ringLaborManager, nameof(_ringLaborManager));
            this.PanicIfNullObject(_bossManager, nameof(_bossManager));
            
            if (_floorTransform.gameObject == null)
            {
                PanicHelper.Panic(new Exception("Floor object has not been set"));
            }
            _originalFloorScale = _floorTransform.localScale;

            // Data
            this.PanicIfNullObject(_firstPhaseData, nameof(_firstPhaseData));
            this.PanicIfNullObject(_secondPhaseData, nameof(_secondPhaseData));
            this.PanicIfNullObject(_finalPhaseData, nameof(_finalPhaseData));
            
            //Ring Systems
            this.PanicIfNullObject(_firstRingLaborConfiguration, nameof(_firstRingLaborConfiguration));
            this.PanicIfNullObject(_ringConfigurationsFirstPhase, nameof(_ringConfigurationsFirstPhase));
            this.PanicIfNullObject(_ringConfigurationsSecondPhase, nameof(_ringConfigurationsSecondPhase));
            this.PanicIfNullObject(_ringConfigurationsFinalPhase, nameof(_ringConfigurationsFinalPhase));
            
            if (_firstRingLaborConfiguration.GroupOfRings == null || _firstRingLaborConfiguration.GroupOfRings.Count != 1)
            {
                Exception exception = new Exception($"{nameof(_firstRingLaborConfiguration)} is null or does not have exactly 1 GroupOfRings {typeof(DerekContextManager).ToString()}");
                PanicHelper.Panic(exception);
            }

            this.PanicIfNullOrEmptyList(_ringConfigurationsFirstPhase.GroupOfRings, nameof(_ringConfigurationsFirstPhase));
            this.PanicIfNullOrEmptyList(_ringConfigurationsSecondPhase.GroupOfRings, nameof(_ringConfigurationsFirstPhase));
            this.PanicIfNullOrEmptyList(_ringConfigurationsFinalPhase.GroupOfRings, nameof(_ringConfigurationsFirstPhase));
            
            // Misc
            this.PanicIfNullObject(_endTutorialButton, nameof(_endTutorialButton));
        }
    }
}
