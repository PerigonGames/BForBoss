using UnityEngine;

namespace BForBoss
{
    public partial class FirstPersonPlayer
    {
        #region ANIMATOR_FIELDS
        private static readonly int ForwardParamId = Animator.StringToHash("Forward");
        private static readonly int TurnParamId = Animator.StringToHash("Turn");
        private static readonly int GroundParamId = Animator.StringToHash("OnGround");
        private static readonly int CrouchParamId = Animator.StringToHash("Crouch");
        private static readonly int JumpParamId = Animator.StringToHash("Jump");
        private static readonly int SlideParamId = Animator.StringToHash("Sliding");
        #endregion

        protected override void Animate()
        {
            if (!IsThirdPerson)
            {
                TurnOffAnimator();
                return;
            }

            if (IsWallRunning())
                SetWallRunSpeed();
            else
                SetRunSpeed();


            animator.SetBool(GroundParamId, IsOnGround() || IsWallRunning());
            animator.SetBool(SlideParamId, IsSliding());
            animator.SetBool(CrouchParamId, IsCrouching());

            SetAnimatorJump();
        }

        private void TurnOffAnimator()
        {
            animator.SetFloat(JumpParamId, 0f);
            animator.SetFloat(ForwardParamId, 0f);
            animator.SetFloat(TurnParamId, 0f);
            animator.SetBool(GroundParamId, true);
        }

        private void SetAnimatorJump()
        {
            if (IsFalling() && !IsWallRunning())
            {
                float verticalSpeed = Vector3.Dot(GetVelocity(), GetUpVector());
                animator.SetFloat(JumpParamId, verticalSpeed, 0.1f, Time.deltaTime);
            }
        }

        private void SetAnimatorSpeed(float forwardAmount, float damping = 0.1f)
        {
            animator.SetFloat(ForwardParamId, forwardAmount, damping, Time.deltaTime);
        }

        private void SetAnimatorTurn(float turnVal, float damping = 0.1f)
        {
            animator.SetFloat(TurnParamId, turnVal, damping, Time.deltaTime);
        }

        private void SetRunSpeed()
        {
            // Compute input move direction vector in local space
            Vector3 move = rootPivot.InverseTransformDirection(GetMovementDirection());
            float forwardAmount = useRootMotion ? move.z : Mathf.InverseLerp(0.0f, GetMaxSpeed(), GetSpeed());
            
            SetAnimatorSpeed(forwardAmount);
            SetAnimatorTurn(Mathf.Atan2(move.x, move.z));
        }

        private void SetWallRunSpeed()
        {
            SetAnimatorSpeed(1f);
            SetAnimatorTurn(-_wallRunBehaviour.CalculateWallSideRelativeToPlayer());
        }
    }
}
