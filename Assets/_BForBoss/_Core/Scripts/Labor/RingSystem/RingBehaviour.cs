using System;
using System.Collections;
using BForBoss.Utility;
using Perigon.Utility;
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
        [SerializeField] private Renderer _renderer;
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

        public void SetLabel(string label)
        {
            _labelTMP.text = label;
        }

        public void SetColor(Color color)
        {
            _renderer.material.color = color;
        }

        private void Awake()
        {
            _labelTMP = GetComponentInChildren<TMPro.TMP_Text>();
            _trigger = GetComponentInChildren<PlayerTriggerBehaviour>();
            _labelTMP = GetComponentInChildren<TMPro.TMP_Text>();
            this.PanicIfNullObject(_renderer, "Renderer");
            this.PanicIfNullObject(_trigger, "PlayerTriggerBehaviour");
            this.PanicIfNullObject(_labelTMP, "TMP_Text");
            SetLabel(_label.ToString());
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
    }
}
