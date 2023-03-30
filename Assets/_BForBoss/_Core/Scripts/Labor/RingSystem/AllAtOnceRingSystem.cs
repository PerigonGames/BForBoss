using System;
using System.Collections;
using System.Collections.Generic;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class AllAtOnceRingSystem : RingSystem
    {
        private List<RingBehaviour> _remainingRings;

        public AllAtOnceRingSystem(RingBehaviour[] rings) : base(rings)
        { }
        
        public override void Activate()
        {
            foreach (var ring in _remainingRings)
            {
                ActivateRing(ring);
            }
        }

        protected override void SetupRingLists(IList<RingBehaviour> rings)
        {
            _remainingRings = new List<RingBehaviour>(rings);
        }

        protected override void RingTriggered(RingBehaviour ring)
        {
            base.RingTriggered(ring);
            _remainingRings.Remove(ring);

            if (_remainingRings.Count == 0)
            {
                InvokeLaborCompleted();
            }
        }
    }
}
