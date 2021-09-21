using ECM2.Components;
using UnityEngine;

namespace ECM2.Common
{
    public interface ICharacterMovementCallbacks
    {
        /// <summary>
        /// Called when Character's hits ground.
        /// </summary>

        void OnGroundHit(ref GroundHit prevGroundHit, ref GroundHit groundHit);

        /// <summary>
        /// Called when the Character's hits a collider when walking into them.
        /// Can be called multiple times during frame.
        /// </summary>

        void OnMovementHit(ref MovementHit movementHitResult);
        
        /// <summary>
        /// Called during CharacterMovement Move. Applies the Character's current movement move eg: walk, fall, fly, etc.
        /// </summary>

        void OnMove();

        /// <summary>
        /// Calculates the resultant displacement after a movement hit (eg: slide along collision).
        /// </summary>

        Vector3 ComputeCollisionResponseDisplacement(Vector3 displacement, ref MovementHit movementHit);

        /// <summary>
        /// Determines if the Character is able to step up on otherCollider.
        /// Returns true if can step up on the collider, otherwise false.
        /// </summary>

        bool CanStepUp(ref RaycastHit hitResult);

        /// <summary>
        /// Applies a downward force when walking on top of non-kinematic physics objects.
        /// </summary>

        void OnApplyStandingDownwardForce(Rigidbody otherRigidbody);

        /// <summary>
        /// Applies a push force to non-kinematic rigidbody (including characters) when walking into them.
        /// </summary>

        void OnApplyPushForce(ref RigidbodyHit rigidbodyHit);

        /// <summary>
        /// Determines if the Character should be moved with otherRigidbody when standing on top of it (eg: moving platform).
        /// </summary>

        bool ShouldMoveCharacterWhenStandingOn(Rigidbody otherRigidbody);

        /// <summary>
        /// Determines if the Character should be rotated with otherRigidbody when standing on top of it (eg: moving platform).
        /// </summary>

        bool ShouldRotateCharacterWhenStandingOn(Rigidbody otherRigidbody);

        /// <summary>
        /// Determines how to impart the platform's velocity when Character leaves the platform.
        /// </summary>

        void ImpartPlatformVelocity(Vector3 platformVelocity);

        /// <summary>
        /// Determines how to impart the external velocity caused by external forces caused by physics simulation.
        /// </summary>

        void ImpartExternalVelocity(Vector3 externalVelocity);

        /// <summary>
        /// Called when PhysicsVolume has been changed.
        /// </summary>

        void OnPhysicsVolumeChanged(PhysicsVolume newPhysicsVolume);
    }
}
