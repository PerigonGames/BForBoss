using ECM2.Characters;
using FMODUnity;
using UnityEngine;

namespace BForBoss
{
    public class JumpPad : MonoBehaviour
    {
        [SerializeField] private float _launchImpulse = 15.0f;
        [SerializeField] private EventReference _jumpAudio;

        [SerializeField] private bool _overrideVerticalVelocity;
        [SerializeField] private bool _overrideLateralVelocity;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Character character = other.GetComponent<Character>();
            if (character == null)
                return;
            
            RuntimeManager.PlayOneShot(_jumpAudio, transform.position);
            character.PauseGroundConstraint();
            character.LaunchCharacter(transform.up * _launchImpulse, _overrideVerticalVelocity, _overrideLateralVelocity);
            character.ResetJumpCount();
        }
    }
}
