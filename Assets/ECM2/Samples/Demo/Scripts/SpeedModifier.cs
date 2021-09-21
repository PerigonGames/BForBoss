using ECM2.Characters;
using UnityEngine;

namespace ECM2.Demo
{
    public class SpeedModifier : MonoBehaviour
    {
        public float speedMultiplier = 2.0f;

        public float accelerationMultiplier = 2.0f;
        public float decelerationMultiplier = 2.0f;

        public float frictionMultiplier = 2.0f;

        private float _savedMaxWalkSpeed;
        private float _savedMaxAcceleration;
        private float _savedDecelerationWalking;
        private float _savedGroundFriction;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Character character = other.GetComponent<Character>();
            if (!character)
                return;

            // Save character's settings

            _savedMaxWalkSpeed = character.maxWalkSpeed;

            _savedMaxAcceleration = character.maxAcceleration;
            _savedDecelerationWalking = character.brakingDecelerationWalking;

            _savedGroundFriction = character.groundFriction;

            // Apply new settings

            character.maxWalkSpeed *= speedMultiplier;

            character.maxAcceleration *= accelerationMultiplier;
            character.brakingDecelerationWalking *= decelerationMultiplier;

            character.groundFriction *= frictionMultiplier;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            
            Character character = other.GetComponent<Character>();
            if (!character)
                return;

            // Restore character's saved settings

            character.maxWalkSpeed = _savedMaxWalkSpeed;
                
            character.maxAcceleration = _savedMaxAcceleration;
            character.brakingDecelerationWalking = _savedDecelerationWalking;

            character.groundFriction = _savedGroundFriction;
        }
    }
}
