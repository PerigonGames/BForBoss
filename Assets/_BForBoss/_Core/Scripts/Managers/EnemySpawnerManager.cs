using UnityEngine;

namespace BForBoss
{
    public class EnemySpawnerManager : MonoBehaviour, ISpawnerControl
    {
        private EnemySpawner[] _enemySpawners = new EnemySpawner[]{};

        public void Reset()
        { 
            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.Reset();
            }
        }
        
        public void PauseSpawning()
        {
            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.PauseSpawning();
            }
        }

        public void ResumeSpawning()
        {
            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.ResumeSpawning();
            }
        }

        public void Initialize(EnemyBehaviourManager enemyBehaviourBehaviourManager, WaveModel waveModel = null)
        {
            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.Initialize(enemyBehaviourBehaviourManager, waveModel);
            }
        }

        private void Awake()
        {
            _enemySpawners = FindObjectsOfType<EnemySpawner>();
        }
    }
}
