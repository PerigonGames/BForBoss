using System;
using System.Collections.Generic;
using BForBoss.Labor;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public abstract class RingSystem : ILabor
    {
        public event Action OnLaborCompleted;

        private Queue<RingBehaviour> _ringQueue;
        private List<RingBehaviour> _remainingRings;
        private readonly RingBehaviour[] _allRings;

        protected RingSystem(RingBehaviour[] rings)
        {
            _allRings = rings;
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
            
            SetupRingLists(_allRings);
        }
        
        public abstract void Activate();

        protected abstract void SetupRingLists(IList<RingBehaviour> rings);

        protected virtual void RingTriggered(RingBehaviour ring)
        {
            DeactivateRing(ring);
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

        protected void InvokeLaborCompleted()
        {
            OnLaborCompleted?.Invoke();
        }
    }
}