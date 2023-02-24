using Perigon.Weapons;

namespace BForBoss
{
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
