using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class GunRangeWorldManager : BaseWorldManager
    {
        [Title("Component")]
        [SerializeField] private LifeCycleManager _lifeCycleManager = null;

        protected override Vector3 SpawnLocation => Vector3.zero;
        protected override Quaternion SpawnLookDirection => Quaternion.identity;

        protected override void Reset()
        {
            base.Reset();
            _lifeCycleManager.Reset();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_lifeCycleManager == null)
            {
                PanicHelper.Panic(new Exception("Life Cycle Manager missing from World Manager"));
            }
        }
    }
}
