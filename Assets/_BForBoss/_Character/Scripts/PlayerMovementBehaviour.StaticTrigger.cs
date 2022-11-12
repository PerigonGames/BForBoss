using UnityEngine;

namespace Perigon.Character
{
    public partial class PlayerMovementBehaviour : IStaticTriggerMode, IChargingSystemDatasource
    {
        public void StartStaticTriggerMode()
        {
            _poweredUpSpeedMultiplier = 2;
            Debug.Log("Max Speed: " + GetMaxSpeed());
        }

        public void StopStaticTriggerMode()
        {
            _poweredUpSpeedMultiplier = 1;
        }

        public bool IsCharging(float velocityThreshold)
        {
            Debug.Log("SpeedMagnitude: " + SpeedMagnitude);
            return SpeedMagnitude > velocityThreshold;
        }
    }
}
