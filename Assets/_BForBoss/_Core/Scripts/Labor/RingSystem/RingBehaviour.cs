using System;
using BForBoss.Utility;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class RingBehaviour : MonoBehaviour
    {
        public event Action<RingBehaviour> RingActivated;
        
        [SerializeField] private TMPro.TMP_Text[] _labels;
        [SerializeField] private string _label;

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
            if (string.IsNullOrEmpty(_label) || _labels.IsNullOrEmpty()) return;
            SetLabel(_label);
        }
    }
}
