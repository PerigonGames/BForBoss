using ECM2.Characters;
using ECM2.Common;
using UnityEngine;

namespace ECM2.Examples.Gameplay.SlopeSpeedModifierExample
{
    /// <summary>
    /// This example shows how to extend a Character to modify its max walk speed based on its current slope angle.
    /// </summary>

    public class MyCharacter : Character
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Slope Speed Modifier")]
        public AnimationCurve slopeSpeedModifier = new AnimationCurve(
            new Keyframe(-50.0f, 2.0f), 
            new Keyframe(-10.0f, 1.0f), 
            new Keyframe(10.0f, 1.0f), 
            new Keyframe(50.0f, 0.0f)
        );

        #endregion

        #region FIELDS

        private float _maxWalkSpeedOnSlope;

        #endregion

        #region METHODS

        /// <summary>
        /// Use the current slope angle to get the corresponding speed modifier from our slopeSpeedModifier curve.
        /// </summary>

        public float GetSlopeSpeedModifier()
        {
            Vector3 velocity = characterMovement.velocity.normalized;

            float signedSlopeAngle = Mathf.Asin(velocity.y) * Mathf.Rad2Deg;

            float speedModifier = slopeSpeedModifier.Evaluate(signedSlopeAngle);

            return speedModifier;
        }

        /// <summary>
        /// Update our max walk speed while character is moving on ground.
        /// Basically interpolate between maxWalkSpeed and maxWalkSpeed multiplied by slopeSpeedModifier.
        /// </summary>

        private void UpdateMaxWalkVelocityOnSlope()
        {
            if (!IsWalking() || characterMovement.velocity.isZero())
                return;

            float speedModifier = GetSlopeSpeedModifier();

            float actualMaxWalkSpeed = base.GetMaxSpeed();
            float desiredMaxWalkSpeed = actualMaxWalkSpeed * speedModifier;

            _maxWalkSpeedOnSlope = Mathf.MoveTowards(_maxWalkSpeedOnSlope, desiredMaxWalkSpeed, 4.0f * Time.deltaTime);
        }

        /// <summary>
        /// Extends GetMaxSpeed method to use our modified maxWalkSpeed.
        /// </summary>

        public override float GetMaxSpeed()
        {
            return IsWalking() ? _maxWalkSpeedOnSlope : base.GetMaxSpeed();
        }


        /// <summary>
        /// Determines the Character's movement for its current movement mode.
        /// Called during character's movement, CharacterMovement Move method (delegate).
        /// Extended to modify speed based on slope angle.
        /// </summary>
        
        protected override void OnMove()
        {
            // Call base method implementation

            base.OnMove();

            // Update our max walk speed while character is moving on ground.

            UpdateMaxWalkVelocityOnSlope();
        }

        /// <summary>
        /// Initialize this.
        /// </summary>
        
        protected override void OnAwake()
        {
            base.OnAwake();

            _maxWalkSpeedOnSlope = maxWalkSpeed;
        }

        #endregion
    }
}
