namespace BForBoss
{
    public class ItemWaveRespawnBehaviour : ItemRespawnBehaviour
    {
        private WaveModel _waveModel;
        private int _currentWaveNumber = 0;

        public void Initialize(WaveModel waveModel)
        {
            _waveModel = waveModel;
            _waveModel.OnDataUpdated += ResetItemIfWaveIncrements;
        }
        
        private void ResetItemIfWaveIncrements(int waveNumber, int enemyCount)
        {
            if (waveNumber == _currentWaveNumber)
            {
                return;
            }

            _canRespawn = true;
            _currentWaveNumber = waveNumber;
        }

        private void OnDestroy()
        {
            _waveModel.OnDataUpdated -= ResetItemIfWaveIncrements;
        }
    }
}
