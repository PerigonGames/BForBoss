using System;

namespace BForBoss
{
    public class WaveModel
    {
        private int _waveNumber;
        private int _initialMaxEnemyCount;
        private int _maxEnemyCount;
        private int _killCount;

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

        public int KillCount
        {
            get => _killCount;
            private set
            {
                _killCount = value;
                OnEnemyKilled?.Invoke(_maxEnemyCount - _killCount);
            }
            
        }

        public void SetupInitialWave(int maxEnemyCount)
        {
            _initialMaxEnemyCount = maxEnemyCount;
            IncrementWave(maxEnemyCount);
        }

        public void IncrementSpawnCount()
        {
            OnEnemySpawned?.Invoke();
        }

        public void IncrementKillCount()
        {
            KillCount++;
        }

        public void IncrementWave(int newMaxEnemyCount)
        {
            WaveNumber++;
            MaxEnemyCount = newMaxEnemyCount;
            OnWaveCountUpdated?.Invoke(_waveNumber, _maxEnemyCount);
            _killCount = 0;
        }

        public void ResetData()
        {
            WaveNumber = 1;
            MaxEnemyCount = _initialMaxEnemyCount;
            OnWaveCountUpdated?.Invoke(_waveNumber, _maxEnemyCount);
            _killCount = 0;
        }
    }
}