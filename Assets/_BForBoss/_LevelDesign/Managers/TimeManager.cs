using UnityEngine;

namespace BForBoss
{
    public class TimeManager : MonoBehaviour
    {
        private float _currentGameTime = 0.0f;
        private bool _isTimerTracking = false;

        public float CurrentGameTime => _currentGameTime;

        public void Reset()
        {
            _currentGameTime = 0.0f;
        }

        public void StartTimer()
        {
            _isTimerTracking = true;
        }

        public void StopTimer()
        {
            _isTimerTracking = false;
        }
        
        private void FixedUpdate()
        {
            if (_isTimerTracking)
            {
                _currentGameTime += Time.fixedDeltaTime;
            }
        }
    }
}
