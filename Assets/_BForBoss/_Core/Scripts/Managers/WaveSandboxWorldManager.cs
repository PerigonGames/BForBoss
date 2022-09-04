using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WaveSandboxWorldManager : BaseWorldManager
    {
        [SerializeField] 
        private Transform _spawnLocation = null;

        [Title("Component")] 
        [SerializeField] private WaveManager _waveManager;
        [SerializeField] private EnemyContainer _enemyContainer;
        [SerializeField] private EnemySpawnersManager _enemySpawnersManager;

        [Title("HUD")] 
        [SerializeField] private WaveViewBehaviour _waveView = null;
        
        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;
        
        protected override void Reset()
        {
            base.Reset();
            _waveManager.Reset();
            _enemyContainer.Reset();
            _waveView.Reset();
        }

        protected override void Start()
        {
            base.Start();

            WaveModel waveModel = new WaveModel();
            _waveView.Initialize(waveModel);
            _enemyContainer.Initialize(() => _playerBehaviour.transform.position);
            _enemySpawnersManager.Initialize(_enemyContainer, waveModel);
            _waveManager.Initialize(waveModel, _enemySpawnersManager);
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_enemyContainer == null)
            {
                PanicHelper.Panic(new Exception("Enemy Container missing from WaveSandboxWorldManager"));
            }

            if (_waveManager == null)
            {
                PanicHelper.Panic(new Exception("Wave Manager missing from the world manager"));
            }

            if (_enemySpawnersManager == null)
            {
                PanicHelper.Panic(new Exception("Enemy Spawner Manager missing from the world manager"));
            }

            if (_waveView == null)
            {
                PanicHelper.Panic(new Exception("Wave View UI missing from the world Manager"));
            }
        }
    }
}
