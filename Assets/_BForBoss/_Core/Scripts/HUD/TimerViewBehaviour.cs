using System;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class TimerViewBehaviour : MonoBehaviour
    {
        private TMP_Text _timerLabel;
        private float _time;
        private bool _isRunning;

        public void StartTimer()
        {
            _isRunning = true;
        }

        public void PauseTimer()
        {
            _isRunning = false;
        }

        public void Reset()
        {
            _time = 0;
            _isRunning = true;
            _timerLabel.text = string.Empty;
        }

        private void Update()
        {
            if (_isRunning)
            {
                _time += Time.deltaTime;
                SetTimerLabel(_time);
            }
        }

        private void SetTimerLabel(float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            string str = time .ToString(@"mm\:ss\:fff");
            _timerLabel.text = str;
        }
 
        private void Awake()
        {
            #if !(UNITY_EDITOR || DEVELOPMENT_BUILD)
                Destroy(gameObject);
            #endif
            _timerLabel = GetComponentInChildren<TMP_Text>();
        }
    }
}
