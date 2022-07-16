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
            _enemySpawners = FindObjectsOfType<EnemySpawner>();

            if (_enemySpawners == null)
            {
                return;
            }

            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.Initialize(lifeCycleManager);
            }
        }
    }
}
