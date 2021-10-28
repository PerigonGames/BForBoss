using System;
using UnityEngine;

namespace BForBoss
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] private Checkpoint _startingCheckpoint = null;
        [SerializeField] private Checkpoint _endingCheckpoint = null;
        private TimeManagerViewModel _timeManagerViewModel = null;

        public void Initialize(TimeManagerViewModel viewModel)
        {
            _timeManagerViewModel = viewModel;
        }

        public void Reset()
        {
            _timeManagerViewModel?.Reset();
        }

        #region Mono
        private void Awake()
        {
            _startingCheckpoint.OnEnterArea += HandleOnEnterStartingCheckpoint;
            _endingCheckpoint.OnEnterArea += HandleOnEnterEndingCheckpoint;
        }

        private void HandleOnEnterStartingCheckpoint(Checkpoint _)
        {
            _timeManagerViewModel.StartTimer();
        }

        private void HandleOnEnterEndingCheckpoint(Checkpoint _)
        {
            _timeManagerViewModel.StopTimer();
        }

        private void OnDestroy()
        {
            _startingCheckpoint.OnEnterArea -= HandleOnEnterStartingCheckpoint;
            _endingCheckpoint.OnEnterArea -= HandleOnEnterEndingCheckpoint;
        }

        private void Update()
        {
            _timeManagerViewModel?.Update(Time.deltaTime);
        }
        #endregion
    }

    public class TimeManagerViewModel
    {
        private float _currentGameTime = 0.0f;
        private bool _isTimerTracking = false;

        public float CurrentGameTime
        {
            get => _currentGameTime;
            set
            {
                _currentGameTime = value;
                OnTimeChanged?.Invoke(value);
            }
        }

        public int CurrentGameTimeMilliSeconds => Mathf.CeilToInt(_currentGameTime * 1000);

        public event Action<float> OnTimeChanged;

        public void Reset()
        {
            _currentGameTime = 0f;
            _isTimerTracking = false;
        }
        
        public void StartTimer()
        {
            _isTimerTracking = true;
        }
        
        public void StopTimer()
        {
            _isTimerTracking = false;
        }

        public void Update(float time)
        {
            if (_isTimerTracking)
            {
                CurrentGameTime += time;
            }
        }
    }
}
