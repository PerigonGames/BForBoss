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
        public Action<int> OnEnemyKilled;
        public Action<int, int> OnWaveCountUpdated;
        
        public int WaveNumber
        {
            get => _waveNumber;
            private set => _waveNumber = value;
        }

        public int MaxEnemyCount
        {
            get => _maxEnemyCount;
            private set => _maxEnemyCount = value;
        }

        private int KillCount
        {
            get => _killCount; 
            set
            {
                _killCount = value;
                OnEnemyKilled?.Invoke(_maxEnemyCount - _killCount);
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

        public bool IsMaxEnemySpawnedReached => _spawnCount >= MaxEnemyCount;

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
        }

        public void IncrementWave(float maxAmountMultiplier)
        {
            _spawnCount = 0;            
            _killCount = 0;
            WaveNumber++;
            MaxEnemyCount = (int)(_maxEnemyCount * maxAmountMultiplier);            
            OnWaveCountUpdated?.Invoke(_waveNumber, _maxEnemyCount);
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