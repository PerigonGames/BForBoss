using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WaveSandboxWorldManager : BaseWorldManager
    {
        [SerializeField] 
        private Transform _spawnLocation = null;
        
        [Title("Component")]
        [SerializeField] private LifeCycleManager _lifeCycleManager = null;
        [SerializeField] private WaveManager _waveManager;
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager;

        [Title("HUD")] [SerializeField] private WaveViewBehaviour _waveView = null;
        
        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;
        
        protected override void Reset()
        {
            base.Reset();
            _lifeCycleManager.Reset();
            
            if (_waveManager != null)
            {
                _waveManager.Reset();
            }
        }

        protected override void Start()
        {
            base.Start();

            if (_lifeCycleManager != null)
            {
                _lifeCycleManager.Initialize(() => _playerBehaviour.PlayerMovement.camera.transform.position);
            }
            
            WaveModel waveModel = new WaveModel();

            if (_waveView != null)
            {
                _waveView.Initialize(waveModel);
            }
            
            if (_waveManager != null)
            {
                _waveManager.Initialize(_lifeCycleManager, _enemySpawnerManager, waveModel);
            }
        }
        
        protected override void HandleOnDeath()
        {
            _lifeCycleManager.Reset();            
            base.HandleOnDeath();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_lifeCycleManager == null)
            {
                Debug.LogWarning("Life Cycle Manager missing from World Manager");
            }

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
        }
    }
}
