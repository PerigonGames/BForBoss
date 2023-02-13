using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [CreateAssetMenu(fileName = "Wall Run Data", menuName = "PerigonGames/WallRunData", order = 2)]
    public class WallRunSO : ScriptableObject
    {
        [FoldoutGroup("Wall Run Movement properties")]
        [SerializeField] 
        private float _speedMultiplier = 1f;
        [FoldoutGroup("Wall Run Movement properties")]
        [SerializeField]
        private float _maxWallRunAcceleration = 20f;
        [FoldoutGroup("Wall Run Movement properties")]
        [SerializeField]
        private float _wallGravityDownForce;

        [FoldoutGroup("Timers")]
        [SerializeField, Tooltip("Gravity won't apply until after this many seconds")]
        private float _gravityTimerDuration = 1f;
        [FoldoutGroup("Timers")]
        [SerializeField, Tooltip("Wall runs on the same wall are only allowed after this long")]
        private float _wallResetTimer = 2f;

        [FoldoutGroup("Wall Run jump properties")]
        [SerializeField]
        private float _wallBounciness = 6f;
        [FoldoutGroup("Wall Run jump properties")]
        [SerializeField]
        private float _jumpHeightMultiplier = 1f;
        [FoldoutGroup("Wall Run jump properties")]
        [SerializeField, Range(0f, 2f)]
        private float _jumpForwardVelocityMultiplier = .75f;

        [FoldoutGroup("Camera Settings")]
        [SerializeField]
        private float _maxCameraAngleRoll = 30f;
        [FoldoutGroup("Camera Settings")]
        [SerializeField]
        private float _cameraRotateDuration = 1f;
        [FoldoutGroup("Camera Settings")]
        [SerializeField] 
        [InfoBox("How fast the camera rotates towards where the player is wall running along")]
        private float _lookAlongWallRotationSpeed = 3f;
        [FoldoutGroup("Camera Settings")]
        [SerializeField]
        [InfoBox("The angle between where you're looking at and the direction where you're wall running towards")]
        private float _minLookAlongWallStabilizationAngle = 5f;

        public WallRunData MapToData()
        {
            return new WallRunData(
                speedMultiplier: _speedMultiplier,
                maxWallRunAcceleration: _maxWallRunAcceleration,
                wallGravityDownForce: _wallGravityDownForce,
                gravityTimerDuration: _gravityTimerDuration,
                wallResetTimer: _wallResetTimer,
                wallBounciness: _wallBounciness,
                jumpHeightMultiplier: _jumpHeightMultiplier,
                jumpForwardVelocityMultiplier: _jumpForwardVelocityMultiplier,
                maxCameraAngleRoll: _maxCameraAngleRoll,
                cameraRotateDuration: _cameraRotateDuration,
                lookAlongWallRotationSpeed: _lookAlongWallRotationSpeed,
                minLookAlongWallStabilizationAngle: _minLookAlongWallStabilizationAngle);
        }
    }
}
