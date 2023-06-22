using System;
using BForBoss.RingSystem;
using Perigon.Utility;
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
    
        //Put this in Boss Manager
        public enum Vulnerability
        {
            Invulnerable, //Shields are up, impervious to damage
            Vulnerable, //Shields are down, susceptible to damage
        }

        private Phase _currentPhase = Phase.Tutorial;
        private Vulnerability _currentVulnerability = Vulnerability.Invulnerable;
        
        public void Reset()
        {
            //_ringLaborManager.Reset();
            _bossManager.Reset();
            _endTutorialButton.Reset();
        }

        public void Initialize(RingLaborManager ringLaborManager, PlayerMovementBehaviour playerMovementBehaviour)
        {
            _ringLaborManager = ringLaborManager;

            if (_ringLaborManager == null)
            {
                PanicHelper.Panic(new Exception("_ringLaborManager is null"));
                return;
            }
            
            _bossManager.Initialize(playerMovementBehaviour);
            _endTutorialButton.Initialize(OnEndTutorialButtonTriggered);
        }

        private void OnEndTutorialButtonTriggered()
        {
            Perigon.Utility.Logger.LogString("Tutorial Phase has ended", LoggerColor.Yellow, "derekboss");
            OnPhaseChanged();
        }
        
        private void OnLaborCompleted()
        {
            if (_currentPhase == Phase.Tutorial || _currentVulnerability != Vulnerability.Invulnerable)
            {
                Perigon.Utility.Logger.LogError("Labor should not have have been active during the tutorial phase, or when the boss is not invulnerable. Exiting Encounter", LoggerColor.Red, "derekboss");
                return;
            }
            
            _bossManager.UpdateVulnerability(Vulnerability.Vulnerable);
        }

        private void OnPhaseChanged()
        {
            switch (_currentPhase)
            {
                case Phase.Tutorial:
                    _currentPhase = Phase.FirstPhase;
                    _ringLaborManager.ActivateSystem(OnLaborCompleted);
                    break;
                case Phase.FirstPhase:
                    _currentPhase = Phase.SecondPhase;
                    break;
                case Phase.SecondPhase:
                    _currentPhase = Phase.FinalPhase;
                    break;
                case Phase.FinalPhase:
                    _currentPhase = Phase.Death;
                    break;
                case Phase.Death:
                    return;
                default:
                    break;
            }
            
            _bossManager.UpdatePhase(_currentPhase);
            _bossManager.UpdateVulnerability(Vulnerability.Invulnerable);
        }

        private void OnValidate()
        {
            if (_bossManager == null)
            {
                PanicHelper.Panic(new Exception($"{nameof(_bossManager)} is null from Derek Context Manager"));
            }

            if (_endTutorialButton == null)
            {
                PanicHelper.Panic(new Exception($"{nameof(_endTutorialButton)} is null from Derek Context Manager"));
            }
        }
    }
}
