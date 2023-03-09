using System;
using System.Collections.Generic;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = Perigon.Utility.Logger;

namespace BForBoss
{
    public interface IEnergyDataSubject
    {
        public event Action<EnergyData> OnStateChanged;
    }
    
    public interface IEnergySystem
    {
        void Accrue(EnergyAccruementType accruementType, float multiplier = 1);
        void Expend(EnergyExpenseType expenseType, float multiplier = 1);
        bool CanExpend(EnergyExpenseType expenseType, float multiplier = 1);
    }
    
    public partial class EnergySystemBehaviour : MonoBehaviour, IEnergySystem, IEnergyDataSubject
    {
        [InlineEditor] [SerializeField] private EnergySystemConfigurationSO _energySystemConfiguration;
        [InlineEditor] [SerializeField] private EnergySO _energy;


        private EnergyData _energyData;
        public EnergyData EnergyData
        {
            private set
            {
                _energyData = value;
                OnStateChanged?.Invoke(_energyData);
                Logger.LogFormat($"Energy Value: {_energyData.Value}", key: "energysystem");  
            } 

            get => _energyData;
        }
        
        public event Action<EnergyData> OnStateChanged;

        private EnergySystemConfigurationData _energySystemConfigurationData;

        private readonly Queue<(EnergyAccruementType, float)> _accruedEnergyTypeQueue = new();
        private readonly Queue<(EnergyExpenseType, float)> _expendEnergyTypeQueue = new();

        private float _elapsedTime;

        public void Reset()
        {
            EnergyData = _energy.MapToData();
            _accruedEnergyTypeQueue.Clear();
            _expendEnergyTypeQueue.Clear();
            _elapsedTime = 0;
        }
        
        public void Accrue(EnergyAccruementType accruementType, float multiplier = 1)
        {
            switch (accruementType)
            {
                case EnergyAccruementType.WallRun:
                    _accruedEnergyTypeQueue.Enqueue((accruementType, multiplier));
                    break;
                default:
                    AccrueEnergyImmediately(accruementType, multiplier);
                    break;
            }
        }

        public void Expend(EnergyExpenseType expenseType, float multiplier = 1)
        {
            switch (expenseType)
            {
                case EnergyExpenseType.SlowMo:
                    _expendEnergyTypeQueue.Enqueue((expenseType, multiplier));
                    break;
                default:
                    ExpendEnergyImmediately(expenseType, multiplier);
                    break;
            }
            
        }

        public bool CanExpend(EnergyExpenseType expenseType, float multiplier = 1) => _energyData.Value >= MapToExpendValue(expenseType, multiplier);

        private void Awake()
        {
            if (_energySystemConfiguration == null)
            {
                PanicHelper.Panic(new Exception("Energy system configuration scriptable object missing from EnergySystemBehaviour"));
            }

            if (_energy == null)
            {
                PanicHelper.Panic(new Exception("Energy scriptable object missing from EnergySystemBehaviour"));
            }
        }

        private void Start()
        {
            EnergyData = _energy.MapToData();
            _energySystemConfigurationData = _energySystemConfiguration.MapToData();
        }

        private void Update()
        {
            if (_accruedEnergyTypeQueue.Count > 0 || _expendEnergyTypeQueue.Count > 0)
            {
                _elapsedTime -= Time.deltaTime;
            }
            
            if (_elapsedTime < 0)
            {
                _elapsedTime = EnergyData.RateOfTransaction;
                AccrueEnergyFromQueue();
                ExpendEnergyFromQueue();
            }

            RemoveExtraEnergyFromQueue();
        }
        
        private void AccrueEnergyFromQueue()
        {
            if (_accruedEnergyTypeQueue.TryPeek(out var result))
            {
                AccrueEnergyImmediately(result.Item1, result.Item2);
            }
        }
        
        private void ExpendEnergyFromQueue()
        {
            if (_expendEnergyTypeQueue.TryPeek(out var result))
            {
                ExpendEnergyImmediately(result.Item1, result.Item2);
            }
        }

        private void AccrueEnergyImmediately(EnergyAccruementType type, float multiplier)
        {
            var energy = Math.Min(_energyData.Value + MapToAccrueValue(type, multiplier), _energyData.MaxEnergyValue);
            EnergyData = _energyData.Apply(energy);
        }
        
        private void ExpendEnergyImmediately(EnergyExpenseType type, float multiplier)
        {
            var energy = Math.Max(_energyData.Value - MapToExpendValue(type, multiplier), 0);
            EnergyData = _energyData.Apply(energy);
        }

        private void RemoveExtraEnergyFromQueue()
        {
            if (_accruedEnergyTypeQueue.Count > 0)
            {
                _accruedEnergyTypeQueue.Dequeue();
            }
            
            if (_expendEnergyTypeQueue.Count > 0)
            {
                _expendEnergyTypeQueue.Dequeue();
            }
        }

        private float MapToAccrueValue(EnergyAccruementType type, float multiplier)
        {
            switch (type)
            {
                case EnergyAccruementType.WallRun:
                    return _energySystemConfigurationData.WallRunEnergy * multiplier;
                case EnergyAccruementType.Dash:
                    return _energySystemConfigurationData.DashEnergy * multiplier;
                case EnergyAccruementType.Slide:
                    return _energySystemConfigurationData.SlideEnergy * multiplier;
                default:
                    PanicHelper.Panic(new Exception("Missing Case for MapToAccrueValue"));
                    return 0;
            }
        }
        
        private float MapToExpendValue(EnergyExpenseType type, float multiplier)
        {
            switch (type)
            {
                case EnergyExpenseType.Shot:
                    return _energySystemConfigurationData.ShotEnergy * multiplier;
                case EnergyExpenseType.SlowMo:
                    return _energySystemConfigurationData.SlowMoEnergy * multiplier;
                default:
                    PanicHelper.Panic(new Exception("Missing Case for MapToExpendValue"));
                    return 0;
            }
        }
    }
}
