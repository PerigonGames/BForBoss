using System;
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
        private RingLaborManager _ringLaborManager;

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

            if (_ringLaborManager == null)
            {
                this.PanicIfNullObject(_ringLaborManager, nameof(_ringLaborManager));
                return;
            }
            
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
                        break;
                    case Phase.FirstPhase:
                        _currentPhase = Phase.SecondPhase;
                        break;
                    case Phase.SecondPhase:
                        _currentPhase = Phase.FinalPhase;
                        break;
                    case Phase.FinalPhase:
                        _currentPhase = Phase.Death;
                        HandleDeath();
                        return;
                    default:
                        return;
                }
                
                _bossManager.UpdatePhase(_currentPhase);
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
