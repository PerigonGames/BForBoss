using ECM2.Characters;
using UnityEngine;

namespace ECM2.Examples.Gameplay.FirstPersonFlyingExample
{
    /// <summary>
    /// This example shows how to extend the FirstPersonCharacter to add flying movement.
    ///
    /// In this case, we allow to fly towards our look direction, allowing to freely move through the air.
    /// To implement the flying state, we use the Flying movement mode. The Flying movement mode needs to be manually enabled / disabled as needed.
    /// </summary>

    public class MyFirstPersonCharacter : FirstPersonCharacter
    {
        #region METHODS

        /// <summary>
        /// Determines if the Character is able to enter Flying state
        /// </summary>

        private bool CanFly()
        {
            // Cant fly if is on ground, or is jumping or not has jumped (eg: jumpCount == 0)

            return !IsOnGround() && !IsJumping() && jumpCount > 0;
        }

        /// <summary>
        /// Extends OnMove method to handle flying state.
        /// </summary>

        protected override void OnMove()
        {
            // Call base method

            base.OnMove();

            // If flying and touched walkable ground, exit flying state.
            // Eg: change to falling movement mode

            if (IsFlying() && IsOnWalkableGround())
                SetMovementMode(MovementMode.Falling);
        }

        /// <summary>
        /// Extends HandleInput method to handle flying.
        /// </summary>

        protected override void HandleInput()
        {
            // Handle default movement mode input

            base.HandleInput();

            if (!IsFlying())
            {
                // If wants to fly and CAN fly, enter flying state.
                // Eg: Launch character up and change to flying movement mode.

                if (jumpButtonPressed && CanFly())
                {
                    SetMovementMode(MovementMode.Flying);
                    
                    LaunchCharacter(-gravity * 0.5f);
                }
            }
            else
            {
                // If Flying, move towards eye view direction, and allow to strafe along out right vector and move up pressing jump button

                Vector2 movementInput = GetMovementInput();

                Vector3 movementDirection = Vector3.zero;

                movementDirection += GetEyeForwardVector() * movementInput.y;
                movementDirection += GetRightVector() * movementInput.x;

                if (jumpButtonPressed)
                    movementDirection += GetUpVector();

                SetMovementDirection(movementDirection);
            }
        }

        #endregion
    }
}
