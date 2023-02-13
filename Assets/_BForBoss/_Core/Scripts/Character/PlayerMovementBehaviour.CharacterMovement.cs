using UnityEngine;

namespace BForBoss
{
    public interface ICharacterMovement
    {
        Transform RootPivot { get; }
        Vector3 CharacterVelocity { get; }
        float CharacterMaxSpeed { get; }
        bool IsGrounded { get; }
        bool IsDashing { get; }
        bool IsSliding { get; }
        bool IsWallRunning { get; }
        float SpeedMagnitude { get; }
        bool IsWalking();
    }
    
    public partial class PlayerMovementBehaviour : ICharacterMovement
    {
        public Transform RootPivot => rootPivot;
        public Vector3 CharacterVelocity => GetVelocity();
        public float SpeedMagnitude => GetVelocity().magnitude;
        public float CharacterMaxSpeed => GetMaxSpeed();
        public bool IsGrounded => IsOnGround();
        public bool IsDashing => _dashBehaviour?.IsDashing ?? false;
        public bool IsSliding => _slideBehaviour?.IsSliding ?? false;
        public bool IsWallRunning => _wallRunBehaviour?.IsWallRunning ?? false;
    }
}
