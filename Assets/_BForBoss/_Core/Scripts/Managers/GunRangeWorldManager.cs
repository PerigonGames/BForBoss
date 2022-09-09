using UnityEngine;

namespace BForBoss
{
    public class GunRangeWorldManager : BaseWorldManager
    {
        protected override Vector3 SpawnLocation => Vector3.zero;
        protected override Quaternion SpawnLookDirection => Quaternion.identity;
    }
}
