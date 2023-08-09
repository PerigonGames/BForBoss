using System;
using System.Collections.Generic;
using BForBoss.Labor;

namespace BForBoss.RingSystem
{
    public class GroupedRingSystem : ILabor
    {
        public event Action<bool> OnLaborCompleted;
        
        private readonly RingBehaviour[] _allRings;
        private readonly float _timeToCompleteSystem;
        private RingBehaviour _currentRing;
        private Queue<RingBehaviour> _ringQueue;
        
        public GroupedRingSystem(RingBehaviour[] rings, float timeToCompleteSystem = 0f)
        {
            _allRings = rings;
            _timeToCompleteSystem = timeToCompleteSystem;
            SetupRings();
        }

        private void SetupRings()
        {
            for (var i = 0; i < _allRings.Length; i++)
            {
                _allRings[i].OnRingTriggered = RingTriggered;
                _allRings[i].SetLabel((i + 1).ToString());
            }
        }

        public void Reset()
        {
            foreach (var ring in _allRings)
            {
                ring.Deactivate();
            }

            _currentRing = null;
            _ringQueue = new Queue<RingBehaviour>(_allRings);
        }

        public void Activate()
        {
            TrySetupNextRing();
            
            if (_timeToCompleteSystem > 0f)
            {
                CountdownTimer.Instance.StartCountdown(_timeToCompleteSystem, onCountdownCompleted: CountdownFinish);
            }
        }

        private void RingTriggered(RingBehaviour ring)
        {
            ring.Deactivate();
            TrySetupNextRing();
        }

        private void CountdownFinish()
        {
            Perigon.Utility.Logger.LogString("Ran out of time! Resetting the labor", key: "Labor");
            Reset();
            OnLaborCompleted?.Invoke(false);
        }

        private void TrySetupNextRing()
        {
            if (_ringQueue.Count == 0)
            {
                InvokeLaborCompleted();
                return;
            }
            _currentRing = _ringQueue.Dequeue();
            _currentRing.Activate();
        }

        private void InvokeLaborCompleted()
        {
            if(_timeToCompleteSystem > 0f)
                CountdownTimer.Instance.StopCountdown();
            OnLaborCompleted?.Invoke(true);
        }
    }
}