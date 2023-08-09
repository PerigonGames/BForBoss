using System;
using BForBoss.Utility;
using UnityEngine;

namespace BForBoss.RingSystem
{
    public class RingBehaviour : MonoBehaviour
    {
        public Action<RingBehaviour> OnRingTriggered;
        
        [SerializeField] private int _label;
        [SerializeField] private GameObject _ringView;

        private TMPro.TMP_Text _labelTMP;
        private PlayerTriggerBehaviour _trigger;

        public void Activate()
        {
            _ringView.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            _ringView.gameObject.SetActive(false);
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
