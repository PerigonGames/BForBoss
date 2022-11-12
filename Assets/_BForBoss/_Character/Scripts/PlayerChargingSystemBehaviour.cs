using UnityEngine;

namespace Perigon.Character
{
    public interface IStaticTriggerMode
    {
        void StartStaticTriggerMode();
        void StopStaticTriggerMode();
    }

    public interface IChargingSystemDatasource
    {
        bool IsCharging(float velocityThreshold);
    }
    
    public class PlayerChargingSystemBehaviour : MonoBehaviour
    {
        [SerializeField] private float _alphaVelocityThreshold = 4f;
        [SerializeField] private float _decreaseRatePerTick = 0.5f;
        [SerializeField] private float _increaseAmountPerAction = 10f;

        public IStaticTriggerMode StaticTriggerModeDelegate { get; set; }
        public IChargingSystemDatasource ChargingSystemDatasource { get; set; } 
        
        private float _maxEnergyBank = 100;
        private float _currentEnergy;
        private bool _isModeActivated;

        public float Percentage => _currentEnergy / _maxEnergyBank;

        private void FixedUpdate()
        {
            if (_isModeActivated)
            {
                _currentEnergy -= _decreaseRatePerTick;
                if (_currentEnergy <= 0)
                {
                    _isModeActivated = false;
                    StaticTriggerModeDelegate.StopStaticTriggerMode();
                    Debug.Log("-----Mode Deactivated");
                }
                return;
            }
            
            if (ChargingSystemDatasource.IsCharging(_alphaVelocityThreshold))
            {
                _currentEnergy += _increaseAmountPerAction;
                Debug.Log("-----Increased Energy to :" + _currentEnergy);
                if (_currentEnergy >= _maxEnergyBank)
                {
                    StaticTriggerModeDelegate.StartStaticTriggerMode();
                    _isModeActivated = true;
                    Debug.Log("-----Mode Activated");
                }
            }
        }

    }
}
