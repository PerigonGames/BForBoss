using UnityEngine;

namespace BForBoss
{
    public class SandboxWorldManager : BaseWorldManager
    {
        [SerializeField] 
        private Transform _spawnLocation = null;
        
        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;
        protected override void SetupAnalytics()
        {
            // Not Implement
        }
    }
}
