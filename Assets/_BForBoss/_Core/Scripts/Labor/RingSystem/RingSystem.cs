using System;
using System.Collections.Generic;
using BForBoss.Labor;
using PerigonGames;
using UnityEngine;

namespace BForBoss.RingSystem
{
    public class RingSystem : ILabor
    {
        public event Action<bool> OnLaborCompleted;
        private RingBehaviour _currentRing;
        private Queue<RingBehaviour> _ringQueue;
        private readonly RingBehaviour[] _allRings;

        private readonly float _time;

        private int _ringIndex = 1;
        private Color _systemColor;

        public RingSystem(RingBehaviour[] rings, Color color, float time = 0f)
        {
            _allRings = rings;
            _time = time;
            _systemColor = color;
            
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
            TrySetupNextRing();
            
            if (_time > 0f)
            {
                CountdownTimer.Instance.StartCountdown(_time, CountdownFinish);
            }
        }

        private void SetupRingLists(IList<RingBehaviour> rings)
        {
            _ringQueue = new Queue<RingBehaviour>(rings);
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
            ring.RingActivated = null;
        }

        private void ActivateRing(RingBehaviour ring)
        {
            ring.RingActivated = RingTriggered;
            ring.SetLabel(_ringIndex++.ToString());
            ring.SetColor(_systemColor);
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