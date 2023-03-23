using System;
using System.Collections.Generic;
using System.Diagnostics;
using BForBoss.Labor;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace BForBoss
{
    public class RingLaborManager : MonoBehaviour
    {
        [SerializeField] private RingGrouping[] _systemsToBuild;

        private LaborSystem _laborSystem;
        private bool _hasCompletedSystem = false;
        private List<RingSystem> _ringSystems;

        public void Reset()
        {
            foreach (var system in _ringSystems)
            {
                system.Reset();
            }

            _laborSystem?.Dispose();
            _laborSystem = new LaborSystem(_ringSystems);
        }
        
        public void Initialize()
        {
            CreateSystems();
            _laborSystem = new LaborSystem(_ringSystems);
        }

        private void CreateSystems()
        {
            _ringSystems = new List<RingSystem>();
            foreach (var grouping in _systemsToBuild)
            {
                RingSystem newSystem = grouping.RingSystemType switch
                {
                    RingSystemTypes.Standard => new OrderedRingSystem(grouping.Rings),
                    RingSystemTypes.DisplayAllAtOnce => new AllAtOnceRingSystem(grouping.Rings),
                    RingSystemTypes.RandomSelection => new OrderedRingSystem(grouping.Rings, true),
                    _ => throw new ArgumentOutOfRangeException()
                };
                newSystem.OnLaborCompleted += () => Debug.Log($"Completed {grouping.Rings.Length} ring {grouping.RingSystemType} system");
                _ringSystems.Add(newSystem);
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
            public RingSystemTypes RingSystemType;
            public RingBehaviour[] Rings;
        }
        
        public enum RingSystemTypes
        {
            Standard, DisplayAllAtOnce, RandomSelection
        }
    }
}