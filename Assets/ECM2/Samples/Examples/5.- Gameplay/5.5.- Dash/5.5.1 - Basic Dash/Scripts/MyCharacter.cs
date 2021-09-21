using ECM2.Characters;
using ECM2.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Examples.Gameplay.BasicDashExample
{
    /// <summary>
    /// This example shows how to extend a Character to add a Dashing state.
    /// </summary>

    public class MyCharacter : Character
    {
        #region EDITOR EXPOSED FIELDS

        public float dashDuration = 0.1f;
        public float dashImpulse = 20.0f;

        #endregion

        #region FIELDS

        private bool _isDashing;
        private float _dashTime;
        private Vector3 _dashingDirection;

        #endregion

        #region INPUT ACTIONS

        private InputAction dashInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        private void OnDash(InputAction.CallbackContext context)
        {
            if (context.started)
                Dash();
            else if (context.canceled)
                StopDashing();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Extends OnMovementHit method to stop dash on collision (eg: against a wall).
        /// </summary>

        protected override void OnMovementHit(ref MovementHit movementHitResult)
        {
            base.OnMovementHit(ref movementHitResult);

            if (IsDashing() && !movementHitResult.isWalkable)
            {
                StopDashing();

                characterMovement.velocity = IsOnWalkableGround() ? Vector3.zero : Vector3.ProjectOnPlane(characterMovement.velocity, GetUpVector());
            }
        }


        /// <summary>
        /// Determines whether the character is dashing.
        /// </summary>
        
        public bool IsDashing()
        {
            return _isDashing;
        }

        /// <summary>
        /// Start a dash.
        /// Call this from an input event (such as a button 'down' event).
        /// </summary>

        public void Dash()
        {
            _isDashing = true;

            brakingFriction = 0.0f;
            useSeparateBrakingFriction = true;

            Vector2 movementInput = GetMovementInput();

            _dashingDirection = movementInput.sqrMagnitude > 0 ? GetMovementDirection() : GetForwardVector();
            LaunchCharacter(_dashingDirection * dashImpulse, true, true);
        }

        /// <summary>
        /// Stop the Character from dashing.
        /// Call this from an input event (such as a button 'up' event).
        /// </summary>

        public void StopDashing()
        {
            _dashTime = 0.0f;
            _isDashing = false;

            useSeparateBrakingFriction = false;
        }

        /// <summary>
        /// Handle Dashing state.
        /// </summary>

        private void Dashing()
        {
            if (!IsDashing())
                return;

            if (IsFalling())
                characterMovement.velocity = Vector3.ProjectOnPlane(characterMovement.velocity, GetUpVector());

            _dashTime += Time.deltaTime;
            if (_dashTime > dashDuration)
                StopDashing();
        }

        /// <summary>
        /// Extends OnMove method to handle dashing state.
        /// </summary>

        protected override void OnMove()
        {
            base.OnMove();

            Dashing();
        }

        /// <summary>
        /// Extends HandleInput method, to support dashing state.
        /// </summary>

        protected override void HandleInput()
        {
            base.HandleInput();

            // If Dashing keep the character looking towards dashing direction

            if (IsDashing())
                SetMovementDirection(_dashingDirection);
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
                dashInputAction.canceled += OnDash;
            }
        }

        protected override void OnOnEnable()
        {
            // Init base class

            base.OnOnEnable();

            // Enable this dash input action

            dashInputAction?.Enable();
        }

        protected override void OnOnDisable()
        {
            // De-init base class

            base.OnOnDisable();

            // Disable this dash input action

            dashInputAction?.Disable();
        }

        #endregion
    }
}
