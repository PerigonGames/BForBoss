using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WeaponSandboxWorldManager : BaseWorldManager
    {
        [SerializeField]
        private Transform _spawnLocation = null;

        [Title("Component")]
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager = null;

        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;

        protected override void Reset()
        {
            base.Reset();

            if (_enemySpawnerManager != null)
            {
                _enemySpawnerManager.Reset();
            }
        }

        protected override void Start()
        {

             if (_enemySpawnerManager != null) 
             { 
                 //_enemySpawnerManager.Initialize();
            }
            base.Start();
        }

        protected override void HandleOnDeath()
        {
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
