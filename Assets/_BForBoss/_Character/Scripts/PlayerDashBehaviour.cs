using ECM2.Characters;
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

        private Character _baseCharacter = null;
        
        public void Initialize(Character baseCharacter)
        {
            _baseCharacter = baseCharacter;
        }
        
        public void SetupPlayerInput(InputAction dashAction)
        {
            dashAction.started += OnDash;
            dashAction.canceled += OnDash;
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
                var characterVelocity = _baseCharacter.GetVelocity();
                var characterUpVector = _baseCharacter.GetUpVector();
                var dashVelocity = Vector3.ProjectOnPlane(characterVelocity, characterUpVector);
                _baseCharacter.SetVelocity(dashVelocity);
            }

            _dashElapsedTime += Time.deltaTime;
            if (_dashElapsedTime > _dashDuration)
            {
                StopDashing();
            }
        }
        
        private void OnDash(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                var movement = context.ReadValue<Vector2>();
                Dash(movement);
            }
            else if (context.canceled)
            {
                StopDashing();
            }
        }

        private void Dash(Vector2 movementInput)
        {
            _isDashing = true;

            _baseCharacter.brakingFriction = 0.0f;
            _baseCharacter.useSeparateBrakingFriction = true;
            
            _dashingDirection = movementInput.sqrMagnitude > 0 ? _baseCharacter.GetMovementDirection() : _baseCharacter.GetForwardVector(); 

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
