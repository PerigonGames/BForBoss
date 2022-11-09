using System.Collections;
using UnityEngine;
using Logger = Perigon.Utility.Logger;

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
            StartCoroutine(InitiateNextWave());
        }

        public void Reset()
        {
            StartCoroutine(InitiateNextWave());
        }

        private void OnEnemySpawned()
        {
            if (_waveModel.IsMaxEnemySpawnedReached)
            {
                Logger.LogString("<color=Blue>Max Enemy Reached</color>", "wavesmode");
                _spawnerControl.PauseAllSpawning();
            }
        }

        private void OnEnemyKilled(int index)
        {
            if (_waveModel.IsRoundConcluded)
            {
                Logger.LogString("<color=green>Wave Over</color>", "wavesmode");
                Logger.LogString($"Please wait <color=green><b>{_secondsBetweenWaves} seconds</b></color> before the next wave", "wavesmode");
                StartCoroutine(InitiateNextWave());
            }
            else if (!_waveModel.IsMaxEnemySpawnedReached)
            {
                _spawnerControl.ResumeSpawning(index);
            }
        }

        private IEnumerator InitiateNextWave()
        {
            if (_spawnerControl != null)
            {
                _spawnerControl.PauseAllSpawning();
                yield return new WaitForSeconds(_secondsBetweenWaves);
            
                _waveModel.IncrementWave(_enemyAmountMultiplier);
                _spawnerControl.ResumeAllSpawning();
            }
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
