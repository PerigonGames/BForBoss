using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace BForBoss
{
    public class WaveSandboxWorldManager : BaseWorldManager
    {
        [SerializeField] 
        private Transform _spawnLocation = null;
        
        [Title("Component")]
        [SerializeField] private EnemyBehaviourManager _enemyBehaviourManager = null;
        [SerializeField] private WaveManager _waveManager;
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager;

        [Title("HUD")] [SerializeField] private WaveViewBehaviour _waveView = null;
        
        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;

        protected override void CleanUp()
        {
            base.CleanUp();
            FindObjectsOfType<PatrolBehaviour>().ForEach(pb => pb.CleanUp());
        }

        protected override void Reset()
        {
            base.Reset();
            _enemyBehaviourManager.Reset();
            
            if (_waveManager != null)
            {
                _waveManager.Reset();
            }

            if (_enemySpawnerManager != null)
            {
                _enemySpawnerManager.Reset();
            }

            if (_enemyBehaviourManager != null)
            {
                _enemyBehaviourManager.Reset();
            }
            
            FindObjectsOfType<PatrolBehaviour>().ForEach(pb => pb.Reset());
        }

        protected override void Start()
        {
            base.Start();

            if (_enemyBehaviourManager != null)
            {
                _enemyBehaviourManager.Initialize(() => _playerBehaviour.transform.position);
            }
            
            WaveModel waveModel = new WaveModel();

            if (_waveView != null)
            {
                _waveView.Initialize(waveModel);
            }
            
            if (_waveManager != null)
            {
                _waveManager.Initialize(waveModel, _enemySpawnerManager);
            }

            if (_enemySpawnerManager != null)
            {
                _enemySpawnerManager.Initialize(_enemyBehaviourManager, waveModel);
            }
        }
        
        protected override void HandleOnDeath()
        {
            _enemyBehaviourManager.Reset();            
            base.HandleOnDeath();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_waveManager == null)
            {
                Debug.LogWarning("Wave Manager missing from the world manager");
            }

            if (_enemySpawnerManager == null)
            {
                Debug.LogWarning("Enemy Spawner Manager missing from the world manager");
            }

            if (_waveView == null)
            {
                Debug.LogWarning("Wave View UI missing from the world Manager");
            }

            if (_enemyBehaviourManager == null)
            {
                Debug.LogWarning("Enemy Behaviour Manager missing from the world Manager");
            }
        }
    }
}
