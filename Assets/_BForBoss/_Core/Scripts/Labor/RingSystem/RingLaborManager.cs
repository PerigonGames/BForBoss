using System;
using System.Collections.Generic;
using BForBoss.Labor;
using Perigon.Utility;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Logger = Perigon.Utility.Logger;

namespace BForBoss.RingSystem
{
    public class RingLaborManager : SerializedMonoBehaviour
    {
        [OdinSerialize] private RingGrouping[] _systemsToBuild;

        private LaborSystem _laborSystem;
        private List<ILabor> _listOfRingSystems;
        private bool _hasCompletedSystem;

        [HideInEditorMode]
        public Action OnLaborCompleted;

        public void Reset()
        {
            //TODO - Reset whole game vs on death
            foreach (var system in _listOfRingSystems)
            {
                system.Reset();
            }

            if (CountdownTimer.Instance.IsRunning)
            {
                CountdownTimer.Instance.ToggleCountdown();
            }
            
            _hasCompletedSystem = false;
        }
        
        public void Initialize()
        {
            CreateSystems();
            _laborSystem = new LaborSystem(_listOfRingSystems, randomize: true);
        }

        public void ActivateSystem()
        {
            _laborSystem?.Activate();
        }

        public void ToggleTimer()
        {
            CountdownTimer.Instance.ToggleCountdown();
        }

        private void CreateSystems()
        {
            _listOfRingSystems = new List<ILabor>();
            foreach (var grouping in _systemsToBuild)
            {
                this.PanicIfNullOrEmptyList(grouping.Rings, "Ring list");
                ILabor newSystem = new GroupedRingSystem(grouping.Rings, time: grouping.Time);
                newSystem.OnLaborCompleted += (success) => Logger.LogString($"{(success ? "Completed" : "Failed")} {grouping.Rings.Length} ring Standard system", key:"Labor");
                _listOfRingSystems.Add(newSystem);
            }
        }

        private void Update()
        {
            if (!_hasCompletedSystem && (_laborSystem?.IsComplete ?? false)) 
            {
                Logger.LogString("All labors completed", key: "Labor");
                _hasCompletedSystem = true;
                OnLaborCompleted?.Invoke();
            }
        }
    }
    
    [Serializable]
    public class RingGrouping
    {
        public float Time = 5f;
        public RingBehaviour[] Rings;
    }
}