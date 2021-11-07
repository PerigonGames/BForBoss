using ECM2.Characters;
using UnityEngine;

namespace BForBoss
{
    public partial class FirstPersonPlayer : FirstPersonCharacter
    {
        #region ANIMATOR_FIELDS
        private static readonly int ForwardParamId = Animator.StringToHash("Forward");
        private static readonly int TurnParamId = Animator.StringToHash("Turn");
        private static readonly int GroundParamId = Animator.StringToHash("OnGround");
        private static readonly int CrouchParamId = Animator.StringToHash("Crouch");
        private static readonly int JumpParamId = Animator.StringToHash("Jump");
        #endregion

        protected override void Animate()
        {
            if (!IsThirdPerson) return;

            // Compute input move direction vector in local space
            Vector3 move = rootPivot.InverseTransformDirection(GetMovementDirection());

            float forwardAmount = useRootMotion ? move.z : Mathf.InverseLerp(0.0f, GetMaxSpeed(), GetSpeed());

            animator.SetFloat(ForwardParamId, forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat(TurnParamId, Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);

            animator.SetBool(GroundParamId, IsOnGround() || IsWallRunning());
            animator.SetBool(CrouchParamId, IsCrouching());

            if (IsFalling() && !IsWallRunning())
            {
                float verticalSpeed = Vector3.Dot(GetVelocity(), GetUpVector());
                animator.SetFloat(JumpParamId, verticalSpeed, 0.1f, Time.deltaTime);
            }
        }
    }
}
