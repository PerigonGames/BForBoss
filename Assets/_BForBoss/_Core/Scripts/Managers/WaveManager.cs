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
        [SerializeField, Tooltip("Number of enemies per wave is the number of enemies from the previous wave multiplied by this multiplier")] private float _enemyAmountMultiplier = 1.2f;

        private int _maxCountForCurrentWave = 0;
        private int _spawnCount = 0;
        private int _killCount = 0;
        private int _waveNumber = 1;

        public void Initialize(LifeCycleManager lifeCycleManager)
        {
            _maxCountForCurrentWave = _initialNumberOfEnemies;
            _enemySpawnerManager.Initialize(lifeCycleManager, OnEnemySpawned, OnEnemyKilled);
        }

        public void Reset()
        {
            _maxCountForCurrentWave = _initialNumberOfEnemies;
            _spawnCount = 0;
            _killCount = 0;
            _waveNumber = 1;

            _enemySpawnerManager.Reset();
        }

        private void OnEnemySpawned()
        {
            _spawnCount++;

            if (_spawnCount >= _maxCountForCurrentWave)
            {
                PauseSpawning();
            }
        }

        private void OnEnemyKilled()
        {
            _killCount++;
            
            //Added the second clause just in case no. spawned > max allowed
            if (_killCount >= _maxCountForCurrentWave && _killCount == _spawnCount)
            {
                _enemySpawnerManager.Reset();
                Debug.Log($"<b>Wave {_waveNumber}</b> Ended. Please wait <b>{_timeBetweenWaves} seconds</b> before the next wave");
                StartCoroutine(InitiateNextWave());
            }
        }

        private IEnumerator InitiateNextWave()
        {
            yield return new WaitForSeconds(_timeBetweenWaves);
            
            _spawnCount = 0;
            _killCount = 0;
            _maxCountForCurrentWave = (int)Math.Ceiling(_maxCountForCurrentWave * _enemyAmountMultiplier);
            _waveNumber++;
            
            Debug.Log($"<b>Wave {_waveNumber}</b> has <b>{_maxCountForCurrentWave}</b> Enemies to kill. good luck");
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
