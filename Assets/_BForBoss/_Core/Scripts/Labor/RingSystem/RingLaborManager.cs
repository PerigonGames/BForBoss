using System;
using System.Collections.Generic;
using BForBoss.Labor;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Perigon.Utility.Logger;

namespace BForBoss.RingSystem
{
    public class RingLaborManager : SerializedMonoBehaviour
    {
        [SerializeField, InfoBox("When player fails, delayed time before starting the round of rings again")]
        private float penaltyDelayedStartTime = 3f;

        private LaborSystem _laborSystem;
        private List<ILabor> _listOfRingSystems;
        private bool _hasCompletedSystem;

        [HideInEditorMode]
        public Action OnLaborCompleted;

        public void Reset()
        {
            foreach (var system in _listOfRingSystems)
            {
                system.Reset();
            }

            CountdownTimer.Instance.Reset();
            _laborSystem = new LaborSystem(_listOfRingSystems, randomize: true);
            _hasCompletedSystem = false;
        }

        public void SetRings(RingGrouping ringSystemsToBuild)
        {
            //CreateSystems(ringSystemsToBuild);
            _laborSystem = new LaborSystem(_listOfRingSystems, randomize: true);
        }

        public void ActivateSystem()
        {
            _laborSystem?.Activate();
        }

        private void CreateSystems(RingGrouping[] ringSystemsToBuild)
        {
            _listOfRingSystems = new List<ILabor>();
            foreach (var grouping in ringSystemsToBuild)
            {
                this.PanicIfNullOrEmptyList(grouping.Rings, "Ring list");
                ILabor newSystem = new GroupedRingSystem(
                    rings: grouping.Rings,
                    color: grouping.RingColor,
                    timeToCompleteSystem: grouping.TimeToComplete,
                    penaltyDelayedStartTime: penaltyDelayedStartTime);
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
        public float TimeToComplete = 5f;
        public RingBehaviour[] Rings;
        public Color RingColor;
    }
}
