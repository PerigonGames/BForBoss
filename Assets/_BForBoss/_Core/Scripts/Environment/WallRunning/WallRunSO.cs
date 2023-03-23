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

        [FoldoutGroup("Wall Run jump properties")]
        [SerializeField]
        private float _wallBounciness = 6f;
        [FoldoutGroup("Wall Run jump properties")]
        [SerializeField]
        private float _jumpHeightMultiplier = 1f;
        [FoldoutGroup("Wall Run jump properties")]
        [SerializeField, Range(0f, 2f)]
        private float _jumpForwardVelocityMultiplier = .75f;

        public WallRunData MapToData()
        {
            return new WallRunData(
                speedMultiplier: _speedMultiplier,
                maxWallRunAcceleration: _maxWallRunAcceleration,
                wallGravityDownForce: _wallGravityDownForce,
                gravityTimerDuration: _gravityTimerDuration,
                wallBounciness: _wallBounciness,
                jumpHeightMultiplier: _jumpHeightMultiplier,
                jumpForwardVelocityMultiplier: _jumpForwardVelocityMultiplier);
        }
    }
}
