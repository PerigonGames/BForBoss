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
            if (_waveModel != null)
            {
                _waveModel.Reset();
            }
        }

        private void OnEnemySpawned()
        {
            if (_waveModel.IsMaxEnemySpawnedReached)
            {
                Debug.Log("<color=Blue>Max Enemy Reached</color>");
                _spawnerControl.PauseSpawning();
            }
        }

        private void OnEnemyKilled(int numberOfRemainingEnemies)
        {
            if (numberOfRemainingEnemies <= 0)
            {
                Debug.Log("<color=green>Round Over</color>");
                Debug.Log($"Please wait <b>{_timeBetweenWaves} seconds</b> before the next wave");
                StartCoroutine(InitiateNextWave());
            }
        }

        private IEnumerator InitiateNextWave()
        {
            _spawnerControl.PauseSpawning();
            yield return new WaitForSeconds(_timeBetweenWaves);
            
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
