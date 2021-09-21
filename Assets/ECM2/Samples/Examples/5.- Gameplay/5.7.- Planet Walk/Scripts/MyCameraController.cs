using ECM2.Common;
using UnityEngine;

namespace ECM2.Examples.Gameplay.PlanetWalkExample
{
    /// <summary>
    /// Extends the third person camera to allow an arbitrary up direction.
    /// Used by the planet walk example.
    /// </summary>

    public sealed class MyCameraController : ThirdPersonCameraController
    {
        // Our current forward direction perpendicular to target's up vector

        private Vector3 _forward = Vector3.forward;

        /// <summary>
        /// Adds the given input value to this yaw.
        /// </summary>        
        
        protected override void AddYawInput(float value)
        {
            // Rotate our forward along follow target's up axis

            var up = follow.up;

            _forward = Quaternion.Euler(up * value) * _forward;
        }

        /// <summary>
        /// Set the camera current orientation.
        /// </summary>
        
        protected override void UpdateCameraRotation()
        {
            // Makesure camera forward vector is perpendicular to Character's current up vector

            var up = follow.up;

            Vector3.OrthoNormalize(ref up, ref _forward);

            // Computes final Camera rotation from yaw and pitch

            transform.rotation = Quaternion.LookRotation(_forward, up) * Quaternion.Euler(_pitch, 0.0f, 0.0f);
        }
    }
}
