using Perigon.Utility;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class WaveViewBehaviour : MonoBehaviour
    {
        private const string WAVES = "Wave";
        private const string REMAINING = "Remaining";
        [SerializeField, Resolve] private TMP_Text _waveCounterLabel = null;
        [SerializeField, Resolve] private TMP_Text _enemiesRemainingLabel = null;

        private WaveModel _model = null;

        public void Initialize(WaveModel model)
        {
            model.OnEnemyKilled += UpdateEnemiesRemainingCounter;
            model.OnWaveCountUpdated += UpdateWaveCounter;
            _model = model;
        }

        private void UpdateEnemiesRemainingCounter(int numberOfRemainingEnemies)
        {
            _enemiesRemainingLabel.text = numberOfRemainingEnemies.ToString();
        }

        private void UpdateWaveCounter(int waveNumber, int maxEnemyCount)
        {
            _waveCounterLabel.text = $"{WAVES}\n<color=yellow>{waveNumber.ToString()}</color>";
            _enemiesRemainingLabel.text = $"{REMAINING}\n<color=red>{maxEnemyCount.ToString()}</color>";
        }
        
        private void OnDestroy()
        {
            if (_model != null)
            {
                _model.OnEnemyKilled -= UpdateEnemiesRemainingCounter;
                _model.OnWaveCountUpdated -= UpdateWaveCounter;
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
