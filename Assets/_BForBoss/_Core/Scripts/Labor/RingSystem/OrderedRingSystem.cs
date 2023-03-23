using System;
using System.Collections;
using System.Collections.Generic;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class OrderedRingSystem : RingSystem
    {
        private readonly bool _randomise;
        private Queue<RingBehaviour> _ringQueue;

        public OrderedRingSystem(RingBehaviour[] rings, bool randomise = false) : base(rings)
        {
            _randomise = randomise;
        }
        
        public override void Activate()
        {
            TrySetupNextRing();
        }

        protected override void SetupRingLists(IList<RingBehaviour> rings)
        {
            if(_randomise)
                rings.ShuffleFisherYates();
            _ringQueue = new Queue<RingBehaviour>(rings);
        }

        protected override void RingTriggered(RingBehaviour ring)
        {
            base.RingTriggered(ring);
            TrySetupNextRing();
        }

        private void TrySetupNextRing()
        {
            if (_ringQueue.Count == 0)
            {
                InvokeLaborCompleted();
                return;
            }
            var ring = _ringQueue.Dequeue();
            ActivateRing(ring);
        }
    }
}
