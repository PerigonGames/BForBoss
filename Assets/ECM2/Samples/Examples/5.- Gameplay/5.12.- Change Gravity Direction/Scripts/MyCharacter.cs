using ECM2.Characters;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Examples.Gameplay.ChangeGravityDirectionExample
{
    /// <summary>
    /// This example shows how to extend a Character to change Gravity direction at run-time.
    /// </summary>

    public class MyCharacter : Character
    {
        #region INPUT ACTIONS

        public InputAction toggleGravityDirection { private get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Overrides HandleInput method to replace the default input method with an horizontal only movement.
        /// </summary>

        protected override void HandleInput()
        {
            // Add horizontal only movement (in world space)

            Vector2 movementInput = GetMovementInput();

            Vector3 movementDirection = Vector3.right * movementInput.x;
            
            SetMovementDirection(movementDirection);

            // Toggle gravity direction if character is on air (e.g. Jumping)

            if (!IsOnGround() && toggleGravityDirection.triggered)
                gravity *= -1.0f;
        }

        /// <summary>
        /// Extends UpdateRotation method to orient the Character towards gravity direction.
        /// </summary>

        protected override void UpdateRotation()
        {
            // Handle default rotation (e.g. Rotate towards movement direction).

            base.UpdateRotation();

            // Append gravity-direction rotation

            Quaternion targetRotation = Quaternion.FromToRotation(GetUpVector(), -gravity) * characterMovement.rotation;

            characterMovement.rotation = Quaternion.RotateTowards(characterMovement.rotation, targetRotation, rotationRate * Time.deltaTime);
        }

        /// <summary>
        /// Extends SetupPlayerInput to init Toggle Gravity Direction InputAction.
        /// </summary>

        protected override void SetupPlayerInput()
        {
            base.SetupPlayerInput();

            toggleGravityDirection = actions.FindAction("Toggle Gravity Direction");
        }

        /// <summary>
        /// Extends OnEnable to enable Toggle Gravity Direction Input Action.
        /// </summary>

        protected override void OnOnEnable()
        {
            base.OnOnEnable();

            toggleGravityDirection?.Enable();
        }

        /// <summary>
        /// Extends OnDisable to disable Toggle Gravity Direction Input Action.
        /// </summary>

        protected override void OnOnDisable()
        {
            base.OnOnDisable();

            toggleGravityDirection?.Disable();
        }

        #endregion
    }
}
