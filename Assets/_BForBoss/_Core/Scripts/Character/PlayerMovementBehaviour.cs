using System;
using Cinemachine;
using ECM2.Characters;
using ECM2.Components;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public partial class PlayerMovementBehaviour : FirstPersonCharacter
    {
        [Header("Cinemachine")]
        public CinemachineVirtualCamera cmWalkingCamera;
        public CinemachineVirtualCamera cmCrouchedCamera;

        [Title("Optional Behaviour")]
        private PlayerDashBehaviour _dashBehaviour = null;
        private PlayerWallRunBehaviour _wallRunBehaviour = null;
        private PlayerSlideBehaviour _slideBehaviour = null;

        private IInputConfiguration _inputConfiguration;
        private PGInputSystem _inputSystem;
        
        private float _unModifiedPlayerSpeed;
        
        public event Action OnDashActivated;
        public event Action Slid;
        
        
        public void Initialize(PGInputSystem inputSystem, IInputConfiguration inputConfiguration = null)
        {
            _inputSystem = inputSystem;
            _inputConfiguration = inputConfiguration ?? new InputConfiguration();
            _unModifiedPlayerSpeed = maxWalkSpeed;
            SetControlConfiguration();
            SetCameraCullingMask();
            
            if (_dashBehaviour != null)
            {
                _inputSystem.OnDashAction += _dashBehaviour.OnDash;
            }
        }

        public override bool CanJump()
        {
            if (_wallRunBehaviour != null)
                return _wallRunBehaviour.CanJump() || base.CanJump();
            return base.CanJump();
        }
        
        public override float GetBrakingDeceleration()
        {
            return IsSliding ? _slideBehaviour.brakingDecelerationSliding : base.GetBrakingDeceleration();
        }

        public override float GetMaxSpeed()
        {
            return IsSliding ? _slideBehaviour.MaxWalkSpeedSliding : base.GetMaxSpeed();
        }

        public override float GetMaxAcceleration()
        {
            return IsWallRunning ? _wallRunBehaviour.GetMaxAcceleration() : base.GetMaxAcceleration();
        }

        public void SetControlConfiguration()
        {
            characterLook.invertLook = !_inputConfiguration.IsInverted;
            characterLook.mouseHorizontalSensitivity = _inputConfiguration.MouseHorizontalSensitivity;
            characterLook.mouseVerticalSensitivity = _inputConfiguration.MouseVerticalSensitivity;
            characterLook.controllerHorizontalSensitivity = _inputConfiguration.ControllerHorizontalSensitivity;
            characterLook.controllerVerticalSensitivity = _inputConfiguration.ControllerVerticalSensitivity;
        }
        
        public void ModifyPlayerSpeed(float modificationMultiplier)
        {
            _unModifiedPlayerSpeed = maxWalkSpeed;
            maxWalkSpeed *= modificationMultiplier;
        }

        public void ResetPlayerSpeed()
        {
            maxWalkSpeed = _unModifiedPlayerSpeed;
        }
        
        protected override void OnAwake()
        {            
            _dashBehaviour = GetComponent<PlayerDashBehaviour>();
            _wallRunBehaviour = GetComponent<PlayerWallRunBehaviour>();
            _slideBehaviour = GetComponent<PlayerSlideBehaviour>();
            base.OnAwake();
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (_dashBehaviour != null)
            {
                _dashBehaviour.Initialize(this, base.GetMovementInput, OnDashActivated);
            }
            if (_slideBehaviour != null)
            {
                _slideBehaviour.Initialize(this, Slid);
            }
            if (_wallRunBehaviour != null)
            {
                _wallRunBehaviour.Initialize(this, base.GetMovementInput, onWallRunFinished: SetJumpCount);
            }
        }

        protected override void AnimateEye()
        {
            // Base class animates the camera for crouching here, cinemachine handles that
        }

        protected override Vector3 CalcDesiredVelocity()
        {
            return IsSliding ? Vector3.zero : base.CalcDesiredVelocity();
        }

        protected override void OnCrouched()
        {
            base.OnCrouched();
            if (_slideBehaviour != null)
            {
                _slideBehaviour.Slide();
            }
            
            cmWalkingCamera.gameObject.SetActive(false);
            cmCrouchedCamera.gameObject.SetActive(true);
        }

        protected override void OnUncrouched()
        {
            base.OnUncrouched();
            if (_slideBehaviour != null)
            {
                _slideBehaviour.StopSliding();
            }
            
            cmCrouchedCamera.gameObject.SetActive(false);
            cmWalkingCamera.gameObject.SetActive(true);
        }

        protected override void Falling(Vector3 desiredVelocity)
        {
            base.Falling(desiredVelocity);
            _wallRunBehaviour?.Falling(desiredVelocity);
        }

        protected override void OnJumped()
        {
            base.OnJumped();
            _wallRunBehaviour?.OnJumped();
        }

        protected override Vector3 CalcJumpVelocity()
        {
            return IsWallRunning ? _wallRunBehaviour.CalcJumpVelocity() : base.CalcJumpVelocity();
        }

        protected override void OnMove()
        {
            base.OnMove();
            if (_dashBehaviour != null)
            {
                _dashBehaviour.OnDashing();
            }
            if (_slideBehaviour != null)
            {
                _slideBehaviour.Sliding();
            }
            _wallRunBehaviour?.OnWallRunning();
        }

        protected override void OnLanded()
        {
            base.OnLanded();
            _wallRunBehaviour?.OnLanded();
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            _wallRunBehaviour?.OnLateUpdate();
        }

        protected override void OnMovementHit(ref MovementHit movementHit)
        {
            base.OnMovementHit(ref movementHit);
            if (_dashBehaviour != null)
            {
                _dashBehaviour.OnMovementHit(movementHit);
            }

            if (_slideBehaviour != null && _slideBehaviour.IsSliding && !movementHit.isWalkable)
            {
                _slideBehaviour.StopSliding();
            }
        }

        protected override void HandleInput()
        {
            base.HandleInput();
            if (_dashBehaviour != null)
            {
                _dashBehaviour.HandleInput();
            }
        }

        protected override void OnOnDestroy()
        {
            base.OnOnDestroy();
            if (cursorLockInputAction != null)
            {
                cursorLockInputAction.started -= OnCursorLock;
            }

            if (cursorUnlockInputAction != null)
            {
                cursorUnlockInputAction.started -= OnCursorUnlock;
            }
        }

        private void SetJumpCount(int count)
        {
            _jumpCount = count;
        }
    }
}
