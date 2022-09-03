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
        [SerializeField, Tooltip("Number of enemies per wave is the number of enemies from the previous wave multiplied by this multiplier")]
        private float _enemyAmountMultiplier = 1.2f;
        
        private ISpawnerControl _spawnerControl;
        private WaveModel _waveModel;
        
        private int _spawnCount = 0;

        public void Initialize(WaveModel waveModel, ISpawnerControl spawnerControl)
        {
            _spawnerControl = spawnerControl;

            _waveModel = waveModel;
            _waveModel.OnEnemySpawned += OnEnemySpawned;
            _waveModel.OnEnemyKilled += OnEnemyKilled;
            _waveModel.SetupInitialWave(_initialNumberOfEnemies);
        }

        public void Reset()
        {
            _spawnCount = 0;

            if (_waveModel != null)
            {
                _waveModel.ResetData();
            }
        }

        private void OnEnemySpawned()
        {
            _spawnCount++;

            if (_spawnCount >= _waveModel.MaxEnemyCount)
            {
                _spawnerControl.PauseSpawning();
            }
        }

        private void OnEnemyKilled(int numberOfRemainingEnemies)
        {
            if (numberOfRemainingEnemies <= 0)
            {
                Debug.Log($"Please wait <b>{_timeBetweenWaves} seconds</b> before the next wave");
                StartCoroutine(InitiateNextWave());
            }
        }

        private IEnumerator InitiateNextWave()
        {
            _spawnerControl.PauseSpawning();
            yield return new WaitForSeconds(_timeBetweenWaves);
            
            _spawnCount = 0;
            _waveModel.IncrementWave((int)Math.Ceiling(_waveModel.MaxEnemyCount * _enemyAmountMultiplier));
            _spawnerControl.ResumeSpawning();
        }

        private void OnDestroy()
        {
            if (_waveModel != null)
            {
                _waveModel.OnEnemySpawned -= OnEnemySpawned;
                _waveModel.OnEnemyKilled -= OnEnemyKilled;
            }
        }
    }
}
