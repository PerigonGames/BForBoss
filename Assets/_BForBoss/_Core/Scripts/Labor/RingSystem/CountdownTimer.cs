using System;
using UnityEngine;

namespace BForBoss
{
    public class CountdownTimer
    {
        public static CountdownTimer Instance
        {
            get
            {
                _instance ??= new CountdownTimer();
                return _instance;
            }
        }
        
        public bool IsRunning => _isRunning;

        private static CountdownTimer _instance = null;
        
        private float _timer;
        private bool _isRunning;
        private float _delayedTimer;

        private float _amountOfTime;

        public event Action OnTimerStarted;
        public event Action OnTimerStopped;

        private float Timer
        {
            get => _timer;
            set
            {
                _timer = value;
                OnTimeUpdated?.Invoke(value);
            }
        }

        public event Action<float> OnTimeUpdated;

        private Action OnCountdownCompleted;

        public void StartCountdown(float amountOfTime, Action onCountdownCompleted = null, float delayedStartTime = 0)
        {
            _delayedTimer = delayedStartTime;
            _amountOfTime = amountOfTime;
            _timer = amountOfTime;
            _isRunning = true;
            OnCountdownCompleted = onCountdownCompleted;
            OnTimerStarted?.Invoke();
        }
        
        public void StopCountdown()
        {
            _isRunning = false;
            OnCountdownCompleted = null;
            OnTimerStopped?.Invoke();
        }
        
        public void ToggleCountdown()
        {
            _isRunning = !_isRunning;
        }
        
        public void Tick()
        {
            if (!_isRunning)
            {
                return;
            }

            if (_delayedTimer > 0)
            {
                _delayedTimer -= Time.deltaTime;
                return;
            }

            if (Timer >= 0.0f)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                CompleteCountdown();
            }
        }

        public void Reset()
        {
            _timer = _amountOfTime;
        }

        private void CompleteCountdown()
        {
            _isRunning = false;
            OnTimerStopped?.Invoke();
            OnCountdownCompleted?.Invoke();
            OnCountdownCompleted = null;
            Reset();
        }
    }
}
