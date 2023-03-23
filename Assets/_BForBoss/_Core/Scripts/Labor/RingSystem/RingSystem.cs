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

        private readonly RingSystemTypes _type;

        private Queue<RingBehaviour> _ringQueue;
        private List<RingBehaviour> _remainingRings;
        private readonly RingBehaviour[] _allRings;

        public RingSystem(RingBehaviour[] rings, RingSystemTypes type = RingSystemTypes.Standard)
        {
            _type = type;

            _allRings = rings;
            SetupRingLists(_allRings);

            foreach (var ring in _allRings)
            {
                ring.gameObject.SetActive(false);
            }
        }
        
        public void Activate()
        {
            Debug.Log("Activating system of type " + _type);
            if (_type == RingSystemTypes.DisplayAllAtOnce)
            {
                ActivateAllAtOnce();
            }
            else
            {
                TrySetupNextRing();
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

        private void SetupRingLists(IList<RingBehaviour> rings)
        {
            switch (_type)
            {
                case RingSystemTypes.DisplayAllAtOnce:
                    _remainingRings = new List<RingBehaviour>(rings);
                    break;
                case RingSystemTypes.RandomSelection:
                    rings.ShuffleFisherYates();
                    _ringQueue = new Queue<RingBehaviour>(rings);
                    break;
                case RingSystemTypes.Standard:
                    _ringQueue = new Queue<RingBehaviour>(rings);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RingTriggered(RingBehaviour ring)
        {
            DeactivateRing(ring);
            if (_type == RingSystemTypes.DisplayAllAtOnce)
            {
                HandleRingTriggeredAllAtOnce(ring);
            }
            else
            {
                TrySetupNextRing();
            }
        }

        private void TrySetupNextRing()
        {
            if (_ringQueue.Count == 0)
            {
                OnLaborCompleted?.Invoke();
                return;
            }
            var ring = _ringQueue.Dequeue();
            ActivateRing(ring);
        }

        private void ActivateAllAtOnce()
        {
            foreach (var ring in _remainingRings)
            {
                ActivateRing(ring);
            }
        }

        private void DeactivateRing(RingBehaviour ring)
        {
            ring.gameObject.SetActive(false);
            ring.RingActivated -= RingTriggered;
        }

        private void ActivateRing(RingBehaviour ring)
        {
            ring.RingActivated += RingTriggered;
            ring.gameObject.SetActive(true);
        }

        private void HandleRingTriggeredAllAtOnce(RingBehaviour ring)
        {
            _remainingRings.Remove(ring);

            if (_remainingRings.Count == 0)
            {
                OnLaborCompleted?.Invoke();
            }
        }
        
        public enum RingSystemTypes
        {
            Standard, DisplayAllAtOnce, RandomSelection
        }
    }
}