using ECM2.Characters;
using ECM2.Common;
using UnityEngine;

namespace ECM2.Examples.OldInput.ThirdPersonExample
{
    /// <summary>
    /// This example shows how to replace the Unity Input system with old input system.
    /// </summary>
    /// <seealso cref="ECM2.Characters.ThirdPersonCharacter" />

    public class MyCharacter : ThirdPersonCharacter
    {
        /// <summary>
        /// Handles the character input using old input system.
        /// </summary>
        
        private void HandleCharacterInput()
        {
            // Read input

            Vector2 movementInput = new Vector2
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical"),
            };

            // Add input movement relative to camera look direction
            
            Vector3 movementDirection = Vector3.zero;
            
            movementDirection += Vector3.right * movementInput.x;
            movementDirection += Vector3.forward * movementInput.y;
            
            movementDirection = movementDirection.relativeTo(cameraTransform);
            
            SetMovementDirection(movementDirection);

            // Jump

            if (Input.GetButtonDown("Jump"))
                Jump();
            else if (Input.GetButtonUp("Jump"))
                StopJumping();

            // Crouch

            if (Input.GetKeyDown(KeyCode.LeftControl))
                Crouch();
            else if (Input.GetKeyUp(KeyCode.LeftControl))
                StopCrouching();

            // Sprint

            if (Input.GetKeyDown(KeyCode.LeftShift))
                Sprint();
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                StopSprinting();
        }
        
        /// <summary>
        /// Override HandleCameraInput method to add support for unity old input system.
        /// </summary>
        
        protected override void HandleCameraInput()
        {
            // If cursor is unlocked, do not process camera input

            if (!cameraController.IsCursorLocked())
                return;

            // Mouse look
            
            Vector2 mouseLookInput = new Vector2
            {
                x = Input.GetAxisRaw("Mouse X"),
                y = Input.GetAxisRaw("Mouse Y"),
            };

            if (mouseLookInput.x != 0.0f)
                cameraController.Turn(mouseLookInput.x);
            
            if (mouseLookInput.y != 0.0f)
                cameraController.LookUp(mouseLookInput.y);

            // Mouse scroll input

            float mouseScrollInput = Input.mouseScrollDelta.y;
            if (mouseScrollInput != 0.0f)
                cameraController.ZoomAtRate(mouseScrollInput);
        }

        /// <summary>
        /// Extends the HandleInput to support old input movement system.
        /// </summary>
        
        protected override void HandleInput()
        {
            // Handle character input

            HandleCharacterInput();

            // Handle third person camera input

            HandleCameraInput();
        }
    }
}
