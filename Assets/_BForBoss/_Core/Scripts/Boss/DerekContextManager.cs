using System;
using BForBoss.RingSystem;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [DisallowMultipleComponent]
    public class DerekContextManager : MonoBehaviour
    {
        private RingLaborManager _ringLaborManager;
        private DerekBossManager _bossManager;
        //private Interactable _TutorialEndButton;
        
        public enum Phase
        {
            Tutorial, // Battle hasn't begun just yet
            FirstPhase, // Health between 75% and 100%
            SecondPhase, // Health between 25% and 75%
            FinalPhase, // Health between 0% and 25%
            Death, // Derek is dead
        }

#if UNITY_EDITOR //Debug Setup
        [Button]
        public void Debug_FirstPhase()
        {
            Initialize(FindObjectOfType<RingLaborManager>(), FindObjectOfType<DerekBossManager>());
            OnPhaseChanged();
        }
#endif
    
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
            _ringLaborManager.Reset();
            _bossManager.Reset();
            
            //TutorialEndButton.Bind(this, BeginEncounter); 
        }

        public void Initialize(RingLaborManager ringLaborManager, DerekBossManager bossManager)
        {
            _ringLaborManager = ringLaborManager;
            _bossManager = bossManager;
            
            if (_ringLaborManager == null)
            {
                PanicHelper.Panic(new Exception("_ringLaborManager is null"));
            }
            
            if (_bossManager == null)
            {
                PanicHelper.Panic(new Exception("_bossManager is null"));
            }
            
            //TutorialEndButton.Bind(this, OnPhaseChanged);
        }
        
        private void OnLaborCompleted()
        {
            if (_currentPhase == Phase.Tutorial || _currentVulnerability != Vulnerability.Invulnerable)
            {
                Perigon.Utility.Logger.LogError("Labor should not have have been active during the tutorial phase, or when the boss is not invulnerable. Exiting Encounter");
                return;
            }
            
            _bossManager.UpdateVulnerability(Vulnerability.Vulnerable);
        }

        private void OnPhaseChanged()
        {
            switch (_currentPhase)
            {
                case Phase.Tutorial:
                    //TutorialEndButton.UnBind();
                    _currentPhase = Phase.FirstPhase;
                    _ringLaborManager.Initialize(OnLaborCompleted);
                    break;
                case Phase.FirstPhase:
                    _currentPhase = Phase.SecondPhase;
                    break;
                case Phase.SecondPhase:
                    _currentPhase = Phase.FinalPhase;
                    break;
                case Phase.Death:
                    return;
                default:
                    break;
            }

            _ringLaborManager.Reset();
            _bossManager.UpdatePhase(_currentPhase);
            _bossManager.UpdateVulnerability(Vulnerability.Invulnerable);
        }
    }
}
