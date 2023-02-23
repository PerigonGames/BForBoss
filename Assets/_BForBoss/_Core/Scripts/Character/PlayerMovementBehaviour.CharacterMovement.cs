using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public interface ICharacterMovement
    {
        Transform RootPivot { get; }
        float CharacterMaxSpeed { get; }
        bool IsWallRunning { get; }
        float SpeedMagnitude { get; }
        bool IsWalking();
    }
    
    public partial class PlayerMovementBehaviour : ICharacterMovement
    {
        public Transform RootPivot => rootPivot;
        public float SpeedMagnitude => GetVelocity().magnitude;
        public float CharacterMaxSpeed => GetMaxSpeed();
        public bool IsWallRunning => _wallRunBehaviour.IsWallRunning;
    }
    
    public partial class PlayerMovementBehaviour : IWeaponBobIntensity
    {
        public float Value
        {
            get
            {
                var canBob = (_wallRunBehaviour.IsWallRunning || IsOnGround()) && 
                             !_dashBehaviour.IsDashing && 
                             !_slideBehaviour.IsSliding && 
                             SpeedMagnitude > 0;
                return SpeedMagnitude * (canBob ? 1 : 0);
            }
        }
    }
}
