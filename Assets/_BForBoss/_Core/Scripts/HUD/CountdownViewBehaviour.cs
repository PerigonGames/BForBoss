using System;
using Perigon.Utility;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class CountdownViewBehaviour : MonoBehaviour
    {
        [SerializeField, Resolve] private TMP_Text _countdownLabel;
        
        public void Reset()
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
            CountdownTimer.Instance.OnTimeUpdated += SetTimerLabel;
            CountdownTimer.Instance.OnTimeReset += Reset;
        }
        
        private void OnDisable()
        {
            CountdownTimer.Instance.OnTimeUpdated -= SetTimerLabel;
            CountdownTimer.Instance.OnTimeReset -= Reset;
        }

        private void Awake()
        {
            this.PanicIfNullObject(_countdownLabel, nameof(_countdownLabel));
        }
    }
}