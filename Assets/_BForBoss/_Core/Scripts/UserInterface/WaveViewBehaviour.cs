using Perigon.Utility;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class WaveViewBehaviour : MonoBehaviour
    {
        [SerializeField, Resolve] private TMP_Text _waveCounterLabel = null;
        [SerializeField, Resolve] private TMP_Text _enemiesRemainingLabel = null;

        private WaveModel _model = null;

        public void Initialize(WaveModel model)
        {
            model.OnEnemyKillCountChanged += UpdateEnemiesRemainingCounter;
            model.OnWaveCountChanged += UpdateWaveCounter;
            _model = model;
        }

        private void UpdateEnemiesRemainingCounter(int remainingEnemiesCount)
        {
            _enemiesRemainingLabel.text = remainingEnemiesCount.ToString();
        }

        private void UpdateWaveCounter(int waveNumber, int maxEnemyCount)
        {
            _waveCounterLabel.text = waveNumber.ToString();
            _enemiesRemainingLabel.text = maxEnemyCount.ToString();
        }

        private void OnDestroy()
        {
            if (_model != null)
            {
                _model.OnEnemyKillCountChanged -= UpdateEnemiesRemainingCounter;
                _model.OnWaveCountChanged -= UpdateWaveCounter;
            }
        }

        private void OnValidate()
        {
            if (_waveCounterLabel == null)
            {
                Debug.LogWarning("Wave Counter Label is missing from Wave View UI");
            }

            if (_enemiesRemainingLabel == null)
            {
                Debug.LogWarning("Enemies Remaining Label is missing from Wave View UI");
            }
        }
    }
}
