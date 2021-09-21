using ECM2.Characters;
using UnityEngine;

namespace ECM2.Examples.Platforms.OneWayPlatformExample
{
    /// <summary>
    /// This basic example shows one easy method to implement a one-way platform.
    ///
    /// This basically enables / disables the platform / character collisions when the Character's enter / exits the platform trigger volume.
    /// 
    /// </summary>

    public class OneWayPlatform : MonoBehaviour
    {
        public Collider platformCollider;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Character character = other.GetComponent<Character>();
            if (character)
                character.IgnoreCollision(platformCollider);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            
            Character character = other.GetComponent<Character>();
            if (character)
                character.IgnoreCollision(platformCollider, false);
        }
    }
}
