using System;
using Perigon.Utility;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class CountdownViewBehaviour : MonoBehaviour
    {
        [SerializeField, Resolve] private TMP_Text _countdownLabel;

        private void OnStopCountdown()
        {
            _countdownLabel.text = string.Empty;
        }

        public void Reset()
        {
            CountdownTimer.Instance.Reset();
            _countdownLabel.text = string.Empty;
        }
        
        private void OnStartCountdown()
        {
            _countdownLabel.text = string.Empty;
        }
        
        private void Update()
        {
            CountdownTimer.Instance.Tick();
        }

        private void SetTimerLabel(float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            string str = time .ToString(@"mm\:ss");
            _countdownLabel.text = str;
        }

        private void OnEnable()
        {
            CountdownTimer.Instance.OnTimerStarted += OnStartCountdown;
            CountdownTimer.Instance.OnTimerStopped += OnStopCountdown;
            CountdownTimer.Instance.OnTimeUpdated += SetTimerLabel;
        }
        
        private void OnDisable()
        {
            CountdownTimer.Instance.OnTimerStarted -= OnStartCountdown;
            CountdownTimer.Instance.OnTimerStopped -= OnStopCountdown;
            CountdownTimer.Instance.OnTimeUpdated -= SetTimerLabel;
        }

        private void Awake()
        {
            this.PanicIfNullObject(_countdownLabel, nameof(_countdownLabel));
        }
    }
}