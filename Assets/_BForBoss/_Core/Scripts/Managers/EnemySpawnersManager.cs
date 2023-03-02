using Perigon.Utility;
using UnityEngine;
using Logger = Perigon.Utility.Logger;

namespace BForBoss
{
    public class EnemySpawnersManager : MonoBehaviour, ISpawnerControl
    {
        private EnemySpawnAreaBehaviour[] _enemySpawnerAreas = {};

        public void Initialize(EnemyContainer enemyContainer, WaveModel waveModel = null)
        {
            foreach (var spawner in _enemySpawnerAreas)
            {
                spawner.Initialize(enemyContainer, waveModel);
            }
        }

        public void PauseSpawning()
        {
            Logger.LogString("Spawning Paused", LoggerColor.Green, "wavesmode");
            foreach (EnemySpawnAreaBehaviour spawner in _enemySpawnerAreas)
            {
                spawner.PauseSpawning();
            }
        }

        public void ResumeSpawning()
        {
            Logger.LogString("Spawning Resumed", LoggerColor.Green, "wavesmode");
            foreach (EnemySpawnAreaBehaviour spawner in _enemySpawnerAreas)
            {
                spawner.ResumeSpawning();
            }
        }

        public void Reset()
        {
            foreach (EnemySpawnAreaBehaviour spawner in _enemySpawnerAreas)
            {
                spawner.Reset();
            }
        }
        
        public void CleanUp()
        {
            foreach (EnemySpawnAreaBehaviour spawner in _enemySpawnerAreas)
            {
                spawner.CleanUp();
            }
        }

        private void Awake()
        {
            _enemySpawnerAreas = FindObjectsOfType<EnemySpawnAreaBehaviour>();
        }
    }
}
