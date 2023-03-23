namespace BForBoss
{
    public readonly struct WallRunData
    {
        public readonly float SpeedMultiplier;
        public readonly float MaxWallRunAcceleration;
        public readonly float WallGravityDownForce;
        public readonly float GravityTimerDuration;
        public readonly float WallBounciness;
        public readonly float JumpHeightMultiplier;
        public readonly float JumpForwardVelocityMultiplier;

        public WallRunData(
            float speedMultiplier,
            float maxWallRunAcceleration,
            float wallGravityDownForce,
            float gravityTimerDuration,
            float wallBounciness,
            float jumpHeightMultiplier,
            float jumpForwardVelocityMultiplier
        )
        {
            SpeedMultiplier = speedMultiplier;
            MaxWallRunAcceleration = maxWallRunAcceleration;
            WallGravityDownForce = wallGravityDownForce;
            GravityTimerDuration = gravityTimerDuration;
            WallBounciness = wallBounciness;
            JumpHeightMultiplier = jumpHeightMultiplier;
            JumpForwardVelocityMultiplier = jumpForwardVelocityMultiplier;
        }
    }
}