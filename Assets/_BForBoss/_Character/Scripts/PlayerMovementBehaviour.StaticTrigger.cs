using UnityEngine;

namespace Perigon.Character
{
    public partial class PlayerMovementBehaviour : IStaticTriggerMode, IChargingSystemDatasource
    {
        public void StartStaticTriggerMode()
        {
            _poweredUpSpeedMultiplier = 2;
            _wallRunBehaviour.SetPoweredUpWallRunSpeedMultiplier(2f);
            _slideBehaviour.SetPowerUpSlideImpulseMultiplier(1.5f);
            Debug.Log("Max Speed: " + GetMaxSpeed());

        }

        public void StopStaticTriggerMode()
        {
            _poweredUpSpeedMultiplier = 1;
            _wallRunBehaviour.RevertPowerUpWallRunSpeedMultiplier();
            _slideBehaviour.RevertPowerUpSlideImpulseMultiplier();
        }

        public bool IsCharging(float velocityThreshold)
        {
            Debug.Log("SpeedMagnitude: " + SpeedMagnitude);
            return SpeedMagnitude > velocityThreshold;
        }
    }
}
