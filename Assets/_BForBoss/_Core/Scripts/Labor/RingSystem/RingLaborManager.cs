using System;
using System.Collections.Generic;
using BForBoss.Labor;
using Perigon.Utility;
using PerigonGames;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = Perigon.Utility.Logger;

namespace BForBoss.RingSystem
{
    public class RingLaborManager : MonoBehaviour
    {
        [SerializeField, InfoBox("When player fails, delayed time before starting the round of rings again")]
        private float penaltyDelayedStartTime = 3f;

        private ILabor _ringSystem;
        private IRandomUtility _randomUtility;

        [HideInEditorMode]
        public Action OnLaborCompleted;

        public void Reset()
        {
            _ringSystem?.Reset();
            CountdownTimer.Instance.Reset();
        }

        public void Initialize(IRandomUtility randomUtility = null)
        {
            _randomUtility = randomUtility ?? new RandomUtility();
        }

        public void SetRings(RingGrouping ringSystemsToBuild)
        {
            _ringSystem = BuildLabor(ringSystemsToBuild);
            _ringSystem.OnLaborCompleted += RingSystemOnOnLaborCompleted;
        }

        private void RingSystemOnOnLaborCompleted(bool didSucceed)
        {
            if (didSucceed)
            {
                Logger.LogString("Labor Completed", key: "Labor");
                OnLaborCompleted?.Invoke();
            }
            else
            {
                Logger.LogString("Labor Failed, Retrying", key: "Labor");
                _ringSystem.Activate();
            }
        }

        public void ActivateSystem()
        {
            _ringSystem?.Activate();
        }

        private ILabor BuildLabor(RingGrouping grouping)
        {
            if (_randomUtility.NextTryGetElement(grouping.GroupOfRings, out var groupOfRings))
            {
                foreach (var ringBehaviour in groupOfRings.Rings)
                {
                    ringBehaviour.Deactivate();
                }
                var ringSystem = new GroupedRingSystem(
                    rings: groupOfRings.Rings,
                    color: grouping.RingColor,
                    timeToCompleteSystem: grouping.TimeToComplete,
                    penaltyDelayedStartTime: penaltyDelayedStartTime);
                ringSystem.OnLaborCompleted += (success) => Logger.LogString($"{(success ? "Completed" : "Failed")} {groupOfRings.Rings.Length} ring system", key:"Labor");
                return ringSystem;
            }

            PanicHelper.Panic(new Exception("Unable to get random element from grouping rings"));
            return null;
        }
    }

    [Serializable]
    public class RingGrouping
    {
        public float TimeToComplete = 5f;
        [ListDrawerSettings(ShowPaging = false)]
        public List<GroupedRingsWrapper> GroupOfRings = new List<GroupedRingsWrapper>();
        public Color RingColor;
    }
    
    [Serializable]
    public class GroupedRingsWrapper
    {
        public RingBehaviour[] Rings;
    }
}
