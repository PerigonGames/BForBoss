using UnityEngine;

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
            Debug.Log("<color=green>Spawn Paused</color>");
            foreach (EnemySpawnAreaBehaviour spawner in _enemySpawnerAreas)
            {
                spawner.PauseSpawning();
            }
        }

        public void ResumeSpawning()
        {
            Debug.Log("<color=green>Spawn Resumed</color>");
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
