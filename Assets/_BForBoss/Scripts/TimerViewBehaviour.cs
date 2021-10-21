using System;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class TimerViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeLabel = null;
        public void Initialize(TimeManagerViewModel timeManagerViewModel)
        {
            timeManagerViewModel.OnTimeChanged += HandleOnOnTimeChanged;
        }

        private void HandleOnOnTimeChanged(float time)
        {
            _timeLabel.text = time.ToString("F");
        }

        public void Reset()
        {
            _timeLabel.text = "0.0";
        }
    }
}
