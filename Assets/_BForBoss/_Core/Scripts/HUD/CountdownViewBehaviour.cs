using System;
using Perigon.Utility;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class CountdownViewBehaviour : MonoBehaviour
    {
        [SerializeField, Resolve] private TMP_Text _countdownLabel;

        public void OnStopCountdown()
        {
            gameObject.SetActive(false);
        }

        public void PauseCountdown()
        {
            CountdownTimer.Instance.PauseCountdown();
        }
        
        public void ResumeCountdown()
        {
            CountdownTimer.Instance.ResumeCountdown();
        }

        public void Reset()
        {
            CountdownTimer.Instance.Reset();
            SetTimerLabel(CountdownTimer.Instance.CurrentTime);
        }

        private void OnCompleteCountdown()
        {
            SetTimerLabel(CountdownTimer.Instance.CurrentTime);
            gameObject.SetActive(false);
        }
        
        private void OnStartCountdown()
        {
            SetTimerLabel(CountdownTimer.Instance.CurrentTime);
            gameObject.SetActive(true);
        }

        private void Update()
        {
            CountdownTimer.Instance.Tick();
            SetTimerLabel(CountdownTimer.Instance.CurrentTime);
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
        }
        
        private void OnDisable()
        {
            CountdownTimer.Instance.OnTimerStarted += OnStartCountdown;
            CountdownTimer.Instance.OnTimerStopped += OnStopCountdown;
        }

        private void Awake()
        {
            this.PanicIfNullObject(_countdownLabel, nameof(_countdownLabel));
        }
    }
}