using System;
using UnityEngine;

namespace BForBoss
{
    public class EnemySpawnerManager : MonoBehaviour
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

        public void Initialize(LifeCycleManager lifeCycleManager, Action onEnemySpawned = null, Action onEnemyKilled = null)
        {
            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.Initialize(lifeCycleManager, onEnemySpawned, onEnemyKilled);
            }
        }

        private void Awake()
        {
            _enemySpawners = FindObjectsOfType<EnemySpawner>();
        }
    }
}
