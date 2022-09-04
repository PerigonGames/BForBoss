using System;
using UnityEngine;

namespace BForBoss
{
    public class WaveModel
    {
        private readonly bool _shouldPrintDebug = false;
        
        private int DEBUG_ACTIVE_SPAWN_COUNT;

        private int _waveNumber;
        private int _initialMaxEnemyCount;
        private int _maxEnemyCount;
        private int _killCount;
        private int _totalRoundTotalSpawnCount;

        public Action OnEnemySpawned;
        public Action OnEnemyKilled;
        public Action<int, int> OnDataUpdated;

        private int WaveNumber
        {
            get => _waveNumber;
            set
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
            }
        }

        private int RoundTotalSpawnCount
        {
            get => _totalRoundTotalSpawnCount;
            set
            {
                _totalRoundTotalSpawnCount = value;
                OnEnemySpawned?.Invoke();
            }
        }

        public bool IsMaxEnemySpawnedReached => _totalRoundTotalSpawnCount >= _maxEnemyCount;
        public bool IsRoundConcluded => _maxEnemyCount == KillCount;

        public WaveModel(bool shouldPrintDebug)
        {
            _shouldPrintDebug = shouldPrintDebug;
        }

        public void SetupInitialWave(int maxEnemyCount)
        {
            _initialMaxEnemyCount = maxEnemyCount;
            _maxEnemyCount = maxEnemyCount;
            IncrementWave(1);
        }

        public void IncrementSpawnCount()
        {
            DEBUG_ACTIVE_SPAWN_COUNT++;
            RoundTotalSpawnCount++;
            PrintDebug($"Enemy Spawned: <color=red>Active Spawn Count: {DEBUG_ACTIVE_SPAWN_COUNT}</color>");
            PrintDebug($"Enemy Spawned: <color=red>Round Spawn Count {_totalRoundTotalSpawnCount}</color>");
            PrintDebug($"===========================================");
        }

        public void IncrementKillCount()
        {
            DEBUG_ACTIVE_SPAWN_COUNT--;
            KillCount++;
            OnEnemyKilled?.Invoke();

            PrintDebug($"Enemy Killed:  <color=red>Active Spawn Count: {DEBUG_ACTIVE_SPAWN_COUNT}</color>");
            PrintDebug($"Enemy Killed:  <color=red>Round Spawn Count {_totalRoundTotalSpawnCount}</color>");
            PrintDebug($"===========================================");
        }

        public void IncrementWave(float maxAmountMultiplier)
        {
            _totalRoundTotalSpawnCount = 0;            
            _killCount = 0;
            _maxEnemyCount = (int)Mathf.Ceil(_maxEnemyCount * maxAmountMultiplier);
            WaveNumber++;
        }

        public void Reset()
        {
            RoundTotalSpawnCount = 0;
            _maxEnemyCount = _initialMaxEnemyCount;
            KillCount = 0;
            WaveNumber = 1;
        }

        private void PrintDebug(string log)
        {
            if (_shouldPrintDebug)
            {
                Debug.Log(log);
            }
        }
    }
}