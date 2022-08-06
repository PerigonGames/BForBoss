using Perigon.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace BForBoss
{
    public class WaveSandboxWorldManager : BaseWorldManager
    {
        private const float DEATH_POST_PROCESSING_DURATION = 0.1F;
        private const float DEATH_POST_PROCESSING_START = 0F;
        private const float DEATH_POST_PROCESSING_END = 0.1f;
        
        [SerializeField] 
        private Transform _spawnLocation = null;
        
        [Title("Component")]
        [SerializeField] private LifeCycleManager _lifeCycleManager = null;
        [SerializeField] private WaveManager _waveManager;
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager;
        
        [Title("Effects")] 
        [SerializeField] private Volume _deathVolume = null;
        
        private PostProcessingVolumeWeightTool _postProcessingVolumeWeightTool = null;

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
            _lifeCycleManager.Reset();
            
            if (_waveManager != null)
            {
                _waveManager.Reset();
            }
            
            FindObjectsOfType<PatrolBehaviour>().ForEach(pb => pb.Reset());
        }

        protected override void Awake()
        {
            base.Awake();
            _postProcessingVolumeWeightTool = new PostProcessingVolumeWeightTool(_deathVolume, DEATH_POST_PROCESSING_DURATION, DEATH_POST_PROCESSING_START, DEATH_POST_PROCESSING_END);
        }

        protected override void Start()
        {
            base.Start();

            if (_lifeCycleManager != null)
            {
                _lifeCycleManager.Initialize(() => _playerBehaviour.transform.position);
            }
            
            WaveModel waveModel = new WaveModel();
            if (_waveManager != null)
            {
                _waveManager.Initialize(_lifeCycleManager, _enemySpawnerManager, waveModel);
            }
        }
        
        protected override void HandleOnDeath()
        {
            _postProcessingVolumeWeightTool.InstantDistortAndRevert();
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
        }
    }
}
