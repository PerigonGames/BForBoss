using System;
using Perigon.Utility;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class CountdownViewBehaviour : MonoBehaviour
    {
        [SerializeField, Resolve] private TMP_Text _countdownLabel;
        private float _time;
        private bool _isRunning;

        private float _amountOfTime;

        public void StartCountdown(float amountOfTime)
        {
            _amountOfTime = amountOfTime;
            _time = amountOfTime;
            gameObject.SetActive(true);
            _isRunning = true;
        }

        public void PauseCountdown()
        {
            _isRunning = false;
        }

        public void Reset()
        {
            _time = _amountOfTime;
            SetTimerLabel(_amountOfTime);
        }

        private void CompleteCountdown()
        {
            _isRunning = false;
            SetTimerLabel(_time);
            gameObject.SetActive(false);
            Reset();
        }

        private void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            if (_time >= 0.0f)
            {
                _time -= Time.deltaTime;
                SetTimerLabel(_time);
            }
            else
            {
                CompleteCountdown();
            }
        }

        private void SetTimerLabel(float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            string str = time .ToString(@"mm\:ss");
            _countdownLabel.text = str;
        }
 
        private void Awake()
        {
            if (_countdownLabel == null)
            {
                PanicHelper.Panic(new Exception($"{nameof(_countdownLabel)} has not been set"));
            }
        }
    }
}