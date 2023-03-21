using System.Collections.Generic;
using BForBoss.Labor;
using UnityEngine;

namespace BForBoss
{
    public class RingLaborManager : MonoBehaviour
    {
        public RingGrouping[] RingSystems;

        private LaborSystem _laborSystem;
        private bool _hasCompletedSystem = false;
        private List<RingSystem> ringSystems;

        public void Reset()
        {
            Debug.Log("Reset called");
            foreach (var system in ringSystems)
            {
                system.Reset();
            }

            _laborSystem = new LaborSystem(ringSystems);
        }
        
        public void Initialize()
        {
            CreateSystems();
            _laborSystem = new LaborSystem(ringSystems);
        }

        private void CreateSystems()
        {
            ringSystems = new List<RingSystem>();
            foreach (var grouping in RingSystems)
            {
                var newSystem = new RingSystem(grouping.Rings, grouping.RingSystemType);
                newSystem.OnLaborCompleted += () => Debug.Log($"Completed {grouping.Rings.Length} ring {grouping.RingSystemType} system");
                ringSystems.Add(newSystem);
            }
        }

        private void Update()
        {
            if(_laborSystem == null) return;
            if (!_hasCompletedSystem && _laborSystem.IsComplete)
            {
                Debug.Log("All labors completed");
                _hasCompletedSystem = true;
            }
        }

        [System.Serializable]
        public class RingGrouping
        {
            public RingSystem.RingSystemTypes RingSystemType;
            public RingBehaviour[] Rings;
        }
    }
}