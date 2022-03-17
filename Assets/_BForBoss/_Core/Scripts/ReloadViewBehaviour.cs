using System;
using Perigon.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class ReloadViewBehaviour : MonoBehaviour
    {
        private Image _reloadView = null;
        private IEquipmentData _equipmentData = null;

        public void Initialize(IEquipmentData equipmentData)
        {
            _equipmentData = equipmentData;
        }

        private void Awake()
        {
            _reloadView = GetComponentInChildren<Image>();
        }

        private void Update()
        {
            _reloadView.fillAmount = GetElapsedDurationPercentageDone();
        }

        private float GetElapsedDurationPercentageDone()
        {
            if (_equipmentData != null)
            {
                return 1 - _equipmentData.ElapsedReloadDuration / _equipmentData.MaxReloadDuration;
            }

            return 0;
        }
    }
}
