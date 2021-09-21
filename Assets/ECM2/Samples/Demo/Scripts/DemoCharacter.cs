using ECM2.Characters;
using UnityEngine;

namespace ECM2.Demo
{
    public class DemoCharacter : ThirdPersonCharacter
    {
        /// <summary>
        /// Extends HandleInput method to add water movement.
        /// </summary>

        protected override void HandleInput()
        {
            // Handle regular movement

            base.HandleInput();

            if (IsSwimming() && jumpButtonPressed)
            {
                // Use immersion depth to check if we are at top of water line...

                float immersionDepth = ImmersionDepth();
                if (immersionDepth > 0.65f)
                {
                    // No, move up

                    Vector3 movementDirection = GetMovementDirection() + GetUpVector();

                    SetMovementDirection(movementDirection);
                }
                else
                {
                    // Yes, jump out of water

                    SetMovementMode(MovementMode.Falling);
                    LaunchCharacter(GetUpVector() * 15f, true);
                }
            }
        }
    }
}