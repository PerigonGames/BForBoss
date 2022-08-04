using System;
using UnityEngine;

namespace BForBoss
{
    public class EnemySpawnerManager : MonoBehaviour
    {
        private EnemySpawner[] _enemySpawners = null;
        //private Action _onEnemySpawned = null;

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
                // Add callback so that each time the spawner spawns the action is called
                spawner.Initialize(lifeCycleManager, onEnemySpawned, onEnemyKilled);
            }
        }

        private void Awake()
        {
            _enemySpawners = FindObjectsOfType<EnemySpawner>();
        }
    }
    
    //When the callback incentive is hit, Call WaveManager Callback - then reset current counter
    //The WaveManager should be make calls for the wave to end and when for the wave to start again
}
