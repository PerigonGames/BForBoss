using ECM2.Characters;
using UnityEngine;

namespace ECM2.Examples.Animation.UnityCharacterAnimatorExample
{
    /// <summary>
    /// This example shows how to externally animate a ECM2 character based on its current movement mode and / or state.
    /// </summary>

    public sealed class UnityCharacterAnimator : MonoBehaviour
    {
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Turn = Animator.StringToHash("Turn");
        private static readonly int Ground = Animator.StringToHash("OnGround");
        private static readonly int Crouch = Animator.StringToHash("Crouch");
        private static readonly int Jump = Animator.StringToHash("Jump");

        [SerializeField]
        private Character _character;

        private bool _isCharacterNull;

        private void Update()
        {
            if (_isCharacterNull)
                return;

            // Get Character animator

            Animator animator = _character.GetAnimator();

            // Compute input move vector in local space

            Vector3 move = transform.InverseTransformDirection(_character.GetMovementDirection());

            // Update the animator parameters

            float forwardAmount = _character.useRootMotion
                ? move.z
                : Mathf.InverseLerp(0.0f, _character.GetMaxSpeed(), _character.GetSpeed());

            animator.SetFloat(Forward, forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat(Turn, Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);

            animator.SetBool(Crouch, _character.IsCrouching());
            animator.SetBool(Ground, _character.IsOnGround() || _character.WasOnGround());

            if (_character.IsFalling() && !_character.IsOnGround())
            {
                float verticalSpeed =
                    Vector3.Dot(_character.GetVelocity(), _character.GetUpVector());

                animator.SetFloat(Jump, verticalSpeed, 0.3333f, Time.deltaTime);
            }
        }

        private void Start()
        {
            _isCharacterNull = _character == null;
        }
    }
}
