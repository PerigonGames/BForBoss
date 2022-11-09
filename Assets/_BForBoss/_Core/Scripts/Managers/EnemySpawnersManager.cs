using UnityEngine;
using Logger = Perigon.Utility.Logger;

namespace BForBoss
{
    public class EnemySpawnersManager : MonoBehaviour, ISpawnerControl
    {
        private EnemySpawnAreaBehaviour[] _enemySpawnerAreas = {};

        public void Initialize(EnemyContainer enemyContainer, WaveModel waveModel = null)
        {
            // foreach (var spawner in _enemySpawnerAreas)
            // {
            //     spawner.Initialize(enemyContainer, waveModel);
            // }

            for (int i = 0; i < _enemySpawnerAreas.Length; i++)
            {
                _enemySpawnerAreas[i].Initialize(enemyContainer,waveModel,i);
            }
        }

        public void PauseAllSpawning()
        {
            Logger.LogString("<color=green>Spawning Paused</color>", "wavesmode");
            foreach (EnemySpawnAreaBehaviour spawner in _enemySpawnerAreas)
            {
                spawner.PauseSpawning();
            }
        }

        public void ResumeAllSpawning()
        {
            Logger.LogString("<color=green>Spawning Resumed</color>", "wavesmode");
            foreach (EnemySpawnAreaBehaviour spawner in _enemySpawnerAreas)
            {
                spawner.ResumeSpawningIfPossible();
            }
        }

        public void ResumeSpawning(int spawnerIndex)
        {
            EnemySpawnAreaBehaviour spawner = _enemySpawnerAreas[spawnerIndex];

            if (spawner != null)
            {
                Logger.LogString("<color=green>Spawning Resumed</color>", "wavesmode");
                _enemySpawnerAreas[spawnerIndex].ResumeSpawningIfPossible();
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
