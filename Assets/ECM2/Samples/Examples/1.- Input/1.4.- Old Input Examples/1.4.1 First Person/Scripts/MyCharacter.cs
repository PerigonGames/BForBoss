using ECM2.Characters;
using UnityEngine;

namespace ECM2.Examples.OldInput.FirstPersonExample
{
    /// <summary>
    /// This example shows how to replace the Unity Input system with old input system.
    /// </summary>
    /// <seealso cref="ECM2.Characters.FirstPersonCharacter" />

    public class MyCharacter : FirstPersonCharacter
    {
        /// <summary>
        /// Handles the character input using old input system.
        /// </summary>
        
        private void HandleCharacterInput()
        {
            // Movement

            Vector2 movementInput = new Vector2
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical"),
            };

            // Add input movement relative to us
            
            Vector3 movementDirection = Vector3.zero;
            
            movementDirection += GetRightVector() * movementInput.x;
            movementDirection += GetForwardVector() * movementInput.y;
            
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

        public void HandleCameraInput()
        {
            // If Character is disabled, halts camera look

            if (IsDisabled())
                return;

            // Mouse look
            
            Vector2 mouseLookInput = new Vector2
            {
                x = Input.GetAxisRaw("Mouse X"),
                y = Input.GetAxisRaw("Mouse Y"),
            };

            if (mouseLookInput.x != 0.0f)
                AddYawInput(mouseLookInput.x * characterLook.mouseHorizontalSensitivity);
            
            if (mouseLookInput.y != 0.0f)
                AddEyePitchInput(mouseLookInput.y * characterLook.mouseVerticalSensitivity);
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
