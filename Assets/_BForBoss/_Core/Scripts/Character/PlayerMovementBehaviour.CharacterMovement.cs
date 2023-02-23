using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public interface IPlayerMovementStates
    {
        bool IsLowerThanSpeed(float speed);
        float GetNormalizedSpeed();
        bool IsWallRunning { get; }
        bool IsRunning { get; }
    }

    public partial class PlayerMovementBehaviour : IPlayerMovementStates
    {
        public bool IsLowerThanSpeed(float speed) => GetVelocity().magnitude < speed;

        public float GetNormalizedSpeed()
        {
            if (GetMaxSpeed() != 0)
            {
                return GetVelocity().magnitude / GetMaxSpeed();
            }

            return 0;
        }

        public bool IsWallRunning => _wallRunBehaviour.IsWallRunning;
        public bool IsRunning => IsWalking();
    }
    
    public partial class PlayerMovementBehaviour : IGetPlayerTransform
    {
        Transform IGetPlayerTransform.Value => rootPivot;
    }

    public partial class PlayerMovementBehaviour : IWeaponBobIntensity
    {
        float IWeaponBobIntensity.Value
        {
            get
            {
                var canBob = (_wallRunBehaviour.IsWallRunning || IsOnGround()) && 
                             !_dashBehaviour.IsDashing && 
                             !_slideBehaviour.IsSliding && 
                             GetVelocity().magnitude > 0;
                return GetVelocity().magnitude * (canBob ? 1 : 0);
            }
        }
    }
}
