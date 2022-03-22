using UnityEngine;

namespace Perigon.Character
{
    public interface ICharacterMovement
    {
        Vector3 CharacterVelocity { get; }
        float CharacterMaxSpeed { get; }
        bool IsGrounded { get; }
        bool IsDashing { get; }
        bool IsSliding { get; }
        bool IsWallRunning { get; }
        float SpeedMagnitude { get; }
        bool IsWalking();
    }
    
    public partial class FirstPersonPlayer : ICharacterMovement
    {
        public Vector3 CharacterVelocity => GetVelocity();
        public float SpeedMagnitude => GetVelocity().magnitude;
        public float CharacterMaxSpeed => GetMaxSpeed();
        public bool IsGrounded => IsOnGround();
        public bool IsDashing => _dashBehaviour?.IsDashing ?? false;
        public bool IsSliding => _slideBehaviour?.IsSliding ?? false;
        public bool IsWallRunning => _wallRunBehaviour?.IsWallRunning ?? false;
    }
}
