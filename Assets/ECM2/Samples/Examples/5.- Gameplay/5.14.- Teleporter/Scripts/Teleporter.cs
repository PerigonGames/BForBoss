using ECM2.Characters;
using ECM2.Common;
using UnityEngine;

namespace ECM2.Examples.Gameplay.TeleporterExample
{
    /// <summary>
    /// This example shows how to teleport a character while (if desired) orient towards destination forward and preserve its momentum.
    /// </summary>

    public class Teleporter : MonoBehaviour
    {
        [Tooltip("The destination teleporter.")]
        public Teleporter destination;

        [Tooltip("If true, the character will orient towards the destination Teleporter forward (yaw only)")]
        public bool OrientWithDestination;

        // Is the Character being teleported ?
        // This helps to prevent the character be instantly teleported back

        public bool isTeleporting { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            // If no destination, return

            if (destination == null)
                return;

            // If teleporting, return

            if (isTeleporting)
                return;

            // If no is a player, return

            if (!other.CompareTag("Player"))
                return;

            // If no is a Character, return

            var character = other.GetComponent<Character>();
            if (!character)
                return;

            // Teleport character to destination

            var destinationTransform = destination.transform;
            character.TeleportPosition( destinationTransform.position );

            // Should orient with destination ?

            if ( OrientWithDestination )
            {
                // Update character's rotation towards destination forward vector (yaw only)

                var characterUp  = character.GetUpVector();
                var teleporterForward = destinationTransform.forward.projectedOnPlane(characterUp);

                var targetRotation = Quaternion.LookRotation(teleporterForward, character.GetUpVector());
                
                character.SetYaw( targetRotation.eulerAngles.y );

                // Re-orient character's velocity along teleporter forward
                
                character.LaunchCharacter(teleporterForward * character.GetSpeed(), false, true);
            }            

            // Prevent destination from teleport us back

            destination.isTeleporting = true;
        }

        private void OnTriggerExit(Collider other)
        {
            // Re-enable teleporter

            isTeleporting = false;
        }
    }
}
