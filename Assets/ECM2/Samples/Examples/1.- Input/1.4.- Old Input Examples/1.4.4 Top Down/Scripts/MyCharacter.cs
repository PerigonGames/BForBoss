using ECM2.Characters;
using UnityEngine;

namespace ECM2.Examples.OldInput.TopDownExample
{
    /// <summary>
    /// This example shows how to replace the Unity Input system with old input system.
    /// </summary>
    /// <seealso cref="ECM2.Characters.AgentCharacter" />

    public class MyCharacter : AgentCharacter
    {
        /// <summary>
        /// Handles the character input using old input system.
        /// </summary>

        private void HandleCharacterInput()
        {
            // Movement (click-to-move)

            if (Input.GetMouseButton(0))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                LayerMask groundMask = characterMovement.groundMask;

                QueryTriggerInteraction queryTriggerInteraction = characterMovement.collideWithTriggers
                    ? QueryTriggerInteraction.Collide
                    : QueryTriggerInteraction.Ignore;

                if (Physics.Raycast(ray, out RaycastHit hitResult, Mathf.Infinity, groundMask, queryTriggerInteraction))
                    MoveToLocation(hitResult.point);
            }

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
        /// Extends Handle input to use old input system.
        /// </summary>

        protected override void HandleInput()
        {
            HandleCharacterInput();
        }
    }
}
