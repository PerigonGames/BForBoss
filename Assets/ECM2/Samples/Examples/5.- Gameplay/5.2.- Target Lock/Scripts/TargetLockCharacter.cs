using ECM2.Characters;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Examples.Gameplay.TargetLockExample
{
    /// <summary>
    /// This example shows how to extend a Character to perform a target locking mechanic.
    /// </summary>

    public class TargetLockCharacter : Character
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Target")]
        public Transform targetTransform;

        #endregion

        #region FIELDS

        private bool _lockButtonPressed;

        #endregion

        #region INPUT ACTIONS

        /// <summary>
        /// Target lock Input action.
        /// </summary>

        private InputAction targetLockInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        /// <summary>
        /// Target lock input action handler.
        /// </summary>

        private void OnTargetLock(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                LockTarget();
            else if (context.canceled)
                StopLockingTarget();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Start a target lock.
        /// Call this from an input event (such as a button 'down' event).
        /// </summary>

        public void LockTarget()
        {
            _lockButtonPressed = true;
        }

        /// <summary>
        /// Stops target locking.
        /// Call this from an input event (such as a button 'up' event).
        /// </summary>

        public void StopLockingTarget()
        {
            _lockButtonPressed = false;
        }

        /// <summary>
        /// Is the Character looking at its target ?
        /// </summary>

        public bool IsLockingTarget()
        {
            return targetTransform != null && _lockButtonPressed;
        }

        /// <summary>
        /// Extends Handle Input method to handle target locking state.
        /// </summary>

        protected override void HandleInput()
        {
            if (!IsLockingTarget())
            {
                // If not locking, perform regular movement

                base.HandleInput();
            }
            else
            {
                // Add movement relative to us

                Vector2 movementInput = GetMovementInput();

                Vector3 movementDirection = Vector3.zero;

                movementDirection += GetRightVector() * movementInput.x;
                movementDirection += GetForwardVector() * movementInput.y;

                SetMovementDirection(movementDirection);
            }
        }

        /// <summary>
        /// Extends UpdateRotation method to handle target locking state.
        /// </summary>

        protected override void UpdateRotation()
        {
            // If not locking target use default rotation

            if (!IsLockingTarget())
                base.UpdateRotation();
            else
            {
                // Is Character is disabled, return

                if (IsDisabled())
                    return;

                // Should update Character's rotation ?

                RotationMode rotationMode = GetRotationMode();

                if (rotationMode == RotationMode.None)
                    return;

                // Look at target

                Vector3 toTarget = targetTransform.position - GetPosition();

                RotateTowards(toTarget);
            }
        }

        /// <summary>
        /// Extends SetupPlayerInput to config targetLock InputAction.
        /// </summary>

        protected override void SetupPlayerInput()
        {
            base.SetupPlayerInput();

            if (actions == null)
                return;

            targetLockInputAction = actions.FindAction("Lock Target");
            if (targetLockInputAction != null)
            {
                targetLockInputAction.started += OnTargetLock;
                targetLockInputAction.performed += OnTargetLock;
                targetLockInputAction.canceled += OnTargetLock;
            }
        }

        /// <summary>
        /// Extends OnOnEnable to enable our new InputAction.
        /// </summary>
        
        protected override void OnOnEnable()
        {
            base.OnOnEnable();

            targetLockInputAction?.Enable();
        }

        /// <summary>
        /// Extends OnOnEnable to disable our new InputAction.
        /// </summary>

        protected override void OnOnDisable()
        {
            base.OnOnDisable();

            targetLockInputAction?.Disable();
        }

        #endregion
    }
}
