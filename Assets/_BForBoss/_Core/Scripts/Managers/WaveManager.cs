using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField, Tooltip("Number of enemies per wave is the number of enemies from the previous wave multiplied by this multiplier")] private float _EnemyAmountMultiplier = 1.2f;

        private int _maxEnemyCountForCurrentWave;
        private int _currentEnemyCount = 0;
        private int _currentWaveNumber = 1;

        public Action onEnemySpawned;

        public void Initialize()
        {
            _maxEnemyCountForCurrentWave = _initialNumberOfEnemies;
            //Create
            
            //Spawn Enemy Manager from here
        }

        private void OnEnemySpawned()
        {
            _currentEnemyCount++;

            if (_currentEnemyCount >= _maxEnemyCountForCurrentWave)
            {
                // Maybe nullify current onEnemySpawned to ensure this method is called
                Debug.Log($"Wave {_currentWaveNumber} completed");
            }
        }
        
    }
}
