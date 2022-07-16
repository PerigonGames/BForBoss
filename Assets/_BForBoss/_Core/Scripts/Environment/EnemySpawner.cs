using System.Collections;
using Perigon.Entities;
using UnityEngine;
using UnityEngine.Pool;

namespace BForBoss
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyBehaviour _enemyAgent;
        [SerializeField] [Min(1)] private int _enemiesToSpawn = 3;
        [SerializeField] [Min(0.0f)] private float _timeBetweenSpawns = 2.5f; //In seconds

        [SerializeField, Tooltip("Should max amount of enemies spawn on initialization")]
        private bool _burstInitialSpawn = true;

        private ObjectPool<EnemyBehaviour> _pool;
        private LifeCycleManager _lifeCycleManager = null;

        public void Initialize(LifeCycleManager lifeCycleManager)
        {
            _lifeCycleManager = lifeCycleManager;
            
            if (_pool == null)
            {
                SetupPool();
            }
            
            if (_burstInitialSpawn)
            {
                for (int i = 0; i < _enemiesToSpawn; i++)
                {
                    _lifeCycleManager.AddEnemyBehaviourFromSpawner(_pool.Get(), Release);
                }
            }
            else
            {
                StartCoroutine(SpawnEnemies(_enemiesToSpawn));
            }
        }

        public void Reset()
        {
            if (_pool != null)
            {
                _pool.Clear();
            }
        }
        
        private void SetupPool()
       {
           _pool = new ObjectPool<EnemyBehaviour>(() => GenerateEnemy(_enemyAgent),
                (enemy) =>
                {
                    Transform spawnTransform = transform;
                    enemy.transform.SetParent(spawnTransform);
                    enemy.transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
                    enemy.gameObject.SetActive(true);
                },
                (enemy) =>
                {
                    enemy.gameObject.SetActive(false);
                });
        }

        private void Release(EnemyBehaviour behaviour)
        {
            _pool.Release(behaviour);
            StartCoroutine(SpawnEnemies(1));
        }

        private IEnumerator SpawnEnemies(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(_timeBetweenSpawns);
                _lifeCycleManager.AddEnemyBehaviourFromSpawner(_pool.Get(), Release);
            }
        }

        private EnemyBehaviour GenerateEnemy(EnemyBehaviour enemyAgent)
        {
            return Instantiate(enemyAgent);
        }
    }
}
