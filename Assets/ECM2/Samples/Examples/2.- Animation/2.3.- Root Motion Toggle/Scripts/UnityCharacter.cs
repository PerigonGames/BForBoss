using UnityEngine;
using ECM2.Characters;

namespace ECM2.Examples.Animation.CharacterRootMotionToggleExample
{
    /// <summary>
    /// This example shows how to extend our Character to toggle root motion on/off based on its current movement mode (or a state).
    /// 
    /// In this example, we use the OnMovementModeChanged to enable / disable root motion on movement mode change.
    /// In this case, enable root motion when walking, otherwise disable it.
    /// 
    /// </summary>

    public class UnityCharacter : Character
    {
        private static readonly int ForwardParamId = Animator.StringToHash("Forward");
        private static readonly int TurnParamId = Animator.StringToHash("Turn");
        private static readonly int GroundParamId = Animator.StringToHash("OnGround");
        private static readonly int CrouchParamId = Animator.StringToHash("Crouch");
        private static readonly int JumpParamId = Animator.StringToHash("Jump");

        protected override void Animate()
        {
            // Set Ethan Animator parameters

            // Compute input move direction vector in local space

            Vector3 move = transform.InverseTransformDirection(GetMovementDirection());

            // Update the animator parameters

            float forwardAmount = useRootMotion ? move.z : Mathf.InverseLerp(0.0f, GetMaxSpeed(), GetSpeed());

            animator.SetFloat(ForwardParamId, forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat(TurnParamId, Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);

            animator.SetBool(GroundParamId, IsOnGround());
            animator.SetBool(CrouchParamId, IsCrouching());

            if (IsFalling())
            {
                float verticalSpeed = Vector3.Dot(GetVelocity(), GetUpVector());
                animator.SetFloat(JumpParamId, verticalSpeed, 0.1f, Time.deltaTime);
            }
        }

        protected override void OnMovementModeChanged(MovementMode prevMovementMode, int prevCustomMode)
        {
            // Call base method implementation

            base.OnMovementModeChanged(prevMovementMode, prevCustomMode);

            // Toggle root motion, allow when walking, otherwise, disable it

            useRootMotion = IsWalking();
        }
    }
}
