using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class DerekContextManager : ContextManager
    {
        [SerializeField, Tooltip("How long should the system wait before restarting the Labor after an incomplete Labor")]
        private float _LaborIncompleteTimer = 5.0f;
        
        private RingLaborManager _ringLaborManager;
        private DerekBossManager _bossManager;
        //private Interactable _TutorialEndButton;

        private WaitForSeconds _LaborIncompleteWaitForSeconds;
        
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

#if UNITY_EDITOR //Debug Buttons for WIP PR
        
        private RingLaborManager debug_rlm;
        private DerekBossManager debug_dbm;
        private Phase debug_currentPhase = Phase.Tutorial;
        private Vulnerability debug_currentVulnerability = Vulnerability.Invulnerable;
        
        private void Debug_Initialize()
        {
            debug_rlm = FindObjectOfType<RingLaborManager>();
            debug_dbm = FindObjectOfType<DerekBossManager>();
            
            Initialize(debug_rlm, debug_dbm);
        }

        [Button]
        private void Debug_Reset()
        {
            debug_currentPhase = Phase.Tutorial;
            debug_currentVulnerability = Vulnerability.Invulnerable;
        }

        [Button]
        private void Debug_AdvancePhase()
        {
            if (debug_rlm == null || debug_dbm == null)
            {
                Debug_Initialize();
            }

            debug_currentPhase++;
            debug_dbm.UpdatePhase(debug_currentPhase);
            
            if (debug_currentPhase == Phase.Death)
            {
                Debug_Reset();
            }
        }

        [Button]
        private void Debug_SwitchVulnerability()
        {
            if (debug_rlm == null || debug_dbm == null)
            {
                Debug_Initialize();
            }

            switch (debug_currentVulnerability)
            {
                case Vulnerability.Invulnerable:
                    debug_currentVulnerability = Vulnerability.Vulnerable;
                    break;
                case Vulnerability.Vulnerable:
                    debug_currentVulnerability = Vulnerability.Invulnerable;
                    break;
            }
            
            debug_dbm.UpdateVulnerability(debug_currentVulnerability);
        }
#endif
        
        
        public void Reset()
        {
            if (_ringLaborManager == null || _bossManager == null)
            {
                return;
            }
            
            _ringLaborManager.Reset();
            _bossManager.Reset();
            
            //TutorialEndButton.Bind(this, BeginEncounter); 
        }

        public void Initialize(RingLaborManager ringLaborManager, DerekBossManager bossManager)
        {
            _ringLaborManager = ringLaborManager;
            _bossManager = bossManager;
            
            //TutorialEndButton.Bind(this, OnPhaseChanged); 
            _LaborIncompleteWaitForSeconds = new WaitForSeconds(_LaborIncompleteTimer);
        }
        
        private void OnLaborCompleted(bool wasSuccessful)
        {
            if (_currentPhase == Phase.Tutorial || _currentVulnerability != Vulnerability.Invulnerable)
            {
                Perigon.Utility.Logger.LogError("Labor should not have have been active during the tutorial phase, or when the boss is not invulnerable. Exiting Encounter");
                return;
            }

            if (wasSuccessful)
            {
                _bossManager.UpdateVulnerability(Vulnerability.Vulnerable);
            }
            else
            {
                StartCoroutine(ResetLaborTimer());
            }
        }

        private void OnPhaseChanged()
        {
            switch (_currentPhase)
            {
                case Phase.Tutorial:
                    //TutorialEndButton.UnBind();
                    _currentPhase = Phase.FirstPhase;
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

            _ringLaborManager.Initialize(OnLaborCompleted);
            _bossManager.UpdatePhase(_currentPhase);
            _bossManager.UpdateVulnerability(Vulnerability.Invulnerable);
        }

        private IEnumerator ResetLaborTimer()
        {
            yield return _LaborIncompleteWaitForSeconds;

            if (_ringLaborManager != null)
            {
                _ringLaborManager.Reset();
            }
        }
    }
}
