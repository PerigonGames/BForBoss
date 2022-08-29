using System.Collections;
using Perigon.Entities;
using UnityEngine;
namespace BForBoss
{
    
    //Let EnemySpawnerManager have getPlayerPosition and bulletSpawner
    //And EnemyManager does not have such need
    
    //Better Yet Initialize The enemy Manager with getPlayerPosition and bulletSpawner
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyBehaviour _enemyAgent; //Todo: This should probably be an Enum instead of a direct reference to class
        [SerializeField] [Min(1)] private int _enemiesToSpawn = 3;
        [SerializeField] [Min(0.0f)] private float _timeBetweenSpawns = 2.5f; //In seconds
        [SerializeField, Tooltip("Should max amount of enemies spawn upon initialization")]
        private bool _burstInitialSpawn = true;
        
        private bool _canSpawn = true;
        private WaveModel _waveModel;
        private EnemyContainer _container;

        //private EnemyBehaviourManager _enemyBehaviourManager;

        public void Initialize(EnemyBehaviourManager enemyBehaviourBehaviourManager, WaveModel waveModel)
        {
            _waveModel = waveModel;
            _container = new EnemyContainer(_enemyAgent, enemyBehaviourBehaviourManager, Release);


            //_enemyBehaviourManager = enemyBehaviourBehaviourManager;
            // if (_pool == null)
            // {
            //     SetupPool();
            // }
            
            SpawnInitialEnemies();
        }

        public void Reset()
        {
            if (_container != null)
            {
                _container.Reset();
            }
        }

        public void PauseSpawning()
        {
            _canSpawn = false;
            StopCoroutine(nameof(SpawnEnemies));
        }

        public void ResumeSpawning()
        {
            _canSpawn = true;
            SpawnInitialEnemies();
        }
        
       //  private void SetupPool()
       // {
       //     _pool = new ObjectPool<EnemyBehaviour>(() => GenerateEnemy(_enemyAgent),
       //          (enemy) =>
       //          {
       //              Transform spawnTransform = transform;
       //              enemy.transform.SetParent(spawnTransform);
       //              enemy.transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
       //              enemy.gameObject.SetActive(true);
       //          },
       //          (enemy) =>
       //          {
       //              enemy.gameObject.SetActive(false);
       //          });
       //  }

        private void Release()
        {
            //_pool.Release(behaviour);
            _waveModel?.IncrementKillCount();
            StartCoroutine(SpawnEnemies(1));
        }
        
        private void SpawnInitialEnemies()
        {
            if (_burstInitialSpawn)
            {
                for (int i = 0; i < _enemiesToSpawn; i++)
                {
                    if (!_canSpawn)
                    {
                        return;
                    }

                    SpawnEnemy();
                }
            }
            else
            {
                StartCoroutine(SpawnEnemies(_enemiesToSpawn));
            }
        }

        private IEnumerator SpawnEnemies(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(_timeBetweenSpawns);
                
                if (!_canSpawn)
                {
                    yield break;
                }
                
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            //_lifeCycleManager.AddEnemyBehaviourFromSpawner(_pool.Get(), Release);
            EnemyBehaviour enemy = GetEnemy();
            _waveModel?.IncrementSpawnCount();
        }

        private EnemyBehaviour GenerateEnemy(EnemyBehaviour enemyAgent)
        {
            return Instantiate(enemyAgent);
        }

        private EnemyBehaviour GetEnemy()
        {
            Transform spawnTransform = transform;
            EnemyBehaviour enemy = Instantiate(_container.Get(), spawnTransform.position, spawnTransform.rotation,
                spawnTransform);
            
            // FloatingTargetBehaviour floatingEnemy = enemy as FloatingTargetBehaviour;
            //
            // if (floatingEnemy != null)
            // {
            //     floatingEnemy.Initialize(_enemyBehaviourManager.GetPlayerPosition, _enemyBehaviourManager.BulletSpawner, null,
            //         (enemy) =>
            //         {
            //             _container.EnemyPool.Release(enemy);
            //             enemy.gameObject.SetActive(false);
            //             Release();
            //         });
            //
            //     return floatingEnemy;
            // }
            
            enemy.gameObject.SetActive(true);
            return enemy;
        }
    }
}
