using ECM2.Characters;
using ECM2.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Examples.Gameplay.TwinStickExample
{
    /// <summary>
    /// This example shows how to extend the Character class to perform a typical twin-stick shooter movement,
    /// where the character is moved with left stick and aim with the right stick. 
    /// </summary>

    public class TwinStickCharacter : Character
    {
        #region FIELDS

        private Vector2 _fireInput;

        #endregion

        #region INPUT ACTIONS

        private InputAction fireInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        private void OnFire(InputAction.CallbackContext context)
        {
            _fireInput = context.ReadValue<Vector2>();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Override UpdateRotation method to add support for right stick aim rotation.
        /// </summary>

        protected override void UpdateRotation()
        {
            // If no fire input...

            if (Mathf.Abs(_fireInput.x) == 0.0f && Mathf.Abs(_fireInput.y) == 0.0f)
            {
                // Rotate towards movement direction

                Vector3 movementDirection = GetMovementDirection();

                RotateTowards(movementDirection);
            }
            else
            {
                // Rotate towards fire input direction,
                // if camera is assigned, transform fire direction to be relative to camera's view direction

                Vector3 fireDirection = new Vector3(_fireInput.x, 0.0f, _fireInput.y);

                if (cameraTransform != null)
                    fireDirection = fireDirection.relativeTo(cameraTransform);

                RotateTowards(fireDirection);
            }
        }

        /// <summary>
        /// Override SetupPlayerInput to add fire input action.
        /// </summary>
        
        protected override void SetupPlayerInput()
        {
            // Setup default input actions

            base.SetupPlayerInput();

            // If no actions, return

            if (actions == null)
                return;

            // Setup fire input action

            fireInputAction = actions.FindAction("Fire");
            if (fireInputAction != null)
            {
                fireInputAction.started += OnFire;
                fireInputAction.performed += OnFire;
                fireInputAction.canceled += OnFire;
            }
        }

        /// <summary>
        /// Extends OnOnEnable to enable our fire input action.
        /// </summary>

        protected override void OnOnEnable()
        {
            // Initialize base class

            base.OnOnEnable();

            // Enable fire input action

            fireInputAction?.Enable();
        }

        /// <summary>
        /// Extends OnOnDisable to disable our fire input action.
        /// </summary>

        protected override void OnOnDisable()
        {
            // De-Initialize base class

            base.OnOnDisable();

            // Disable fire input action

            fireInputAction?.Disable();
        }

        #endregion
    }
}
