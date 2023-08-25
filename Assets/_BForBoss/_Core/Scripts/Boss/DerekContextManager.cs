using BForBoss.RingSystem;
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
        [SerializeField] private ShootAtInteractiveButtonBehaviour _endTutorialButton;
        [SerializeField] private DerekBossManager _bossManager;
        [SerializeField] private RingLaborManager _ringLaborManager;

        //Phase Information
        [Title("Derek Phase Data")]
        [SerializeField, InlineEditor] private DerekPhaseDataSO _firstPhaseData;
        [SerializeField, InlineEditor] private DerekPhaseDataSO _secondPhaseData;
        [SerializeField, InlineEditor] private DerekPhaseDataSO _finalPhaseData;

        //RingLabors configurations
        [Title("Ring Configurations")] 
        [SerializeField]
        private RingGrouping _ringConfigurationsTutorial;
        [Header("Phase 1")]
        [SerializeField]
        private RingGrouping _ringConfigurationsFirstPhase;
        [Header("Phase 2")]
        [SerializeField]
        private RingGrouping _ringConfigurationsSecondPhase;
        
        private DerekPhaseDataSO _currentPhaseDataSO = null;

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
            Perigon.Utility.Logger.LogString("Tutorial Phase has ended", LoggerColor.Yellow, "derekboss");
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
                        Logger.LogString("Set Tutorial Ring system", key:"Labor");
                        _ringLaborManager.SetRings(_ringConfigurationsTutorial);
                        break;
                    case Phase.FirstPhase:
                        _currentPhase = Phase.SecondPhase;
                        _currentPhaseDataSO = _secondPhaseData;
                        _ringLaborManager.SetRings(_ringConfigurationsFirstPhase);
                        break;
                    case Phase.SecondPhase:
                        _currentPhase = Phase.FinalPhase;
                        _currentPhaseDataSO = _finalPhaseData;
                        _ringLaborManager.SetRings(_ringConfigurationsSecondPhase);                        
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

            _ringLaborManager.ActivateSystem();
            _currentVulnerability = Vulnerability.Invulnerable;
            _bossManager.UpdateVulnerability(_currentVulnerability);
        }

        public void HandleDeath()
        {
            Perigon.Utility.Logger.LogString("Player wins -> Start Defeat animation and then open thank you for playing Text box", LoggerColor.Green, "derekboss");
            StateManager.Instance.SetState(State.EndGame);
        }

        private void Awake()
        {
            // Managers
            this.PanicIfNullObject(_ringLaborManager, nameof(_ringLaborManager));
            this.PanicIfNullObject(_bossManager, nameof(_bossManager));
            
            // Data
            this.PanicIfNullObject(_firstPhaseData, nameof(_firstPhaseData));
            this.PanicIfNullObject(_secondPhaseData, nameof(_secondPhaseData));
            this.PanicIfNullObject(_finalPhaseData, nameof(_finalPhaseData));
            
            this.PanicIfNullObject(_ringConfigurationsTutorial, nameof(_ringConfigurationsTutorial));
            this.PanicIfNullObject(_ringConfigurationsFirstPhase, nameof(_ringConfigurationsFirstPhase));
            this.PanicIfNullObject(_ringConfigurationsSecondPhase, nameof(_ringConfigurationsSecondPhase));
            
            this.PanicIfNullOrEmptyList(_ringConfigurationsTutorial.GroupOfRings, nameof(_ringConfigurationsTutorial));
            this.PanicIfNullOrEmptyList(_ringConfigurationsFirstPhase.GroupOfRings, nameof(_ringConfigurationsTutorial));
            this.PanicIfNullOrEmptyList(_ringConfigurationsSecondPhase.GroupOfRings, nameof(_ringConfigurationsTutorial));
            
            // Misc
            this.PanicIfNullObject(_endTutorialButton, nameof(_endTutorialButton));
        }
    }
}
