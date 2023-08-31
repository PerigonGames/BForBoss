using FMODUnity;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Collider))]
    public class DerekShieldBehaviour : MonoBehaviour
    {
        [SerializeField, Range(0f,1f), Tooltip("Higher values mean a more vertical launch off the wall")] private float _yLaunchComponent = .7f;
        [SerializeField, Range(0.5f, 10f), Tooltip("Multiplier for the player's current velocity")] private float _launchMagnitudeMultiplier = 1.5f;

        [SerializeField, MinMaxSlider(0f, 50f)] private Vector2 _launchMagnitudeRange = new Vector2(10f, 30f);

        [SerializeField] private EventReference _shieldHitAudio;

        public void ToggleShield(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        private Vector3 CenterAtGroundLevel
        {
            get
            {
                var pos = transform.position;
                pos.y = 0;
                return pos;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            RuntimeManager.PlayOneShot(_shieldHitAudio, other.contacts[0].point);
            var player = other.collider.GetComponent<PlayerBehaviour>();
            if(player != null)
                LaunchPlayer(player.PlayerMovement);
        }

        private void LaunchPlayer(PlayerMovementBehaviour playerMovement)
        {
            var playerSpeed = playerMovement.GetSpeed();
            var playerPos = playerMovement.GetPosition();
            playerPos.y = 0;
            var direction = playerPos - CenterAtGroundLevel;
            direction.Normalize();
            direction.y = _yLaunchComponent;

            var magnitude = _launchMagnitudeRange.ClampToRange(playerSpeed * _launchMagnitudeMultiplier);
            direction *= magnitude;

            playerMovement.PauseGroundConstraint();
            playerMovement.LaunchCharacter(direction, overrideVerticalVelocity: true, overrideLateralVelocity: true);
        }
    }
}
