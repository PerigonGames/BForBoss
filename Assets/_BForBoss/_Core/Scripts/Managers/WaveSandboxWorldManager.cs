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
        [SerializeField] private EnemyContainer _enemyContainer;
        [SerializeField] private WaveManager _waveManager;
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager;

        [Title("HUD")] [SerializeField] private WaveViewBehaviour _waveView = null;
        
        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;
        
        protected override void Reset()
        {
            base.Reset();
            
            if (_waveManager != null)
            {
                _waveManager.Reset();
            }
        }

        protected override void Start()
        {
            base.Start();

            WaveModel waveModel = new WaveModel();

            if (_waveView != null)
            {
                _waveView.Initialize(waveModel);
            }
            
            if (_waveManager != null)
            {
               // _waveManager.Initialize(_lifeCycleManager, _enemySpawnerManager, waveModel);
            }
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_waveManager == null)
            {
                PanicHelper.Panic(new Exception("Wave Manager missing from the world manager"));
            }

            if (_enemySpawnerManager == null)
            {
                PanicHelper.Panic(new Exception("Enemy Spawner Manager missing from the world manager"));
            }

            if (_waveView == null)
            {
                PanicHelper.Panic(new Exception("Wave View UI missing from the world Manager"));
            }

            if (_enemyContainer == null)
            {
                PanicHelper.Panic(new Exception("Enemy Container missing from WaveSandboxWorldManager"));
            }
        }
    }
}
