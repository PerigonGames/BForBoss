using System;
using BForBoss.Utility;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class RingBehaviour : MonoBehaviour
    {
        public event Action<RingBehaviour> RingActivated;
        
        [SerializeField] private string _label;
        
        private TMPro.TMP_Text[] _labels;
        private PlayerTriggerBehaviour _trigger;

        private void Awake()
        {
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
            RingActivated?.Invoke(this);
        }

        private void SetLabel(string label)
        {
            _label = label;
            foreach (var tmp in _labels)
            {
                tmp.text = _label;
            }
        }

        private void OnValidate()
        {
            if (_labels.IsNullOrEmpty()) _labels = GetComponentsInChildren<TMPro.TMP_Text>();
            if (string.IsNullOrEmpty(_label) || _labels.IsNullOrEmpty()) return;
            SetLabel(_label);
        }
    }
}
