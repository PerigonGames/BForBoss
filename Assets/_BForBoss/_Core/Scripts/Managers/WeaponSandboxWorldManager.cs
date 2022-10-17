using UnityEngine;

namespace BForBoss
{
    public class WeaponSandboxWorldManager : BaseWorldManager
    {
        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;
    }
}
