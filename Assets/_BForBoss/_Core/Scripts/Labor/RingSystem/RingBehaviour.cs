using System;
using BForBoss.Utility;
using UnityEngine;

namespace BForBoss.RingSystem
{
    public class RingBehaviour : MonoBehaviour
    {
        public Action<RingBehaviour> RingActivated;
        
        [SerializeField] private int _label;
        [SerializeField] private Renderer _renderer;

        private TMPro.TMP_Text _labelTMP;
        private PlayerTriggerBehaviour _trigger;

        public void SetLabel(string label)
        {
            _labelTMP.text = label;
        }

        public void SetColor(Color color)
        {
            if (_renderer == null) return;
            _renderer.material.color = color;
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
            RingActivated?.Invoke(this);
        }

        private void OnValidate()
        {
            if (_labelTMP == null)
                _labelTMP = GetComponentInChildren<TMPro.TMP_Text>();
            SetLabel(_label.ToString());
        }
    }
}
