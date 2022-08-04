using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WaveManager : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager;
        
        [Title("Settings")] 
        [SerializeField, Tooltip("Number of enemies for the first wave")] private int _initialNumberOfEnemies = 10;
        [SerializeField, Tooltip("Cooldown time between waves")] private float _timeBetweenWaves = 2.5f;
        [SerializeField, Tooltip("Number of enemies per wave is the number of enemies from the previous wave multiplied by this multiplier")] private float _EnemyAmountMultiplier = 1.2f;

        private int _maxEnemyCountForCurrentWave = 0;
        private int _currentEnemySpawnCount = 0;
        private int _currentEnemyKillCount = 0;
        private int _currentWaveNumber = 1;

        public void Initialize(LifeCycleManager lifeCycleManager)
        {
            _maxEnemyCountForCurrentWave = _initialNumberOfEnemies;
            _enemySpawnerManager.Initialize(lifeCycleManager, OnEnemySpawned, OnEnemyKilled);
        }

        public void Reset()
        {
            _maxEnemyCountForCurrentWave = _initialNumberOfEnemies;
            _currentEnemySpawnCount = 0;
            _currentEnemyKillCount = 0;
            _currentWaveNumber = 1;

            _enemySpawnerManager.Reset();
        }

        private void OnEnemySpawned()
        {
            _currentEnemySpawnCount++;
            Debug.Log($"<color=blue>Current Enemy Spawn Count : {_currentEnemySpawnCount}</color>");

            if (_currentEnemySpawnCount >= _maxEnemyCountForCurrentWave)
            {
                Debug.Log($"All {_maxEnemyCountForCurrentWave} spawned");
                PauseSpawning();
            }
        }

        private void OnEnemyKilled()
        {
            _currentEnemyKillCount++;
            Debug.Log($"<color=red>Current Enemy Kill Count : {_currentEnemyKillCount}</color>");
            
            //Add the second clause just in case no spawned > max allowed
            if (_currentEnemyKillCount >= _maxEnemyCountForCurrentWave && _currentEnemyKillCount == _currentEnemySpawnCount)
            {
                _enemySpawnerManager.Reset();
                Debug.Log($"Wave {_currentWaveNumber} Ended. Please wait {_timeBetweenWaves} seconds before next wave");
                StartCoroutine(InitiateNextWave());
            }
        }

        private IEnumerator InitiateNextWave()
        {
            yield return new WaitForSeconds(_timeBetweenWaves);
            
            _currentEnemySpawnCount = 0;
            _currentEnemyKillCount = 0;
            _maxEnemyCountForCurrentWave = (int)Math.Ceiling(_maxEnemyCountForCurrentWave * _EnemyAmountMultiplier);
            _currentWaveNumber++;
            
            Debug.Log($"Wave {_currentWaveNumber} is starting with {_maxEnemyCountForCurrentWave} Enemies. good luck");
            ResumeSpawning();
        }

        private void PauseSpawning()
        {
            _enemySpawnerManager.PauseSpawning();
        }

        private void ResumeSpawning()
        {
            _enemySpawnerManager.ResumeSpawning();
        }

        private void OnValidate()
        {
            if (_enemySpawnerManager == null)
            {
                Debug.LogWarning("Enemy Spawner Manager missing from the Wave Manager");
            }
        }
    }
}
