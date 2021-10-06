using System;
using UnityEngine;

namespace BForBoss
{
    public class TimeManager : MonoBehaviour
    {
        private static TimeManager m_instance;
        private float m_currentGameTime = 0.0f;
        private bool m_isTimerTracking = false;

        public static TimeManager Instance => m_instance;
        public float CurrentGameTime => m_currentGameTime;

        public void Reset()
        {
            m_currentGameTime = 0.0f;
        }

        public void StartTimer()
        {
            Reset();
            m_isTimerTracking = true;
        }

        public void StopTimer()
        {
            m_isTimerTracking = false;
        }
        
        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            
            DontDestroyOnLoad(this);
        }

        private void FixedUpdate()
        {
            if (m_isTimerTracking)
            {
                m_currentGameTime += Time.fixedDeltaTime;
            }
        }
    }
}
