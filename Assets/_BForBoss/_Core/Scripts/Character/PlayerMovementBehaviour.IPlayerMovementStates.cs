namespace BForBoss
{
    public interface IPlayerMovementStates
    {
        bool IsLowerThanSpeed(float speed);
        float GetNormalizedSpeed();
        bool IsWallRunning { get; }
        bool IsRunning { get; }
    }

    public partial class PlayerMovementBehaviour : IPlayerMovementStates
    {
        public bool IsLowerThanSpeed(float speed) => GetVelocity().magnitude < speed;

        public float GetNormalizedSpeed()
        {
            if (GetMaxSpeed() != 0)
            {
                return GetVelocity().magnitude / GetMaxSpeed();
            }

            return 0;
        }

        public bool IsWallRunning => _wallRunBehaviour.IsWallRunning;
        public bool IsRunning => IsWalking();
    }
}
