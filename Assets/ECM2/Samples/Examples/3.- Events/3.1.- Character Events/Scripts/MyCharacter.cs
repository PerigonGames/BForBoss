using ECM2.Characters;
using ECM2.Components;
using UnityEngine;

namespace ECM2.Examples.Events.CharacterEventsExample
{
    /// <summary>
    /// This shows how to extend the Character 'On' event handlers to respond to Character specific events like: landed, jumped, etc.
    /// Its important when extending a OnXXX event handler, always call its base.OnXXX first, as this is the responsible of actually trigger the Event.
    /// </summary>

    public class MyCharacter : Character
    {
        #region EVENTS

        protected override void OnMovementModeChanged(MovementMode prevMovementMode, int prevCustomMode)
        {
            // Call base method implementation

            base.OnMovementModeChanged(prevMovementMode, prevCustomMode);

            Debug.Log("Changed from " + prevMovementMode + " to " + GetMovementMode());
        }

        protected override void OnGroundHit(ref GroundHit prevGroundHitResult, ref GroundHit groundHitResult)
        {
            // Call base method implementation

            base.OnGroundHit(ref prevGroundHitResult, ref groundHitResult);
            
            // Commented as this will spam the console

            // Debug.Log("Hit Ground " + groundHitResult.transform.name);
        }

        protected override void OnMovementHit(ref MovementHit movementHitResult)
        {
            // Call base method implementation

            base.OnMovementHit(ref movementHitResult);

            Debug.Log("Movement Hit " + movementHitResult.collider.name);
        }

        protected override void OnJumped()
        {
            // Call base method implementation

            base.OnJumped();

            // Enable jump apex event notification, otherwise wont receive ReachedJumpApex Event

            notifyJumpApex = true;

            Debug.Log("Jump!");
        }

        protected override void OnLaunched(Vector3 launchVelocity, bool overrideVerticalVelocity, bool overrideLateralVelocity)
        {
            // Call base method implementation

            base.OnLaunched(launchVelocity, overrideVerticalVelocity, overrideLateralVelocity);

            Debug.Log($"The Character has been launched with {launchVelocity.ToString("F4")} launchVelocity");
        }

        protected override void OnReachedJumpApex()
        {
            // Call base method implementation

            base.OnReachedJumpApex();

            Debug.Log("Reached jump apex at " + fallingTime + " seconds");
        }

        protected override void OnWillLand()
        {
            base.OnWillLand();

            Debug.Log("The Character is about to land");
        }

        protected override void OnLanded()
        {
            // Call base method implementation

            base.OnLanded();

            Debug.Log("Landed!");
        }

        protected override void OnCrouched()
        {
            // Call base method implementation

            base.OnCrouched();

            Debug.Log("The Character has crouched");
        }

        protected override void OnUncrouched()
        {
            // Call base method implementation

            base.OnUncrouched();

            Debug.Log("The Character has Uncrouched");
        }

        #endregion
    }
}
