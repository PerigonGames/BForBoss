using ECM2.Components;
using UnityEngine;

namespace ECM2.Demo
{
    public class RotatingPlatform : PlatformMovement
    {
        [SerializeField]
        private float _rotationSpeed = 30.0f;

        /// <summary>
        /// Extends OnMove method to perform platform's rotation.
        /// </summary>

        protected override void OnMove()
        {
            // Update platform's yaw rotation

            rotation *= Quaternion.Euler(0f, _rotationSpeed * Time.deltaTime, 0f);
        }
    }
}
