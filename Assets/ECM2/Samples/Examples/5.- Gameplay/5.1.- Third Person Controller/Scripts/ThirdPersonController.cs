using ECM2.Characters;
using ECM2.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Examples.Gameplay.ThirdPersonControllerExample
{
    public class ThirdPersonController : MonoBehaviour
    {
        [SerializeField]
        private ThirdPersonCameraController _cameraController;

        [SerializeField]
        private Character _character;

        protected Vector2 _movementInput;

        protected bool _isMouseInput;
        protected Vector2 _lookInput;
        protected Vector2 _zoomInput;
        
        /// <summary>
        /// Movement Input action handler.
        /// </summary>

        public virtual void OnMovement(InputAction.CallbackContext context)
        {
            // This returns Vector2.zero when context.canceled is true,
            // so no need to handle these separately

            _movementInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Jump Input action handler.
        /// </summary>

        public virtual void OnJump(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                _character.Jump();
            else if (context.canceled)
                _character.StopJumping();
        }

        /// <summary>
        /// Crouch Input action handler.
        /// </summary>

        public virtual void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                _character.Crouch();
            else if (context.canceled)
                _character.StopCrouching();
        }

        /// <summary>
        /// Sprint Input action handler.
        /// </summary>

        public virtual void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                _character.Sprint();
            else if (context.canceled)
                _character.StopSprinting();
        }

        /// <summary>
        /// Mouse Look Input action handler.
        /// </summary>

        public virtual void OnMouseLook(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
            {
                _isMouseInput = true;
                _lookInput = context.ReadValue<Vector2>();
            }
            else if (context.canceled)
            {
                _lookInput = Vector2.zero;
            }
        }

        /// <summary>
        /// Mouse Scroll Input action handler.
        /// </summary>
        
        public virtual void OnMouseScroll(InputAction.CallbackContext context)
        {
            // This returns Vector2.zero when context.canceled is true,
            // so no need to handle these separately

            _zoomInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Controller Look Input action handler.
        /// </summary>

        public virtual void OnControllerLook(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
            {
                _isMouseInput = false;
                _lookInput = context.ReadValue<Vector2>();
            }
            else if (context.canceled)
            {
                _lookInput = Vector2.zero;
            }
        }

        /// <summary>
        /// Cursor lock input action handler.
        /// </summary>

        public virtual void OnCursorLock(InputAction.CallbackContext context)
        {
            if (context.started)
                _cameraController.LockCursor();
        }

        /// <summary>
        /// Cursor unlock input action handler.
        /// </summary>

        public virtual void OnCursorUnlock(InputAction.CallbackContext context)
        {
            if (context.started)
                _cameraController.UnlockCursor();
        }

        protected virtual void HandleCameraInput()
        {
            if (!_cameraController.IsCursorLocked())
                return;

            if (_isMouseInput)
            {
                if (_lookInput.y != 0.0f)
                    _cameraController.LookUp(_lookInput.y);

                if (_lookInput.x != 0.0f)
                    _cameraController.Turn(_lookInput.x);
            }
            else
            {
                if (_lookInput.y != 0.0f)
                    _cameraController.LookUpAtRate(_lookInput.y);

                if (_lookInput.x != 0.0f)
                    _cameraController.TurnAtRate(_lookInput.x);
            }

            if (_zoomInput.y != 0.0f)
                _cameraController.ZoomAtRate(_zoomInput.y);
        }

        protected virtual void HandleCharacterInput()
        {
            Vector3 movementDirection = Vector3.zero;

            movementDirection += Vector3.right * _movementInput.x;
            movementDirection += Vector3.forward * _movementInput.y;

            movementDirection = movementDirection.relativeTo( _cameraController.transform );

            _character.SetMovementDirection(movementDirection);
        }

        private void HandlePlayerInput()
        {
            HandleCameraInput();
            HandleCharacterInput();
        }

        private void Update()
        {
            HandlePlayerInput();
        }
    }
}