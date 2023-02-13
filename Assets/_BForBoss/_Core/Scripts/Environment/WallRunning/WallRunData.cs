namespace BForBoss
{
    public struct WallRunData
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


/*
        private float _speedMultiplier = 1f;
        private float _maxWallRunAcceleration = 20f;
        private float _wallGravityDownForce = 0f;
        private float _minSpeed = 3f;
        private float _minHeight = 1f;
        private float _wallMaxDistance = 1f;
        private float _minJumpDuration = 0.3f;
        private float _obtuseWallAngle = 70f;
        private float _gravityTimerDuration = 1f;
        private float _wallResetTimer = 2f;
        private float _wallBounciness = 6f;
        private float _jumpHeightMultiplier = 1f;
        private float _jumpForwardVelocityMultiplier = .75f;
        private float _maxCameraAngleRoll = 30f;
        private float _cameraRotateDuration = 1f;
        private float _lookAlongWallRotationSpeed = 3f;
        private float _minLookAlongWallStabilizationAngle = 5f;
        private bool _shouldPrintDebugLogs = false;
        */