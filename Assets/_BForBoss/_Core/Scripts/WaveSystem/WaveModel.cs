using System;
using UnityEngine;

namespace BForBoss
{
    public class WaveModel
    {
        private int _waveNumber;
        private int _initialMaxEnemyCount;
        private int _maxEnemyCount;
        private int _killCount;
        
        private int _spawnCount;

        public Action OnEnemySpawned;
        public Action OnEnemyKilled;
        public Action<int, int> OnDataUpdated;
        
        public int WaveNumber
        {
            get => _waveNumber;
            private set
            {
                _waveNumber = value;
                OnDataUpdated?.Invoke(_waveNumber, _maxEnemyCount - _killCount);
            }
        }

        private int KillCount
        {
            get => _killCount; 
            set
            {
                _killCount = value;
                OnDataUpdated?.Invoke(_waveNumber, _maxEnemyCount - _killCount);
                OnEnemyKilled?.Invoke();
            }
        }

        private int SpawnCount
        {
            get => _spawnCount;
            set
            {
                _spawnCount = value;
                OnEnemySpawned?.Invoke();
            }
        }

        public bool IsMaxEnemySpawnedReached => _spawnCount >= _maxEnemyCount;
        public bool IsEnemiesAllDead => _maxEnemyCount == KillCount; 

        public void SetupInitialWave(int maxEnemyCount)
        {
            _initialMaxEnemyCount = maxEnemyCount;
            _maxEnemyCount = maxEnemyCount;
            IncrementWave(1);
        }

        public void IncrementSpawnCount()
        {
            SpawnCount++;
            Debug.Log($"Spawn Count: {_spawnCount}");
        }

        public void IncrementKillCount()
        {
            KillCount++;
            SpawnCount--;
            Debug.Log($"Spawn Count: {_spawnCount}");
        }

        public void IncrementWave(float maxAmountMultiplier)
        {
            _spawnCount = 0;            
            _killCount = 0;
            WaveNumber++;
            _maxEnemyCount = (int)(_maxEnemyCount * maxAmountMultiplier);            
        }

        public void Reset()
        {
            _spawnCount = 0;
            _waveNumber = 1;
            _maxEnemyCount = _initialMaxEnemyCount;
            _killCount = 0;
        }
    }
}