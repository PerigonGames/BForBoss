using ECM2.Characters;
using ECM2.Common;
using ECM2.Components;
using UnityEngine;

namespace ECM2.Examples.Gameplay.SlideExample
{
    /// <summary>
    /// This example shows how to extend the FirstPersonCharacter to add a sliding state.
    /// 
    /// To perform a slide the character must be sprinting (on ground) and crouch.
    /// We use the Crouched / Uncrouched events to start and stop a slide.
    /// </summary>

    public class SlidingCharacter : FirstPersonCharacter
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Sliding")]
        [Tooltip("The maximum ground speed while sliding.")]
        [SerializeField]
        private float _maxWalkSpeedSliding;

        [Tooltip("The ground friction while sliding.")]
        [SerializeField]
        private float _groundFrictionSliding;

        [Tooltip("Deceleration while sliding.")]
        [SerializeField]
        private float _brakingDecelerationSliding;

        [Tooltip("The sliding impulse.")]
        [SerializeField]
        private float _slideImpulse;

        #endregion

        #region FIELDS
        
        private bool _isSliding;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The maximum ground speed while sliding.
        /// </summary>

        public float maxWalkSpeedSliding
        {
            get => _maxWalkSpeedSliding;
            set => _maxWalkSpeedSliding = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The ground friction while sliding.
        /// </summary>

        public float groundFrictionSliding
        {
            get => _groundFrictionSliding;
            set => _groundFrictionSliding = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Deceleration while sliding.
        /// </summary>

        public float brakingDecelerationSliding
        {
            get => _brakingDecelerationSliding;
            set => _brakingDecelerationSliding = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The sliding initial impulse.
        /// </summary>

        public float slideImpulse
        {
            get => _slideImpulse;
            set => _slideImpulse = Mathf.Max(0.0f, value);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// MovementHit event handler.
        /// Use this to stop a slide when character hits a non-walkable (eg: a wall).
        /// </summary>
        
        protected override void OnMovementHit(ref MovementHit movementHitResult)
        {
            // Call base method

            base.OnMovementHit(ref movementHitResult);

            // If sliding, stop slide on non-walkable hit

            if (IsSliding() && !movementHitResult.isWalkable)
                StopSliding();
        }

        /// <summary>
        /// Crouch event handler.
        /// Used to start a slide (if possible) when character's crouch.
        /// </summary>
        
        protected override void OnCrouched()
        {
            // Call base method

            base.OnCrouched();

            Slide();
        }

        /// <summary>
        /// Uncrouch event handler.
        /// Used to stop a slide when character's uncrouch.
        /// </summary>
        
        protected override void OnUncrouched()
        {
            // Call base method

            base.OnUncrouched();

            StopSliding();
        }

        /// <summary>
        /// Is the Character sliding ?
        /// </summary>
        
        public bool IsSliding()
        {
            return _isSliding;
        }

        /// <summary>
        /// Is the Character able to perform the requested slide ?
        /// </summary>

        private bool CanSlide()
        {
            // If already sliding, return

            if (IsSliding())
                return false;

            // If not sprinting, return

            if (!IsSprinting())
                return false;

            // If no player input, return

            Vector3 movementDirection = GetMovementDirection();
            if (movementDirection.isZero())
                return false;

            // Allow slide

            return true;
        }

        /// <summary>
        /// Start a slide.
        /// </summary>

        private void Slide()
        {
            // Is the character able to perform the requested slide ?

            if (!CanSlide())
                return;

            // Start a slide
            
            _isSliding = true;
            
            // Bypass ground friction (use separate friction)

            brakingFriction = groundFrictionSliding;
            useSeparateBrakingFriction = true;

            // Add slide impulse to character's current velocity

            Vector3 slideDirection = GetVelocity().normalized;
            LaunchCharacter(slideDirection * slideImpulse);
        }

        /// <summary>
        /// Stop the Character from sliding.
        /// </summary>

        private void StopSliding()
        {
            // If sliding, stop the sliding and restore ground friction

            if (!IsSliding())
                return;
            
            _isSliding = false;

            useSeparateBrakingFriction = false;
        }

        /// <summary>
        /// Update the sliding state.
        /// </summary>

        private void Sliding()
        {
            // Is Character sliding ?

            if (!IsSliding())
                return;

            // Are we on ground ?

            if (!IsOnGround())
            {
                // If not on ground, stop sliding

                StopSliding();
            }
            else
            {
                // While on ground, add gravity to accelerate down slope and slowdown up slope

                Vector3 velocity = GetVelocity();
                Vector3 gravityVector = GetGravityVector();

                velocity += gravityVector.projectedOnPlane(characterMovement.groundHit.normal) * Time.deltaTime;

                SetVelocity(velocity);

                // If our current velocity is lower than maxWalkSpeedCrouched, stop sliding

                if (velocity.sqrMagnitude < MathLib.Square(maxWalkSpeedCrouched))
                    StopSliding();
            }
        }

        /// <summary>
        /// Override GetBrakingDeceleration to add sliding support.
        /// </summary>

        public override float GetBrakingDeceleration()
        {
            return IsSliding() ? brakingDecelerationSliding : base.GetBrakingDeceleration();
        }

        /// <summary>
        /// Override GetMaxSpeed to add sliding support.
        /// </summary>

        public override float GetMaxSpeed()
        {
            // Return max speed for current movement mode (factoring sliding)

            return IsSliding() ? maxWalkSpeedSliding : base.GetMaxSpeed();
        }

        /// <summary>
        /// Override CalcDesiredVelocity.
        /// Here we disable input (zero desired velocity) while sliding to maintain our slide direction AND cause deceleration because of no input,
        /// otherwise return desired velocity.
        /// </summary>

        protected override Vector3 CalcDesiredVelocity()
        {
            return IsSliding() ? Vector3.zero : base.CalcDesiredVelocity();
        }

        /// <summary>
        /// Extends OnMove to handle sliding state.
        /// </summary>

        protected override void OnMove()
        {
            // Handle base movement modes (eg: walking, falling, etc)

            base.OnMove();

            // Handle sliding state

            Sliding();
        }

        /// <summary>
        /// Override OnReset.
        /// Set this default values.
        /// </summary>

        protected override void OnReset()
        {
            base.OnReset();

            maxWalkSpeedSliding = 12.0f;
            groundFrictionSliding = 0.5f;
            brakingDecelerationSliding = 10.0f;
            slideImpulse = 10.0f;
        }

        /// <summary>
        /// Override OnOnValidate.
        /// Validate this editor exposed fields.
        /// </summary>

        protected override void OnOnValidate()
        {
            base.OnOnValidate();

            maxWalkSpeedSliding = _maxWalkSpeedSliding;
            groundFrictionSliding = _groundFrictionSliding;
            brakingDecelerationSliding = _brakingDecelerationSliding;
            slideImpulse = _slideImpulse;
        }

        #endregion
    }
}
