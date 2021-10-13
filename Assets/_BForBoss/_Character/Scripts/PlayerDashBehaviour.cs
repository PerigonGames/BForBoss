using System;
using ECM2.Characters;
using ECM2.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BForBoss
{
    public class PlayerDashBehaviour : MonoBehaviour
    {
        [SerializeField] private float _dashImpulse = 20.0f;
        [SerializeField] private float _dashDuration = 0.2f;

        private float _dashElapsedTime = 0;
        private bool _isDashing = false;
        private Vector3 _dashingDirection = Vector3.zero;
        private Func<Vector2> _characterInputMovement = null;

        private Character _baseCharacter = null;
        
        public void Initialize(Character baseCharacter, Func<Vector2> characterMovement)
        {
            _baseCharacter = baseCharacter;
            _characterInputMovement = characterMovement;
        }
        
        public void SetupPlayerInput(InputAction dashAction)
        {
            dashAction.started += OnDash;
            dashAction.canceled += OnDash;
            dashAction.Enable();
        }

        public void OnMovementHit(MovementHit movementHitResult)
        {
            if (_isDashing && !movementHitResult.isWalkable)
            {
                StopDashing();
                var characterVelocity = _baseCharacter.IsOnWalkableGround()
                    ? Vector3.zero
                    : GetDashVelocity();
                _baseCharacter.SetVelocity(characterVelocity);
            }
        }

        public void HandleInput()
        {
            if (_isDashing)
            {
                _baseCharacter.SetMovementDirection(_dashingDirection);
            }
        }
        
        public void OnDashing()
        {
            if (!_isDashing)
            {
                return;
            }

            if (_baseCharacter.IsFalling())
            {
                _baseCharacter.SetVelocity(GetDashVelocity());
            }

            _dashElapsedTime += Time.deltaTime;
            if (_dashElapsedTime > _dashDuration)
            {
                StopDashing();
            }
        }

        private Vector3 GetDashVelocity()
        {
            var characterVelocity = _baseCharacter.GetVelocity();
            var characterUpVector = _baseCharacter.GetUpVector();
            return Vector3.ProjectOnPlane(characterVelocity, characterUpVector);
        }
        
        private void OnDash(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Dash();
            }
            else if (context.canceled)
            {
                StopDashing();
            }
        }

        private void Dash()
        {
            _isDashing = true;
            
            _baseCharacter.brakingFriction = 0.0f;
            _baseCharacter.useSeparateBrakingFriction = true;

            _dashingDirection = _characterInputMovement().sqrMagnitude > 0 ? _baseCharacter.GetMovementDirection() : _baseCharacter.GetForwardVector();
            _baseCharacter.LaunchCharacter(_dashingDirection * _dashImpulse, true, true);
        }

        private void StopDashing()
        {
            _dashDuration = 0f;
            _isDashing = false;
            _baseCharacter.useSeparateBrakingFriction = false;
        }
        
    }
}
