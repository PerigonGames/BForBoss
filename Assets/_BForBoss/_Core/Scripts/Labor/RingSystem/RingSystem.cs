using System;
using System.Collections.Generic;
using BForBoss.Labor;
using PerigonGames;

namespace BForBoss
{
    public class RingSystem : ILabor
    {
        public event Action OnLaborCompleted;

        private RingSystemTypes _type;

        private Queue<RingBehaviour> _ringQueue;
        private List<RingBehaviour> _remainingRings;
        private List<RingBehaviour> _completedRings;

        public RingSystem(RingBehaviour[] rings, RingSystemTypes type = RingSystemTypes.Standard)
        {
            _type = type;

            SetupRingLists(rings);

            foreach (var ring in rings)
            {
                ring.gameObject.SetActive(false);
            }
        }
        
        public void Activate()
        {
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
            _completedRings.Reverse();
            if (_ringQueue is {Count: > 0})
            {
                _completedRings.AddRange(_ringQueue);
            }
            if (_remainingRings is {Count: > 0})
            {
                _completedRings.AddRange(_remainingRings);
            }
            foreach (var ring in _completedRings)
            {
                ring.gameObject.SetActive(false);
                ring.RingActivated -= RingTriggered;
            }
            
            SetupRingLists(_completedRings);
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
            _completedRings = new List<RingBehaviour>();
        }

        private void RingTriggered(RingBehaviour ring)
        {
            ring.gameObject.SetActive(false);
            ring.RingActivated -= RingTriggered;
            _completedRings.Add(ring);
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
            ring.RingActivated += RingTriggered;
            ring.gameObject.SetActive(true);
        }

        private void ActivateAllAtOnce()
        {
            foreach (var ring in _remainingRings)
            {
                ring.RingActivated += RingTriggered;
                ring.gameObject.SetActive(true);
            }
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