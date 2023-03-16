using UnityEngine;
using UnityEngine.SceneManagement;

namespace BForBoss
{
    public class SandboxWorldManager : BaseWorldManager
    {
        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;
    }
}
