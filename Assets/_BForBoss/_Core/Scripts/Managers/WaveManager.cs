using System.Collections;
using UnityEngine;

namespace BForBoss
{
    public class WaveManager : MonoBehaviour
    {
        private float _secondsBetweenWaves;
        private float _enemyAmountMultiplier;
        
        private ISpawnerControl _spawnerControl;
        private WaveModel _waveModel;
        private EnemyContainer _enemyContainer;
        
        public void Initialize(WaveModel waveModel, 
            ISpawnerControl spawnerControl,
            float secondsBetweenWaves,
            float enemyAmountMultiplier)
        {
            _spawnerControl = spawnerControl;
            _secondsBetweenWaves = secondsBetweenWaves;
            _enemyAmountMultiplier = enemyAmountMultiplier;

            _waveModel = waveModel;
            _waveModel.OnEnemySpawned += OnEnemySpawned;
            _waveModel.OnEnemyKilled += OnEnemyKilled;
        }

        public void Reset()
        {
            StartCoroutine(InitiateNextWave());
        }

        private void OnEnemySpawned()
        {
            if (_waveModel.IsMaxEnemySpawnedReached)
            {
                Debug.Log("<color=Blue>Max Enemy Reached</color>");
                _spawnerControl.PauseSpawning();
            }
        }

        private void OnEnemyKilled()
        {
            if (_waveModel.IsRoundConcluded)
            {
                Debug.Log("<color=green>Wave Over</color>");
                Debug.Log($"Please wait <color=green><b>{_secondsBetweenWaves} seconds</b></color> before the next wave");
                StartCoroutine(InitiateNextWave());
            }
            else if (!_waveModel.IsMaxEnemySpawnedReached)
            {
                _spawnerControl.ResumeSpawning();
            }
        }

        private IEnumerator InitiateNextWave()
        {
            _spawnerControl.PauseSpawning();
            yield return new WaitForSeconds(_secondsBetweenWaves);
            
            _waveModel.IncrementWave(_enemyAmountMultiplier);
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
