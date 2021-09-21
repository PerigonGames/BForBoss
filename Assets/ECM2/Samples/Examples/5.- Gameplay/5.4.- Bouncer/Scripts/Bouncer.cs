using ECM2.Characters;
using UnityEngine;

namespace ECM2.Examples.Gameplay.BouncerExample
{
    /// <summary>
    /// This Example shows how to use the Character's LaunchCharacter function to implement a directional bouncer.
    /// </summary>

    public class Bouncer : MonoBehaviour
    {
        public float launchImpulse = 15.0f;

        public bool overrideVerticalVelocity;
        public bool overrideLateralVelocity;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Character character = other.GetComponent<Character>();
            if (character == null)
                return;

            character.PauseGroundConstraint();
            character.LaunchCharacter(transform.up * launchImpulse, overrideVerticalVelocity, overrideLateralVelocity);
        }
    }
}
