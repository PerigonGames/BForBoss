using System;
using System.Collections.Generic;
using BForBoss.Labor;
using UnityEngine;

namespace BForBoss
{
    public class RingLaborManager : MonoBehaviour
    {
        [SerializeField] private RingGrouping[] _systemsToBuild;

        private CountdownViewBehaviour _countdownView;
        private LaborSystem _laborSystem;
        private bool _hasCompletedSystem = false;
        private List<RingSystem> _ringSystems;

        public void Reset()
        {
            if(_ringSystems == null) CreateSystems();
            
            foreach (var system in _ringSystems)
            {
                system.Reset();
            }

            _laborSystem?.Dispose();
            _laborSystem = new LaborSystem(_ringSystems);
        }
        
        public void Initialize(CountdownViewBehaviour countdownView)
        {
            _countdownView = countdownView;
            CreateSystems();
            _laborSystem = new LaborSystem(_ringSystems);
        }

        public void ToggleTimer()
        {
            if(_countdownView.IsRunning)
                _countdownView.PauseCountdown();
            else
                _countdownView.ResumeCountdown();
        }

        private void CreateSystems()
        {
            _ringSystems = new List<RingSystem>();
            foreach (var grouping in _systemsToBuild)
            {
                RingSystem newSystem = grouping.RingSystemType switch
                {
                    RingSystemTypes.Standard => new RingSystem(grouping.Rings, _countdownView, time: grouping.Time),
                    RingSystemTypes.DisplayAllAtOnce => new RingSystem(grouping.Rings, _countdownView, allAtOnce: true, time: grouping.Time),
                    RingSystemTypes.RandomSelection => new RingSystem(grouping.Rings, _countdownView, isRandomized: true, time: grouping.Time),
                    _ => throw new ArgumentOutOfRangeException()
                };
                newSystem.OnLaborCompleted += () => Perigon.Utility.Logger.LogString($"Completed {grouping.Rings.Length} ring {grouping.RingSystemType} system", key:"Labor");
                _ringSystems.Add(newSystem);
            }
        }

        private void Update()
        {
            if(_laborSystem == null) return;
            if (!_hasCompletedSystem && _laborSystem.IsComplete)
            {
                Perigon.Utility.Logger.LogString("All labors completed", key: "Labor");
                _hasCompletedSystem = true;
            }
        }

        [System.Serializable]
        public class RingGrouping
        {
            public RingSystemTypes RingSystemType;
            public float Time = 5f;
            public RingBehaviour[] Rings;
        }
        
        public enum RingSystemTypes
        {
            Standard, DisplayAllAtOnce, RandomSelection
        }
    }
}