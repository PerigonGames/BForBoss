using System;
using System.Collections;
using System.Collections.Generic;
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
        public float CurrentTime => _time;

        private static CountdownTimer _instance = null;
        
        private float _time;
        private bool _isRunning;

        private float _amountOfTime;

        public event Action OnTimerTick;
        public event Action OnTimerStarted;
        public event Action OnTimerStopped;
        
        private Action OnCountdownCompleted;

        public void StartCountdown(float amountOfTime, Action onCountdownCompleted = null)
        {
            _amountOfTime = amountOfTime;
            _time = amountOfTime;
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

        public void PauseCountdown()
        {
            _isRunning = false;
        }

        public void ResumeCountdown()
        {
            _isRunning = true;
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

            if (_time >= 0.0f)
            {
                _time -= Time.deltaTime;
                OnTimerTick?.Invoke();
            }
            else
            {
                CompleteCountdown();
            }
        }

        public void Reset()
        {
            _time = _amountOfTime;
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
