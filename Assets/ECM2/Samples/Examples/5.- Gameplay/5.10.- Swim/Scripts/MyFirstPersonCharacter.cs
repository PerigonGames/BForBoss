using ECM2.Characters;
using UnityEngine;

namespace ECM2.Examples.Gameplay.FirstPersonSwimmingExample
{
    /// <summary>
    /// This example shows how to extend the FirstPersonCharacter to add swimming movement.
    ///
    /// In this case, we allow to swim towards our look direction, allowing to freely move through water volume.
    /// </summary>

    public class MyFirstPersonCharacter : FirstPersonCharacter
    {
        #region METHODS

        /// <summary>
        /// Extends HandleInput method to add specific movement for swimming state.
        /// </summary>

        protected override void HandleInput()
        {
            // Call base method

            base.HandleInput();

            // Handle swimming input

            if (!IsSwimming())
                return;

            // If swimming, move towards eye view direction, and allow to strafe along out right vector

            Vector2 movementInput = GetMovementInput();
            
            Vector3 movementDirection = Vector3.zero;

            movementDirection += GetEyeForwardVector() * movementInput.y;
            movementDirection += GetRightVector() * movementInput.x;

            if (jumpButtonPressed)
            {
                // Use immersion depth to check if we are at top of water line,
                // if yes, jump of of water

                float depth = ImmersionDepth();
                if (depth > 0.65f)
                    movementDirection += GetUpVector();
                else
                {
                    // Jump out of water

                    SetMovementMode(MovementMode.Falling);
                    LaunchCharacter(GetUpVector() * 15f, true);
                }
            }

            SetMovementDirection(movementDirection);
        }

        #endregion
    }
}
