using System;
using System.Collections.Generic;
using BForBoss.Labor;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class RingSystem : ILabor
    {
        public event Action OnLaborCompleted;
        private RingBehaviour _currentRing;
        private Queue<RingBehaviour> _ringQueue;
        private readonly RingBehaviour[] _allRings;

        private bool _isRandomized;
        private bool _allAtOnce;

        public RingSystem(RingBehaviour[] rings, bool isRandomized = false, bool allAtOnce = false)
        {
            _allRings = rings;
            _isRandomized = isRandomized;
            _allAtOnce = allAtOnce;
            SetupRingLists(_allRings);

            foreach (var ring in _allRings)
            {
                ring.gameObject.SetActive(false);
            }
        }

        public void Reset()
        {
            foreach (var ring in _allRings)
            {
                DeactivateRing(ring);
            }

            _currentRing = null;
            SetupRingLists(_allRings);
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

        protected void DeactivateRing(RingBehaviour ring)
        {
            ring.gameObject.SetActive(false);
            ring.RingActivated -= RingTriggered;
        }

        protected void ActivateRing(RingBehaviour ring)
        {
            ring.RingActivated += RingTriggered;
            ring.gameObject.SetActive(true);
        }
        
        protected void TrySetupNextRing()
        {
            if (_ringQueue.Count == 0)
            {
                InvokeLaborCompleted();
                return;
            }
            _currentRing = _ringQueue.Dequeue();
            if (!_currentRing.gameObject.activeInHierarchy) ActivateRing(_currentRing);
        }

        protected void InvokeLaborCompleted()
        {
            OnLaborCompleted?.Invoke();
        }
    }
}