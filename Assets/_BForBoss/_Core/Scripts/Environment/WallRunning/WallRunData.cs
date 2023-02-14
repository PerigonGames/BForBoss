namespace BForBoss
{
    public readonly struct WallRunData
    {
        public readonly float SpeedMultiplier;
        public readonly float MaxWallRunAcceleration;
        public readonly float WallGravityDownForce;
        public readonly float GravityTimerDuration;
        public readonly float WallResetTimer;
        public readonly float WallBounciness;
        public readonly float JumpHeightMultiplier;
        public readonly float JumpForwardVelocityMultiplier;
        public readonly float MaxCameraAngleRoll;
        public readonly float CameraRotateDuration;
        public readonly float LookAlongWallRotationSpeed;
        public readonly float MinLookAlongWallStabilizationAngle;

        public WallRunData(
            float speedMultiplier,
            float maxWallRunAcceleration,
            float wallGravityDownForce,
            float gravityTimerDuration,
            float wallResetTimer,
            float wallBounciness,
            float jumpHeightMultiplier,
            float jumpForwardVelocityMultiplier,
            float maxCameraAngleRoll,
            float cameraRotateDuration,
            float lookAlongWallRotationSpeed,
            float minLookAlongWallStabilizationAngle
            )
        {
            SpeedMultiplier = speedMultiplier;
            MaxWallRunAcceleration = maxWallRunAcceleration;
            WallGravityDownForce = wallGravityDownForce;
            GravityTimerDuration = gravityTimerDuration;
            WallResetTimer = wallResetTimer;
            WallBounciness = wallBounciness;
            JumpHeightMultiplier = jumpHeightMultiplier;
            JumpForwardVelocityMultiplier = jumpForwardVelocityMultiplier;
            MaxCameraAngleRoll = maxCameraAngleRoll;
            CameraRotateDuration = cameraRotateDuration;
            LookAlongWallRotationSpeed = lookAlongWallRotationSpeed;
            MinLookAlongWallStabilizationAngle = minLookAlongWallStabilizationAngle;
        }
    }
}