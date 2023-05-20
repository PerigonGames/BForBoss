using System;
using System.Collections.Generic;
using BForBoss.Labor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace BForBoss.RingSystem
{
    public class RingLaborManager : SerializedMonoBehaviour
    {
        [OdinSerialize] private RingGrouping[] _systemsToBuild;

        private LaborSystem _laborSystem;
        private bool _hasCompletedSystem = false;
        private List<ILabor> _ringSystems;

        private Action _onLaborCompleted;

        public void Reset()
        {
            if(_ringSystems == null) CreateSystems();
            
            foreach (var system in _ringSystems)
            {
                system.Reset();
            }

            _laborSystem?.Dispose();
            _laborSystem = new LaborSystem(_ringSystems, false);
            _hasCompletedSystem = false;
        }
        
        public void Initialize(Action onLaborCompleted)
        {
            _onLaborCompleted = onLaborCompleted;
            CreateSystems();
            _laborSystem = new LaborSystem(_ringSystems, false);
        }

        public void ToggleTimer()
        {
            CountdownTimer.Instance.ToggleCountdown();
        }

        private void CreateSystems()
        {
            _ringSystems = new List<ILabor>();
            foreach (var grouping in _systemsToBuild)
            {
                ILabor newSystem;
                if (grouping.RingSystemType == RingSystemTypes.Nested)
                {
                    var nestedSystems = new RingSystem[grouping.NestedSystems.Length];
                    for(int i = 0; i < grouping.NestedSystems.Length; i++)
                    {
                        nestedSystems[i] = BuildFromGrouping(grouping.NestedSystems[i], -1f);
                    }
                    newSystem = new NestedRingSystem(nestedSystems, true, grouping.Time);
                    newSystem.OnLaborCompleted += () => Perigon.Utility.Logger.LogString($"Completed {grouping.NestedSystems.Length} system {grouping.RingSystemType} system", key:"Labor");
                }
                else
                {
                    newSystem = BuildFromGrouping(grouping);
                    newSystem.OnLaborCompleted += () => Perigon.Utility.Logger.LogString($"Completed {grouping.Rings.Length} ring {grouping.RingSystemType} system", key:"Labor");
                }
                _ringSystems.Add(newSystem);
            }
        }

        private RingSystem BuildFromGrouping(RingGrouping grouping, float? timeOverride = null)
        {
            RingSystem newSystem = grouping.RingSystemType switch
            {
                RingSystemTypes.Standard => new RingSystem(grouping.Rings, time: timeOverride.GetValueOrDefault(grouping.Time)),
                RingSystemTypes.DisplayAllAtOnce => new RingSystem(grouping.Rings,  allAtOnce: true, time: timeOverride.GetValueOrDefault(grouping.Time)),
                RingSystemTypes.RandomSelection => new RingSystem(grouping.Rings, isRandomized: true, time: timeOverride.GetValueOrDefault(grouping.Time)),
                    
                _ => throw new ArgumentOutOfRangeException()
            };
            return newSystem;
        }

        private void Update()
        {
            if (_laborSystem == null)
            {
                return;
            }
            
            if (!_hasCompletedSystem && _laborSystem.IsComplete)
            {
                Perigon.Utility.Logger.LogString("All labors completed", key: "Labor");
                _hasCompletedSystem = true;

                _onLaborCompleted?.Invoke();
            }
        }

        [System.Serializable]
        public class RingGrouping
        {
            public RingSystemTypes RingSystemType;
            public float Time = 5f;
            
            [HideIf("RingSystemType", Value = RingSystemTypes.Nested)]
            public RingBehaviour[] Rings;
            [ShowIf("RingSystemType", Value = RingSystemTypes.Nested), NonSerialized, OdinSerialize]
            public RingGrouping[] NestedSystems = null;

        }
        
        public enum RingSystemTypes
        {
            Standard, DisplayAllAtOnce, RandomSelection, Nested
        }
    }
}