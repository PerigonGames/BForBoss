using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
            _lifeCycleManager.Reset();

            if (_enemySpawnerManager != null)
            {
                _enemySpawnerManager.Reset();
            }
        }

        protected override void Start()
        {
            if (_lifeCycleManager != null)
            {
                _lifeCycleManager.Initialize(() => _playerBehaviour.transform.position);
            }

            // if (_enemySpawnerManager != null)
            // {
            //     _enemySpawnerManager.Initialize(_lifeCycleManager);
            // }
            base.Start();
        }

        protected override void HandleOnDeath()
        {
            _lifeCycleManager.Reset();
            base.HandleOnDeath();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            // if (_enemySpawnerManager == null)
            // {
            //     Debug.LogWarning("Enemy Spawner Manager missing from the world manager");
            // }
        }
    }
}
