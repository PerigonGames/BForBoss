using ECM2.Characters;
using ECM2.Common;
using ECM2.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Examples.Gameplay.AdvancedDashExample
{
    /// <summary>
    /// This example shows how to extend a Character to add a Dashing state.
    /// The dashing ends on collision (eg: against a wall), if button is released or if Character's speed is less than maxWalkSpeed.
    /// </summary>

    public class MyCharacter : Character
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Dashing")]
        [Tooltip("The maximum ground speed while dashing.")]
        [SerializeField]
        private float _maxWalkSpeedDashing;

        [Tooltip("Deceleration when dashing and not applying acceleration.")]
        [SerializeField]
        private float _brakingDecelerationDashing;

        [Tooltip("Initial velocity when dashing.")]
        [SerializeField]
        private float _dashImpulse;

        #endregion

        #region FIELDS

        private bool _isDashing;

        private RotationMode _lastRotationMode;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The maximum ground speed while dashing.
        /// </summary>

        public float maxWalkSpeedDashing
        {
            get => _maxWalkSpeedDashing;
            set => _maxWalkSpeedDashing = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Deceleration when dashing and not applying acceleration.
        /// </summary>

        public float brakingDecelerationDashing
        {
            get => _brakingDecelerationDashing;
            set => _brakingDecelerationDashing = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Initial velocity when dashing.
        /// </summary>

        public float dashImpulse
        {
            get => _dashImpulse;
            set => _dashImpulse = Mathf.Max(0.0f, value);
        }

        #endregion

        #region INPUT ACTIONS

        /// <summary>
        /// Dash InputAction.
        /// </summary>

        protected InputAction dashInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        /// <summary>
        /// Dash input action handler.
        /// </summary>

        protected virtual void OnDash(InputAction.CallbackContext context)
        {
            if (context.started)
                Dash();
            else if (context.canceled)
                StopDashing();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// MovementHit event handler.
        /// Stops a dash if hit a non-walkable (eg: a wall).
        /// </summary>

        protected override void OnMovementHit(ref MovementHit movementHitResult)
        {
            // Call base method

            base.OnMovementHit(ref movementHitResult);

            // If dashing, stop dashing on non-walkable hit

            if (!IsDashing() || movementHitResult.isWalkable)
                return;

            StopDashing();

            SetVelocity(Vector3.zero);
        }

        /// <summary>
        /// Is the Character dashing ?
        /// Eg: Let the animator know the character is dashing.
        /// </summary>

        public bool IsDashing()
        {
            return _isDashing;
        }

        /// <summary>
        /// Is the Character able to perform a Dash ?
        /// </summary>

        private bool CanDash()
        {
            // Do not allow to dash if crouched

            if (IsCrouching())
                return false;

            // Only allow to dash if IsWalking or IsFalling (Eg: in air)

            return IsWalking() || IsFalling();
        }

        /// <summary>
        /// Start a dash.
        /// Call this from an input event (such as a button 'down' event).
        /// </summary>

        public void Dash()
        {
            // Is the character able to perform a dash ?

            if (!CanDash())
                return;

            // Perform dash

            _isDashing = true;

            // Use a separate braking friction (eg: bypass current ground friction while dashing)

            brakingFriction = 0.0f;
            useSeparateBrakingFriction = true;

            // Disable character's rotation to keep facing dash direction

            _lastRotationMode = GetRotationMode();
            SetRotationMode(RotationMode.None);

            // If no input, dash along character's forward

            Vector3 dashDirection = GetMovementDirection();
            if (dashDirection.isZero())
                dashDirection = GetForwardVector();

            LaunchCharacter(dashDirection.normalized * dashImpulse, true);
        }

        /// <summary>
        /// Stop the Character from dashing.
        /// Call this from an input event (such as a button 'up' event).
        /// </summary>

        public void StopDashing()
        {
            if (!IsDashing())
                return;

            _isDashing = false;

            useSeparateBrakingFriction = false;

            SetRotationMode(_lastRotationMode);
        }

        /// <summary>
        /// Handle dashing state.
        /// </summary>

        private void Dashing()
        {
            if (!IsDashing())
                return;

            // If falling during dashing, cancel gravity effects

            if (IsFalling())
            {
                Vector3 lateralVelocity = characterMovement.velocity.projectedOnPlane(GetUpVector());
                characterMovement.velocity = lateralVelocity;
            }
            
            // If our current velocity is lower than maxWalkSpeed, stop dash
            
            if (GetSpeed() < maxWalkSpeed)
                StopDashing();
        }

        /// <summary>
        /// Override GetBrakingDeceleration to add dashing support.
        /// </summary>

        public override float GetBrakingDeceleration()
        {
            return IsDashing() ? brakingDecelerationDashing : base.GetBrakingDeceleration();
        }

        /// <summary>
        /// Override GetMaxSpeed to add dashing support.
        /// </summary>

        public override float GetMaxSpeed()
        {
            return IsDashing() ? maxWalkSpeedDashing : base.GetMaxSpeed();
        }

        /// <summary>
        /// Overrides OnMove to add dashing custom movement mode to our base movements modes.
        /// </summary>

        protected override void OnMove()
        {
            // Handle base movement modes (eg: walking, falling, etc)

            base.OnMove();

            // Handle dashing state

            Dashing();
        }

        /// <summary>
        /// Extends HandleInput method to disable input while Character is dashing to preserve direction.
        /// </summary>

        protected override void HandleInput()
        {
            // If dashing, disable player input

            if (IsDashing())
                return;

            // Default input

            base.HandleInput();
        }

        /// <summary>
        /// Overrides SetupPlayerInput to add dash input action.
        /// </summary>

        protected override void SetupPlayerInput()
        {
            // Setup default input actions

            base.SetupPlayerInput();

            // Setup Dash input action handlers
            
            dashInputAction = actions.FindAction("Dash");
            if (dashInputAction != null)
            {
                dashInputAction.started += OnDash;
                dashInputAction.performed += OnDash;
                dashInputAction.canceled += OnDash;
            }
        }

        /// <summary>
        /// Override OnOnEnable.
        /// </summary>

        protected override void OnOnEnable()
        {
            // Init base class

            base.OnOnEnable();

            // Enable this dash input action

            dashInputAction?.Enable();
        }

        /// <summary>
        /// Override OnOnDisable.
        /// </summary>

        protected override void OnOnDisable()
        {
            // De-init base class

            base.OnOnDisable();

            // Disable this dash input action

            dashInputAction?.Disable();
        }

        /// <summary>
        /// Override OnReset.
        /// Set this default values.
        /// </summary>

        protected override void OnReset()
        {
            // Base defaults

            base.OnReset();

            // This default values

            maxWalkSpeedDashing = 10.0f;
            brakingDecelerationDashing = 20.0f;
            dashImpulse = 10.0f;
        }

        /// <summary>
        /// Override OnOnValidate.
        /// Validate this editor exposed fields.
        /// </summary>

        protected override void OnOnValidate()
        {
            // Validate base editor exposed fields

            base.OnOnValidate();

            // Validate this editor exposed fields

            maxWalkSpeedDashing = _maxWalkSpeedDashing;
            brakingDecelerationDashing = _brakingDecelerationDashing;
            dashImpulse = _dashImpulse;
        }

        #endregion
    }
}
