using Perigon.Entities;
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

        public void Initialize(LifeCycleManager lifeCycleManager)
        {
            if (_enemySpawners == null)
            {
                return;
            }

            foreach (EnemySpawner spawner in _enemySpawners)
            {
                // Add callback so that each time the spawner spawns the action is called
                spawner.Initialize(lifeCycleManager);
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
