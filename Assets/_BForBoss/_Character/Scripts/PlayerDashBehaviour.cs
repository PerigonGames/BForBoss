using System;
using ECM2.Components;
using ECM2.Characters;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Perigon.Character
{
    public class PlayerDashBehaviour : MonoBehaviour
    {
        [Title("Visual Effects")]
        [SerializeField] private ParticleSystem _dashVisualEffects = null;
        [SerializeField] private Volume _dashPostProcessingEffects = null;        
        
        [Title("Properties")]
        [SerializeField] private float _dashImpulse = 20.0f;
        [SerializeField] private float _dashDuration = 0.2f;
        [SerializeField] private float _dashCoolDown = 0.5f;
        private PostProcessingVolumeWeightTool _postProcessingVolumeWeightTool = null;
        
        private float _dashElapsedTime = 0;
        private float _dashCoolDownElapsedTime = 0;
        private bool _isDashing = false;
        private Vector3 _dashingDirection = Vector3.zero;
        private Func<Vector2> _characterInputMovement = null;

        private ECM2.Characters.Character _baseCharacter = null;
        private InputAction _dashInputAction = null;

        private bool IsCoolDownOver => _dashCoolDownElapsedTime <= 0;
        
        public void Initialize(ECM2.Characters.Character baseCharacter, Func<Vector2> characterMovement)
        {
            _baseCharacter = baseCharacter;
            _characterInputMovement = characterMovement;
        }
        
        public void SetupPlayerInput(InputAction dashAction)
        {
            _dashInputAction = dashAction;
            _dashInputAction.started += OnDash;
            _dashInputAction.canceled += OnDash;
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

        public void DisableAction()
        {
            _dashInputAction.Disable();
        }

        public void EnableAction()
        {
            _dashInputAction.Enable();
        }

        private Vector3 GetDashVelocity()
        {
            var characterVelocity = _baseCharacter.GetVelocity();
            var characterUpVector = _baseCharacter.GetUpVector();
            return Vector3.ProjectOnPlane(characterVelocity, characterUpVector);
        }
        
        private void OnDash(InputAction.CallbackContext context)
        {
            if (context.started && IsCoolDownOver)
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
            if (!CanDash())
            {
                return;
            }

            PlayerDashVisuals();
            _isDashing = true;
            
            _baseCharacter.brakingFriction = 0.0f;
            _baseCharacter.useSeparateBrakingFriction = true;

            _dashingDirection = _characterInputMovement().sqrMagnitude > 0 ? _baseCharacter.GetMovementDirection() : _baseCharacter.GetForwardVector();
            _baseCharacter.LaunchCharacter(_dashingDirection * _dashImpulse, true, true);
        }

        private void StopDashing()
        {
            if (!_isDashing)
            {
                return;
            }

            _postProcessingVolumeWeightTool?.Revert();
            _dashCoolDownElapsedTime = _dashCoolDown;
            _dashElapsedTime = 0f;
            _isDashing = false;
            _baseCharacter.useSeparateBrakingFriction = false;
        }
        
        private bool CanDash()
        {
            // Do not allow to dash if crouched

            if (_baseCharacter.IsCrouching())
                return false;

            // Only allow to dash if IsWalking or IsFalling (Eg: in air)

            return _baseCharacter.IsWalking() || _baseCharacter.IsFalling();
        }

        private void PlayerDashVisuals()
        {
            if (_dashVisualEffects != null)
            {
                _dashVisualEffects.Play();
            }

            _postProcessingVolumeWeightTool?.Distort();
        }
        
        #region Mono

        private void Awake()
        {
            SetupVisualEffects();
        }

        private void SetupVisualEffects()
        {
            if (_dashPostProcessingEffects != null)
            {
                _postProcessingVolumeWeightTool = new PostProcessingVolumeWeightTool(_dashPostProcessingEffects, _dashDuration);
            }
            else
            {
                Debug.LogWarning("There was an issue finding PostProcessing LensDistortion");
            }
        }

        private void Update()
        {
            if (_dashCoolDownElapsedTime > 0)
            {
                _dashCoolDownElapsedTime -= Time.deltaTime;
            }
        }

        public void OnOnEnable()
        {
            _dashInputAction.Enable();
        }
        
        public void OnOnDisable()
        {
            _dashInputAction.Disable();
        }

        public void OnOnDestroy()
        {
            _dashInputAction.started -= OnDash;
            _dashInputAction.canceled -= OnDash;
            _dashInputAction = null;
        }

        #endregion
    }
}
