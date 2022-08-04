using System;
using UnityEngine;

namespace BForBoss
{
    public class EnemySpawnerManager : MonoBehaviour
    {
        private EnemySpawner[] _enemySpawners = null;

        public void Reset()
        {
            if (_enemySpawners == null)
            {
                return;
            }

            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.Reset();
            }
        }

        public void PauseSpawning()
        {
            if (_enemySpawners == null)
            {
                return;
            }

            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.PauseSpawning();
            }
        }

        public void ResumeSpawning()
        {
            if (_enemySpawners == null)
            {
                return;
            }

            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.ResumeSpawning();
            }
        }

        public void Initialize(LifeCycleManager lifeCycleManager, Action onEnemySpawned = null, Action onEnemyKilled = null)
        {
            if (_enemySpawners == null)
            {
                return;
            }

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
