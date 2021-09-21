using ECM2.Characters;
using ECM2.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Examples.NewInput.CharacterControllerExample
{
    /// <summary>
    /// This example shows create a Controller to use our character, eg: To take control of our Character.
    ///
    /// Here the Controller will make use of the new input system PlayerInput component to handle player input.
    /// </summary>

    public sealed class MyCharacterController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [SerializeField]
        public Camera _camera;

        [SerializeField]
        public Character _character;

        #endregion

        #region FIELDS

        private Vector2 _movementInput;

        #endregion

        #region INPUT ACTION HANDLERS

        /// <summary>
        /// Movement Input action handler.
        /// </summary>

        public void OnMovement(InputAction.CallbackContext context)
        {
            // This returns Vector2.zero when context.canceled is true,
            // so no need to handle these separately

            _movementInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Jump Input action handler.
        /// </summary>

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                _character.Jump();
            else if (context.canceled)
                _character.StopJumping();
        }

        /// <summary>
        /// Crouch Input action handler.
        /// </summary>

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                _character.Crouch();
            else if (context.canceled)
                _character.StopCrouching();
        }

        /// <summary>
        /// Sprint Input action handler.
        /// </summary>

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                _character.Sprint();
            else if (context.canceled)
                _character.StopSprinting();
        }

        #endregion

        #region METHODS

        private void HandleInput()
        {
            // Add movement input relative to camera's view direction (in world space)
            
            Vector3 movementDirection = Vector3.zero;

            movementDirection += Vector3.right * _movementInput.x;
            movementDirection += Vector3.forward * _movementInput.y;

            movementDirection = movementDirection.relativeTo(_camera.transform);

            _character.SetMovementDirection(movementDirection);
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Update()
        {
            HandleInput();
        }

        #endregion
    }
}
