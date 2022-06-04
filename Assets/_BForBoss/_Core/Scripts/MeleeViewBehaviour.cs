using System.Collections;
using System.Collections.Generic;
using Perigon.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class MeleeViewBehaviour : MonoBehaviour
    {
        private Image _meleeView = null;
        private IEquipmentData _equipmentData = null;

        public void Initialize(IEquipmentData equipmentData)
        {
            _equipmentData = equipmentData;
        }

        private void Awake()
        {
            _meleeView = GetComponentInChildren<Image>();
        }

        private void Update()
        {
            _meleeView.fillAmount = GetElapsedDurationPercentageDone();
        }

        private float GetElapsedDurationPercentageDone()
        {
            if (_equipmentData != null)
            {
                return _equipmentData.CurrentMeleeCooldown / _equipmentData.MaxMeleeCooldown;
            }

            return 0;
        }
    }
}
