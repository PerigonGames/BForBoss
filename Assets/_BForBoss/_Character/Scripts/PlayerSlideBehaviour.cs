using ECM2.Common;
using UnityEngine;

namespace Perigon.Character
{
    public class PlayerSlideBehaviour : MonoBehaviour
    {
        [SerializeField] private float _maxWalkSpeedSliding = 12;
        [SerializeField] private float _groundFrictionSliding = 0.5f;
        [SerializeField] private float _brakingDecelerationSliding = 10;
        [SerializeField] private float _slideImpulse = 10;
        
        private bool _canSlide = true;
        private bool _isSliding = false;

        private ECM2.Characters.Character _baseCharacter = null;

        public bool IsSliding => _isSliding;

        public float MaxWalkSpeedSliding => _maxWalkSpeedSliding;
        public float brakingDecelerationSliding => _brakingDecelerationSliding;

        public void Initialize(ECM2.Characters.Character character)
        {
            _baseCharacter = character;
        }
        
        public void Slide()
        {
            if (!CanSlide())
            {
                return;
            }

            _isSliding = true;
            
            _baseCharacter.brakingFriction = _groundFrictionSliding;
            _baseCharacter.useSeparateBrakingFriction = true;

            // Add slide impulse to character's current velocity

            Vector3 slideDirection = _baseCharacter.GetVelocity().normalized;
            _baseCharacter.LaunchCharacter(slideDirection * _slideImpulse);
        }
        
        public void StopSliding()
        {
            // If sliding, stop the sliding and restore ground friction

            if (!_isSliding)
            {
                return;
            }
            
            _isSliding = false;

            _baseCharacter.useSeparateBrakingFriction = false;
        }

        public void Sliding()
        {
            // Is the character sliding?
            if (!_isSliding)
            {
                return;
            }
            
            if (_baseCharacter.IsOnGround())
            {
                // While on ground, add gravity to accelerate down slope and slowdown up slope

                Vector3 velocity = _baseCharacter.GetVelocity();
                Vector3 gravityVector = _baseCharacter.GetGravityVector();
                
                velocity += gravityVector.projectedOnPlane(_baseCharacter.GetCharacterMovement().groundHit.normal) * Time.deltaTime;

                _baseCharacter.SetVelocity(velocity);

                // If our current velocity is lower than maxWalkSpeedCrouched, stop sliding

                var isSlideSpeedSlowerThanCrouchSpeed =
                    velocity.sqrMagnitude < MathLib.Square(_baseCharacter.maxWalkSpeedCrouched);
                if (isSlideSpeedSlowerThanCrouchSpeed)
                {
                    StopSliding();
                }
                
            }
            else
            {
                StopSliding();
            }
        }

        private bool CanSlide()
        {
            if (_isSliding)
            {
                return false;
            }

            var movementDirection = _baseCharacter.GetMovementDirection();
            if (movementDirection.isZero())
            {
                return false;
            }

            return true;
        }
    }
}
