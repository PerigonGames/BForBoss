using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WaveManager : MonoBehaviour
    {
        [Title("Settings")] 
        [SerializeField, Tooltip("Number of enemies for the first wave")] private int _initialNumberOfEnemies = 10;
        [SerializeField, Tooltip("Cooldown time between waves")] private float _timeBetweenWaves = 2.5f;
        [SerializeField, Tooltip("Number of enemies per wave is the number of enemies from the previous wave multiplied by this multiplier")] private float _enemyAmountMultiplier = 1.2f;


        private EnemySpawnerManager _enemySpawnerManager;
        private WaveModel _waveModel;
        
        private int _spawnCount = 0;

        public void Initialize(LifeCycleManager lifeCycleManager, EnemySpawnerManager enemySpawnerManager, WaveModel waveModel)
        {
            _enemySpawnerManager = enemySpawnerManager;

            _waveModel = waveModel;
            _waveModel.waveNumber = 1;
            _waveModel.maxEnemyCountforWave = _initialNumberOfEnemies;
            _waveModel.killCount = 0;

            _waveModel.OnEnemySpawned += OnEnemySpawned;
            _waveModel.OnEnemyKilled += OnEnemyKilled;
            
            _enemySpawnerManager.Initialize(lifeCycleManager, waveModel);
        }

        public void Reset()
        {
            _spawnCount = 0;

            if (_waveModel != null)
            {
                _waveModel.ResetData();
                _waveModel.OnEnemySpawned -= OnEnemySpawned;
                _waveModel.OnEnemyKilled -= OnEnemyKilled;
            }

            if (_enemySpawnerManager != null)
            {
                _enemySpawnerManager.Reset();
            }
        }

        private void OnEnemySpawned()
        {
            _spawnCount++;

            if (_spawnCount >= _waveModel.maxEnemyCountforWave)
            {
                PauseSpawning();
            }
        }

        private void OnEnemyKilled()
        {
            _waveModel.killCount++;

            //Added the second clause just in case no. spawned > max allowed
            if (_waveModel.killCount >= _waveModel.maxEnemyCountforWave && _waveModel.killCount == _spawnCount)
            {
                Debug.Log($"<b>Wave {_waveModel.waveNumber}</b> Ended. Please wait <b>{_timeBetweenWaves} seconds</b> before the next wave");
                StartCoroutine(InitiateNextWave());
            }
        }

        private IEnumerator InitiateNextWave()
        {
            yield return new WaitForSeconds(_timeBetweenWaves);
            
            _spawnCount = 0;
            _waveModel.killCount = 0;
            _waveModel.maxEnemyCountforWave = (int)Math.Ceiling(_waveModel.maxEnemyCountforWave * _enemyAmountMultiplier);
            _waveModel.waveNumber++;
            
            Debug.Log($"<b>Wave {_waveModel.waveNumber}</b> has <b>{_waveModel.maxEnemyCountforWave}</b> Enemies to kill. good luck");
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
