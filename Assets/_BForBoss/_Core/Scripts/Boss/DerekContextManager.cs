using BForBoss.RingSystem;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [DisallowMultipleComponent]
    public class DerekContextManager : MonoBehaviour
    {
        [SerializeField] private ShootAtInteractiveButtonBehaviour _endTutorialButton;
        [SerializeField] private DerekBossManager _bossManager;
        
        //Phase Information
        [Header("Derek Phase Data")]
        [SerializeField] private DerekPhaseDataSO _firstPhaseData;
        [SerializeField] private DerekPhaseDataSO _secondPhaseData;
        [SerializeField] private DerekPhaseDataSO _finalPhaseData;


        private RingLaborManager _ringLaborManager;
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
            _bossManager.Reset();
            _endTutorialButton.Reset();
        }

        public void Initialize(RingLaborManager ringLaborManager, IGetPlayerTransform playerMovementBehaviour)
        {
            _ringLaborManager = ringLaborManager;

            this.PanicIfNullObject(_ringLaborManager, nameof(_ringLaborManager));
            this.PanicIfNullObject(_firstPhaseData, nameof(_firstPhaseData));
            this.PanicIfNullObject(_secondPhaseData, nameof(_secondPhaseData));
            this.PanicIfNullObject(_finalPhaseData, nameof(_finalPhaseData));

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
                        break;
                    case Phase.FirstPhase:
                        _currentPhase = Phase.SecondPhase;
                        _currentPhaseDataSO = _secondPhaseData;
                        break;
                    case Phase.SecondPhase:
                        _currentPhase = Phase.FinalPhase;
                        _currentPhaseDataSO = _finalPhaseData;
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

            _ringLaborManager.ActivateSystem(OnLaborCompleted);
            _currentVulnerability = Vulnerability.Invulnerable;
            _bossManager.UpdateVulnerability(_currentVulnerability);
        }

        private void HandleDeath()
        {
            Perigon.Utility.Logger.LogString("Player wins -> Start Defeat animation and then open thank you for playing Text box", LoggerColor.Green, "derekboss");
        }

        private void Awake()
        {
            this.PanicIfNullObject(_bossManager, nameof(_bossManager));
            this.PanicIfNullObject(_endTutorialButton, nameof(_endTutorialButton));
        }
    }
}
