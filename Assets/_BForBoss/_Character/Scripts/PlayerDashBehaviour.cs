using System;
using ECM2.Components;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Character
{
    public class PlayerDashBehaviour : MonoBehaviour
    {
        [Title("Visual Effects")]
        [SerializeField] private DashLinesEffectBehaviour _dashEffectBehaviour = null;
        
        [Title("Properties")]
        [SerializeField] private float _dashImpulse = 20.0f;
        [SerializeField] private float _dashDuration = 0.2f;
        [SerializeField] private float _dashCoolDown = 0.5f;
        
        private float _dashElapsedTime = 0;
        private float _dashCoolDownElapsedTime = 0;
        private bool _isDashing = false;
        private Vector3 _dashingDirection = Vector3.zero;
        private Func<Vector2> _characterInputMovement = null;

        private ECM2.Characters.Character _baseCharacter = null;
        private Action _onDashing = null;

        public bool IsDashing => _isDashing;
        private bool IsCoolDownOver => _dashCoolDownElapsedTime <= 0;
        
        public void Initialize(ECM2.Characters.Character baseCharacter, Func<Vector2> characterMovement, Action onDashing)
        {
            _baseCharacter = baseCharacter;
            _characterInputMovement = characterMovement;
            _onDashing = onDashing;
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
        
        public void OnDash(bool isDashing)
        {
            if (isDashing)
            {
                Dash();
            }
            else
            {
                StopDashing();
            }
        }

        private void Dash()
        {
            if (CanDash())
            {
                PlayerDashVisuals();
                _onDashing?.Invoke();
                _isDashing = true;
            
                _baseCharacter.brakingFriction = 0.0f;
                _baseCharacter.useSeparateBrakingFriction = true;

                _dashingDirection = _characterInputMovement().sqrMagnitude > 0 ? _baseCharacter.GetMovementDirection() : _baseCharacter.GetForwardVector();
                _baseCharacter.LaunchCharacter(_dashingDirection * _dashImpulse, true, true);
            }
        }

        private void StopDashing()
        {
            if (!_isDashing)
            {
                return;
            }

            _dashCoolDownElapsedTime = _dashCoolDown;
            _dashElapsedTime = 0f;
            VisualEffectsManager.Instance.Revert(HUDVisualEffect.Dash);
            _isDashing = false;
            _baseCharacter.useSeparateBrakingFriction = false;
        }
        
        private bool CanDash()
        {
            if (!IsCoolDownOver)
            {
                return false;
            }

            if (_baseCharacter.IsCrouching())
            {
                return false;
            }
            
            return _baseCharacter.IsWalking() || _baseCharacter.IsFalling();
        }

        private void PlayerDashVisuals()
        {
            if (_dashEffectBehaviour != null)
            {
                _dashEffectBehaviour.Play();
            }

            VisualEffectsManager.Instance.Distort(HUDVisualEffect.Dash);
        }
        
        #region Mono

        private void Awake()
        {
            SetupVisualEffects();
        }

        private void SetupVisualEffects()
        {
            if (_dashEffectBehaviour == null)
            {
                Debug.LogWarning("Dash Effect Missing from PlayerDashBehaviour.cs");
            }
        }

        private void Update()
        {
            if (_dashCoolDownElapsedTime > 0)
            {
                _dashCoolDownElapsedTime -= Time.deltaTime;
            }
        }

        #endregion
    }
}
