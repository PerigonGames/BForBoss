using BForBoss;

namespace Tests
{
    public class MockCharacterMovement : IPlayerMovementStates
    {
        public bool _isLowerThanSpeed = false;
        public bool IsLowerThanSpeed(float speed) => _isLowerThanSpeed;

        public float GetNormalizedSpeed() => 0;

        public bool IsWallRunning { get; set; }
        public bool IsRunning { get; set; }
    }
}
