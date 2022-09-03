using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class GunRangeWorldManager : BaseWorldManager
    {
        [Title("Component")]

        protected override Vector3 SpawnLocation => Vector3.zero;
        protected override Quaternion SpawnLookDirection => Quaternion.identity;

        protected override void Reset()
        {
            base.Reset();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
        }
    }
}
