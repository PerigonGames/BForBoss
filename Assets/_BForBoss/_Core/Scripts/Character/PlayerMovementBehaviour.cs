using System;
using Cinemachine;
using ECM2.Characters;
using ECM2.Components;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public partial class PlayerMovementBehaviour : FirstPersonCharacter, IPlayerDashEvents, IPlayerSlideEvents, IPlayerWallRunEvents
    {
        [Header("Cinemachine")]
        public CinemachineVirtualCamera cmWalkingCamera;
        public CinemachineVirtualCamera cmCrouchedCamera;
        private PlayerDashBehaviour _dashBehaviour = null;
        private PlayerWallRunBehaviour _wallRunBehaviour = null;
        private PlayerSlideBehaviour _slideBehaviour = null;

        private IInputConfiguration _inputConfiguration;
        private PGInputSystem _inputSystem;
        private IEnergySystem _energySystem;
        
        private float _unModifiedPlayerSpeed;
        
        public event Action OnDashStarted;
        public event Action OnSlideStarted;
        
        
        public void Initialize(IEnergySystem energySystem, PGInputSystem inputSystem, IInputConfiguration inputConfiguration = null)
        {
            _energySystem = energySystem;
            _inputSystem = inputSystem;
            _inputConfiguration = inputConfiguration ?? new InputConfiguration();
            _unModifiedPlayerSpeed = maxWalkSpeed;
            SetControlConfiguration();
            SetCameraCullingMask();
        }

        public override bool CanJump()
        {
            return _wallRunBehaviour.IsWallRunning || base.CanJump();
        }
        
        public override float GetBrakingDeceleration()
        {
            return _slideBehaviour.IsSliding ? _slideBehaviour.brakingDecelerationSliding : base.GetBrakingDeceleration();
        }

        public override float GetMaxSpeed()
        {
            return _slideBehaviour.IsSliding ? _slideBehaviour.MaxWalkSpeedSliding : base.GetMaxSpeed();
        }

        public override float GetMaxAcceleration()
        {
            return _wallRunBehaviour.IsWallRunning ? _wallRunBehaviour.GetMaxAcceleration() : base.GetMaxAcceleration();
        }

        public void SetControlConfiguration()
        {
            characterLook.invertLook = !_inputConfiguration.IsInverted;
            characterLook.mouseHorizontalSensitivity = _inputConfiguration.MouseHorizontalSensitivity;
            characterLook.mouseVerticalSensitivity = _inputConfiguration.MouseVerticalSensitivity;
            characterLook.controllerHorizontalSensitivity = _inputConfiguration.ControllerHorizontalSensitivity;
            characterLook.controllerVerticalSensitivity = _inputConfiguration.ControllerVerticalSensitivity;
            _inputSystem.OnDashAction += _dashBehaviour.OnDash;
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
            if (_dashBehaviour == null)
            {
                PanicHelper.Panic(new Exception("Missing DashBehaviour from PLayerMovement"));
            }
            
            if (_wallRunBehaviour == null)
            {
                PanicHelper.Panic(new Exception("Missing WallRunBehaviour from PLayerMovement"));
            }
            
            if (_slideBehaviour == null)
            {
                PanicHelper.Panic(new Exception("Missing SlideBehaviour from PLayerMovement"));
            }
            base.OnAwake();
        }

        protected override void OnStart()
        {
            base.OnStart();
            _dashBehaviour.Initialize(this, base.GetMovementInput);
            _dashBehaviour.DashEventsDelegate = this;
            _slideBehaviour.Initialize(this);
            _slideBehaviour.SlideEventsDelegate = this;
            _wallRunBehaviour.Initialize(this, base.GetMovementInput);
            _wallRunBehaviour.WallRunEventsDelegate = this;
        }

        protected override void AnimateEye()
        {
            // Base class animates the camera for crouching here, cinemachine handles that
        }

        protected override Vector3 CalcDesiredVelocity()
        {
            return _slideBehaviour.IsSliding ? Vector3.zero : base.CalcDesiredVelocity();
        }

        protected override void OnCrouched()
        {
            base.OnCrouched();
            _slideBehaviour.Slide();

            cmWalkingCamera.gameObject.SetActive(false);
            cmCrouchedCamera.gameObject.SetActive(true);
        }

        protected override void OnUncrouched()
        {
            base.OnUncrouched();
            _slideBehaviour.StopSliding();
            cmCrouchedCamera.gameObject.SetActive(false);
            cmWalkingCamera.gameObject.SetActive(true);
        }

        protected override void Falling(Vector3 desiredVelocity)
        {
            base.Falling(desiredVelocity);
            _wallRunBehaviour.Falling(desiredVelocity);
        }

        protected override void OnJumped()
        {
            base.OnJumped();
            _wallRunBehaviour.OnJumped();
        }

        protected override Vector3 CalcJumpVelocity()
        {
            return _wallRunBehaviour.IsWallRunning ? _wallRunBehaviour.CalcJumpVelocity() : base.CalcJumpVelocity();
        }

        protected override void OnMove()
        {
            base.OnMove();
            _dashBehaviour.DashDuringMovement();
            _slideBehaviour.Sliding();
            _wallRunBehaviour.OnWallRunning();
        }

        protected override void OnLanded()
        {
            base.OnLanded();
            _wallRunBehaviour.OnLanded();
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            _wallRunBehaviour.OnLateUpdate();
        }

        protected override void OnMovementHit(ref MovementHit movementHit)
        {
            base.OnMovementHit(ref movementHit);
            _dashBehaviour.OnMovementHit(movementHit);

            if (_slideBehaviour.IsSliding && !movementHit.isWalkable)
            {
                _slideBehaviour.StopSliding();
            }
        }

        protected override void HandleInput()
        {
            base.HandleInput();
            _dashBehaviour.HandleInput();
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

        #region EventDelegates
        void IPlayerDashEvents.OnDashStopped()
        {
            _energySystem?.Accrue(EnergyAccruementType.Dash);
            OnDashStarted?.Invoke();
        }
        
        void IPlayerSlideEvents.OnSlideStarted()
        {
            _energySystem?.Accrue(EnergyAccruementType.Slide);
            OnSlideStarted?.Invoke();
        }
        
        void IPlayerWallRunEvents.OnWallCompleted(bool didJumpOut)
        {
            _jumpCount = didJumpOut ? 1 : 0;
        }

        void IPlayerWallRunEvents.OnWallRunning()
        {
            _energySystem?.Accrue(EnergyAccruementType.WallRun);
        }
        #endregion
    }
}
