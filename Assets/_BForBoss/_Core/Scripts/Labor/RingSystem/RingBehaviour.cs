using System;
using System.Collections;
using BForBoss.Utility;
using UnityEngine;
using Logger = Perigon.Utility.Logger;

namespace BForBoss.RingSystem
{
    public class RingBehaviour : MonoBehaviour
    {
        public Action<RingBehaviour> OnRingTriggered;
        public bool IsDelayedStart { get; set; }
        
        [SerializeField] private int _label;
        [SerializeField] private GameObject _ringView;

        private TMPro.TMP_Text _labelTMP;
        private PlayerTriggerBehaviour _trigger;
        private float _penaltyDelayedStartTime;

        public void Initialize(float penaltyDelayedStartTime)
        {
            _penaltyDelayedStartTime = penaltyDelayedStartTime;
        }

        public void Activate()
        {
            StartCoroutine(ActivateRingCoroutine());
        }

        public void Deactivate()
        {
            _ringView.gameObject.SetActive(false);
        }

        private IEnumerator ActivateRingCoroutine()
        {
            if (IsDelayedStart)
            {
                Logger.LogString("Delayed Start to Activate Ring", key: "Labor");
                yield return new WaitForSeconds(_penaltyDelayedStartTime);
            }
            _ringView.gameObject.SetActive(true);
        }
        
        private void Awake()
        {
            _labelTMP = GetComponentInChildren<TMPro.TMP_Text>();
            _trigger = GetComponentInChildren<PlayerTriggerBehaviour>();
        }

        private void OnEnable()
        {
            _trigger.PlayerEnteredTrigger += OnPlayerEnteredTrigger;
        }
        
        private void OnDisable()
        {
            _trigger.PlayerEnteredTrigger -= OnPlayerEnteredTrigger;
        }

        private void OnPlayerEnteredTrigger()
        {
            OnRingTriggered?.Invoke(this);
        }

        public void SetLabel(string label)
        {
            _labelTMP.text = label;
        }

        private void OnValidate()
        {
            if (_labelTMP == null)
                _labelTMP = GetComponentInChildren<TMPro.TMP_Text>();
            SetLabel(_label.ToString());
        }
    }
}
