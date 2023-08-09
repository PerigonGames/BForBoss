using System;
using System.Collections.Generic;
using BForBoss.Labor;

namespace BForBoss.RingSystem
{
    public class GroupedRingSystem : ILabor
    {
        public event Action<bool> OnLaborCompleted;
        private RingBehaviour _currentRing;
        private Queue<RingBehaviour> _ringQueue;
        private readonly RingBehaviour[] _allRings;

        private readonly float _time;
        
        public GroupedRingSystem(RingBehaviour[] rings, float time = 0f)
        {
            _allRings = rings;
            _time = time;
            SetupRings();
        }

        private void SetupRings()
        {
            for (var i = 0; i < _allRings.Length; i++)
            {
                _allRings[i].RingActivated = RingTriggered;
                _allRings[i].SetLabel((i + 1).ToString());
            }
        }

        public void Reset()
        {
            foreach (var ring in _allRings)
            {
                DeactivateRing(ring);
            }

            _currentRing = null;
            _ringQueue = new Queue<RingBehaviour>(_allRings);
        }

        public void Activate()
        {
            TrySetupNextRing();
            
            if (_time > 0f)
            {
                CountdownTimer.Instance.StartCountdown(_time, onCountdownCompleted: CountdownFinish);
            }
        }

        private void RingTriggered(RingBehaviour ring)
        {
            DeactivateRing(ring);
            TrySetupNextRing();
        }

        private void CountdownFinish()
        {
            Perigon.Utility.Logger.LogString("Ran out of time! Resetting the labor", key: "Labor");
            Reset();
            OnLaborCompleted?.Invoke(false);
        }

        private void DeactivateRing(RingBehaviour ring)
        {
            ring.gameObject.SetActive(false);
        }

        private void ActivateRing(RingBehaviour ring)
        {
            ring.gameObject.SetActive(true);
        }
        
        private void TrySetupNextRing()
        {
            if (_ringQueue.Count == 0)
            {
                InvokeLaborCompleted();
                return;
            }
            _currentRing = _ringQueue.Dequeue();
            if (!_currentRing.gameObject.activeInHierarchy)
            {
                ActivateRing(_currentRing);
            }
        }

        private void InvokeLaborCompleted()
        {
            if(_time > 0f)
                CountdownTimer.Instance.StopCountdown();
            OnLaborCompleted?.Invoke(true);
        }
    }
}