using Perigon.Entities;
using Sirenix.Utilities;
using UnityEngine;

namespace BForBoss
{
    public class EnvironmentManager: MonoBehaviour
    {
        private PatrolBehaviour[] _patrolBehaviours;
        private ItemPickupBehaviour[] _itemPickupBehaviours;
        private DummyTargetBehaviour[] _dummyTargetBehaviours;
        
        public void Initialize()
        {
            _dummyTargetBehaviours.ForEach(dummy => dummy.Initialize(onDeathCallback: () => { }));
        }

        public void Reset()
        {
            _patrolBehaviours.ForEach(pb => pb.Reset());
            _itemPickupBehaviours.ForEach(pickUp => pickUp.Reset());
            _dummyTargetBehaviours.ForEach(dummy => dummy.Reset());
        }
        
        public void CleanUp()
        {
            _patrolBehaviours.ForEach(pb => pb.CleanUp());
            _itemPickupBehaviours.ForEach(pickUp => pickUp.CleanUp());
            _dummyTargetBehaviours.ForEach(dummy => dummy.CleanUp());
        }

        private void Awake()
        {
            _patrolBehaviours = FindObjectsOfType<PatrolBehaviour>();
            _itemPickupBehaviours = FindObjectsOfType<ItemPickupBehaviour>();
            _dummyTargetBehaviours = FindObjectsOfType<DummyTargetBehaviour>();
        }
    }
}
