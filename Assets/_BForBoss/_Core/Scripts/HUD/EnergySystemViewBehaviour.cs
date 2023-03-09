using System;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class EnergySystemViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _fillImage; 
        private IEnergyDataSubject _energyDataSubject;
        
        public void Initialize(IEnergyDataSubject energyDataSubject)
        {
            _energyDataSubject = energyDataSubject;
            _energyDataSubject.OnStateChanged += EnergyDataSubjectOnOnStateChanged;
        }

        private void EnergyDataSubjectOnOnStateChanged(EnergyData data)
        {
            _fillImage.fillAmount = data.Value / Math.Max(data.MaxEnergyValue, 1f);
        }

        private void Awake()
        {
            if (_fillImage == null)
            {
                PanicHelper.Panic(new Exception("Missing Image from EnergySystemViewBehaviour"));
            }
        }
    }
}
