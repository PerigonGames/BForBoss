using UnityEngine;
using ECM2.Characters;

namespace ECM2.Examples.Gameplay.FallDamageExample
{
    /// <summary>
    /// This example shows how to extend a Character,
    /// and use its events to compute a 'fallen' distance which can be used to apply a fall damage.
    /// </summary>

    public class MyCharacter : Character
    {
        #region FIELDS

        private Vector3 _lastPositionOnWalkableGround;

        #endregion

        #region METHODS

        /// <summary>
        /// Extends OnMovementModeChanged to save our las position on walkable ground.
        /// </summary>

        protected override void OnMovementModeChanged(MovementMode prevMovementMode, int prevCustomMode)
        {
            // Call base method

            base.OnMovementModeChanged(prevMovementMode, prevCustomMode);

            // If Was walking and started to fall, save our last position on walkable ground

            if (prevMovementMode == MovementMode.Walking && IsFalling())
                _lastPositionOnWalkableGround = GetPosition();
        }

        /// <summary>
        /// Extends OnLanded to compute our 'fallen' distance,
        /// eg: the vertical distance from our last position on walkable ground to our landed position.
        /// </summary>

        protected override void OnLanded()
        {
            // Call base method

            base.OnLanded();

            // Compute the signed vertical distance from our last position on walkable ground to our landed position.

            var fallenDistance = Vector3.Dot(GetPosition() - _lastPositionOnWalkableGround, GetUpVector());
            if (fallenDistance < 0.0f)
            {
                // if your fallen distance is grater than a given 'safe fall distance' apply fall damage!
                
                Debug.Log($"fallenDistance: {fallenDistance:F2} fallingTime: {fallingTime:F2}");
            }
        }

        #endregion
    }
}


