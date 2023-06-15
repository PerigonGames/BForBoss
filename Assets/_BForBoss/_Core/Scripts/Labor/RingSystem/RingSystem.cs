using System;
using System.Collections.Generic;
using BForBoss.Labor;
using PerigonGames;

namespace BForBoss.RingSystem
{
    public class RingSystem : ILabor
    {
        public event Action<bool> OnLaborCompleted;
        private RingBehaviour _currentRing;
        private Queue<RingBehaviour> _ringQueue;
        private readonly RingBehaviour[] _allRings;

        private readonly bool _isRandomized;
        private readonly bool _allAtOnce;
        
        private readonly float _time;

        private int _ringIndex = 1;

        public RingSystem(RingBehaviour[] rings, bool isRandomized = false, bool allAtOnce = false, float time = 0f)
        {
            _allRings = rings;
            _isRandomized = isRandomized;
            _allAtOnce = allAtOnce;
            _time = time;
            
            Reset();
        }

        public void Reset()
        {
            foreach (var ring in _allRings)
            {
                DeactivateRing(ring);
            }

            _currentRing = null;
            SetupRingLists(_allRings);
            _ringIndex = 1;
        }

        public void Activate()
        {
            if (_allAtOnce)
            {
                foreach (var ring in _ringQueue)
                {
                    ActivateRing(ring);
                }
            }
            TrySetupNextRing();
            
            if (_time > 0f)
            {
                CountdownTimer.Instance.StartCountdown(_time, CountdownFinish);
            }
        }

        private void SetupRingLists(IList<RingBehaviour> rings)
        {
            if (_isRandomized)
            {
                rings.ShuffleFisherYates();
            }
            _ringQueue = new Queue<RingBehaviour>(rings);
        }

        private void RingTriggered(RingBehaviour ring)
        {
            if(_allAtOnce && ring != _currentRing) return;
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
            ring.RingActivated = null;
        }

        private void ActivateRing(RingBehaviour ring)
        {
            ring.RingActivated = RingTriggered;
            ring.SetLabel(_ringIndex++.ToString());
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
            if (!_currentRing.gameObject.activeInHierarchy) ActivateRing(_currentRing);
        }

        private void InvokeLaborCompleted()
        {
            if(_time > 0f)
                CountdownTimer.Instance.StopCountdown();
            OnLaborCompleted?.Invoke(true);
        }
    }
}