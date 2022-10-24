using System.Collections;
using Perigon.Entities;
using Perigon.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;
using Logger = Perigon.Utility.Logger;
using Random = UnityEngine.Random;

namespace BForBoss
{
    public class EnemySpawnAreaBehaviour : MonoBehaviour
    {
        [SerializeField] [Min(1)] private int _enemiesToSpawn = 3;
        [SerializeField] [Min(0.0f)] private float _secondsBetweenSpawns = 2.5f;
        [SerializeField, Tooltip("Should max amount of enemies spawn upon initialization")]
        private bool _burstInitialSpawn = true;
        [SerializeField, Range(1, 10)] 
        [InfoBox("Spawn Radius dictates the area to check for empty space for an AI to spawn. HOWEVER, NavMesh.FindClosestEdge is used, and could return a position outside the area.")]
        private float _spawnRadius = 1;
        
        private bool _canSpawn = true;
        private WaveModel _waveModel;
        private EnemyContainer _enemyContainer;
        
        public void Initialize(EnemyContainer enemyContainer, WaveModel waveModel)
        {
            _enemyContainer = enemyContainer;
            _waveModel = waveModel;

            SpawnInitialEnemies();
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

        public void Reset()
        {
            _canSpawn = true;
        }

        public void CleanUp()
        {
            _canSpawn = false;
            StopCoroutine(nameof(SpawnEnemies));
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
                yield return new WaitForSeconds(_secondsBetweenSpawns);
                
                if (!_canSpawn)
                {
                    yield break;
                }
                
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            Logger.LogString("<color=red>Spawn Enemy</color>", "wavesmode");
            Vector3? spawnPosition = GenerateSpawnPosition();
            if (spawnPosition != null)
            {
                var enemy = _enemyContainer.GetEnemy();
            
                enemy.OnDeath += HandleOnEnemyDeath;
                enemy.transform.SetPositionAndRotation((Vector3)spawnPosition, transform.rotation);
                enemy.transform.SetParent(transform, true);
            
                //https://answers.unity.com/questions/771908/navmesh-issue-with-spawning-players.html
                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                agent.enabled = false;
                agent.enabled = true;
            
                _waveModel?.IncrementSpawnCount();
            }
        }

        private Vector3? GenerateSpawnPosition()
        {
            if (IsPositionValid(transform.position))
            {
                return transform.position;
            }

            const int MaxRetries = 10;
            for(int tries = 0; tries < MaxRetries; tries++)
            {
                var randomPosition = _spawnRadius * Random.insideUnitSphere;
                var randomSpawnPosition = transform.position +
                                new Vector3(randomPosition.x, 0, randomPosition.z);

                if (NavMesh.FindClosestEdge(randomSpawnPosition, out var hit, NavMesh.AllAreas) && 
                    IsPositionValid(hit.position))
                {
                    return hit.position;
                }
            }

            return null;
        }

        private bool IsPositionValid(Vector3 position)
        {
            const int halfExtentSize = 2;
            return Physics.OverlapBox(position, halfExtents: Vector3.one * halfExtentSize, Quaternion.identity, TagsAndLayers.Layers.Enemy)
                .IsNullOrEmpty();
        }

        private void HandleOnEnemyDeath(EnemyBehaviour enemy)
        {
            enemy.OnDeath -= HandleOnEnemyDeath;
            _waveModel?.IncrementKillCount();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 1, 0.3f);
            Gizmos.DrawCube(transform.position, Vector3.one * _spawnRadius);
        }
    }
}
