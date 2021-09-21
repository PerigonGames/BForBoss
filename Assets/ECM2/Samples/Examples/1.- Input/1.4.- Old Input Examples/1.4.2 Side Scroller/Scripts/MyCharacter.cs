using ECM2.Characters;
using UnityEngine;

namespace ECM2.Examples.OldInput.SideScrollerExample
{
    /// <summary>
    /// This shows how to use the old input system to perform tpycal side scroller movement.
    /// </summary>

    public class MyCharacter : Character
    {
        /// <summary>
        /// Handles the character input using old input system.
        /// </summary>

        protected override void HandleInput()
        {
            // Add horizontal input movement (in world space)

            Vector3 movementDirection = Vector3.right * Input.GetAxisRaw("Horizontal");

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
        /// Extends UpdateRotation method to perform instant side to side lock rotation.
        /// </summary>
     
        protected override void UpdateRotation()
        {
            if (rotationRate > 0.0f || GetRotationMode() != RotationMode.OrientToMovement)
                base.UpdateRotation();
            else
            {
                Vector3 movementDirection = GetMovementDirection();
                if (movementDirection.x != 0.0f)
                    transform.rotation = Quaternion.LookRotation(movementDirection, GetUpVector());
            }
        }        
    }
}