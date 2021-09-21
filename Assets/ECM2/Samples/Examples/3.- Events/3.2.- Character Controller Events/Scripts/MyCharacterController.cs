using ECM2.Characters;
using ECM2.Common;
using ECM2.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Examples.Events.CharacterControllerEventsExample
{
    /// <summary>
    /// This example shows how a Controller can subscribe to its controlled Character events.
    /// 
    /// </summary>

    public class MyCharacterController : MonoBehaviour
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

        #region CHARACTER EVENTS HANDLERS

        private void OnMovementModeChanged(MovementMode prevMovementMode, int prevCustomMode)
        {
            Debug.Log("Changed from " + prevMovementMode + " to " + _character.GetMovementMode());
        }

        private void OnGroundHit(ref GroundHit prevGroundHitResult, ref GroundHit groundHitResult)
        {
            Debug.Log("Hit Ground " + groundHitResult.collider.name);
        }

        private void OnMovementHit(ref MovementHit movementHitResult)
        {
            Debug.Log("Movement Hit " + movementHitResult.collider.name);
        }

        private void OnJumped()
        {
            Debug.Log("Jump!");

            // Enable jump apex event notification, otherwise wont receive ReachedJumpApex Event

            _character.notifyJumpApex = true;
        }

        private void OnReachedJumpApex()
        {
            Debug.Log("Reached jump apex at " + _character.fallingTime + " seconds");
        }
        
        private void OnWillLand()
        {
            Debug.Log("The Character is about to land");
        }

        private void OnLanded()
        {
            Debug.Log("Landed!");
        }

        private void OnCrouched()
        {
            Debug.Log("The Character has crouched");
        }

        private void OnUncrouched()
        {
            Debug.Log("The Character has Uncrouched");
        }

        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// Subscribe to controlled Character's events.
        /// </summary>

        private void OnEnable()
        {
            _character.MovementModeChanged += OnMovementModeChanged;

            // Commented out as will spam console

            //_character.GroundHit += OnGroundHit;
            _character.MovementHit += OnMovementHit;

            _character.Jumped += OnJumped;
            _character.ReachedJumpApex += OnReachedJumpApex;
            _character.WillLand += OnWillLand;
            _character.Landed += OnLanded;

            _character.Crouched += OnCrouched;  
            _character.Uncrouched += OnUncrouched;
        }

        /// <summary>
        /// Unsubscribe to controlled Character's events.
        /// </summary>

        private void OnDisable()
        {
            _character.MovementModeChanged -= OnMovementModeChanged;

            // Commented out as will spam console

            //_character.GroundHit -= OnGroundHit;
            _character.MovementHit -= OnMovementHit;

            _character.Jumped -= OnJumped;
            _character.ReachedJumpApex -= OnReachedJumpApex;
            _character.WillLand -= OnWillLand;
            _character.Landed -= OnLanded;

            _character.Crouched -= OnCrouched;  
            _character.Uncrouched -= OnUncrouched;
        }

        private void Update()
        {
            // Add movement input relative to camera's view direction (in world space)
            
            Vector3 movementDirection = Vector3.zero;

            movementDirection += Vector3.right * _movementInput.x;
            movementDirection += Vector3.forward * _movementInput.y;

            movementDirection = movementDirection.relativeTo(_camera.transform);

            _character.SetMovementDirection(movementDirection);
        }

        #endregion
    }
}
