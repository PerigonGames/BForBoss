using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WeaponSandboxWorldManager : BaseWorldManager
    {
        [SerializeField]
        private Transform _spawnLocation = null;

        [Title("Component")]
        [SerializeField] private LifeCycleManager _lifeCycleManager = null;
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager = null;

        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;
        
        protected override void Reset()
        {
            base.Reset();
            if (_lifeCycleManager != null)
            {
                _lifeCycleManager.Reset();
            }

            if (_enemySpawnerManager != null)
            {
                _enemySpawnerManager.Reset();
            }
        }

        protected override void Start()
        {
            if (_lifeCycleManager != null)
            {
                _lifeCycleManager.Initialize(() => _playerBehaviour.PlayerMovement.camera.transform.position);
            }

            if (_enemySpawnerManager != null)
            {
                _enemySpawnerManager.Initialize(_lifeCycleManager);
            }
            base.Start();
        }

        protected override void HandleOnDeath()
        {
            if (_lifeCycleManager != null)            
            {
                _lifeCycleManager.Reset();
            }
            base.HandleOnDeath();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_lifeCycleManager == null)
            {
                Debug.LogWarning("Life Cycle Manager missing from World Manager");
            }

            if (_enemySpawnerManager == null)
            {
                Debug.LogWarning("Enemy Spawner Manager missing from the world manager");
            }
        }
    }
}
