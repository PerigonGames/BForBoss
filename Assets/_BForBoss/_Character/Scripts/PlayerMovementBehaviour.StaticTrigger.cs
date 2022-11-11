namespace Perigon.Character
{
    public partial class PlayerMovementBehaviour : IStaticTriggerMode, IChargingSystemDatasource
    {
        public void StartStaticTriggerMode()
        {
        }

        public void StopStaticTriggerMode()
        {
        }

        public bool IsCharging(float velocityThreshold)
        {
            return true;
        }
    }
}
