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

        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField, Tooltip("Stop a wall run if speed dips below this")]
        private float _minSpeed = 3f;
        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField, Tooltip("Don't allow a wall run if the player is too close to the ground")] 
        private float _minHeight = 1f;
        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField]
        private float _wallMaxDistance = 1f;
        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField, Range(0f, 3f), Tooltip("Only allow for a wall run if jump is longer than this")]
        private float _minJumpDuration = 0.3f;
        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField, Tooltip("Stop wall running next angled wall is obtuse to this angle")]
        private float _obtuseWallAngle = 70f;

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
        private float _lookAlongWallRotationSpeed = 3f;
        [FoldoutGroup("Camera Settings")]
        [SerializeField] 
        private float _minLookAlongWallStabilizationAngle = 5f;

        public WallRunData MapToData()
        {
            return new WallRunData(
                speedMultiplier: _speedMultiplier,
                maxWallRunAcceleration: _maxWallRunAcceleration,
                wallGravityDownForce: _wallGravityDownForce,
                minSpeed: _minSpeed,
                minHeight: _minHeight,
                wallMaxDistance: _wallMaxDistance,
                minJumpDuration: _minJumpDuration,
                obtuseWallAngle: _obtuseWallAngle,
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
