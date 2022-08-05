using System;

namespace BForBoss
{
    public class WaveModel
    {
        public int waveNumber;
        public int maxEnemyCountforWave;
        public int killCount;

        public Action OnEnemySpawned;
        public Action OnEnemyKilled;

        public void ResetData()
        {
            waveNumber = 0;
            maxEnemyCountforWave = 0;
            killCount = 0;
        }
    }
}