using Cinemachine;
using ECM2.Characters;
using ECM2.Components;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Character
{
    public partial class FirstPersonPlayer : FirstPersonCharacter
    {
        [Header("Cinemachine")]
        public CinemachineVirtualCamera cmWalkingCamera;
        public CinemachineVirtualCamera cmCrouchedCamera;
        public CinemachineVirtualCamera cmThirdPersonCamera;

        [Title("Optional Behaviour")]
        private PlayerDashBehaviour _dashBehaviour = null;
        private PlayerWallRunBehaviour _wallRunBehaviour = null;
        private PlayerSlideBehaviour _slideBehaviour = null;

        [SerializeField] private bool _isThirdPerson = false;

        public bool IsThirdPerson
        {
            get => _isThirdPerson;
            set
            {
                if (value == _isThirdPerson) return;
                _isThirdPerson = value;
                ToggleThirdPerson();
            }
        }

        public void Initialize()
        {
            SetupInput();
            SetupThirdPerson();
        }

        public override bool CanJump()
        {
            if (_wallRunBehaviour != null)
                return _wallRunBehaviour.CanJump() || base.CanJump();
            return base.CanJump();
        }
        
        public override float GetBrakingDeceleration()
        {
            return IsSliding() ? _slideBehaviour.brakingDecelerationSliding : base.GetBrakingDeceleration();
        }

        public override float GetMaxSpeed()
        {
            return IsSliding() ? _slideBehaviour.MaxWalkSpeedSliding : base.GetMaxSpeed();
        }

        public override float GetMaxAcceleration()
        {
            return IsWallRunning() ? _wallRunBehaviour.GetMaxAcceleration() : base.GetMaxAcceleration();
        }

        protected override void OnAwake()
        {            
            _dashBehaviour = GetComponent<PlayerDashBehaviour>();
            _wallRunBehaviour = GetComponent<PlayerWallRunBehaviour>();
            _slideBehaviour = GetComponent<PlayerSlideBehaviour>();

            _thirdPersonMask = camera.cullingMask;

            base.OnAwake();
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (_dashBehaviour != null)
            {
                _dashBehaviour.Initialize(this, base.GetMovementInput);
            }
            if (_slideBehaviour != null)
            {
                _slideBehaviour.Initialize(this);
            }
            _wallRunBehaviour?.Initialize(this, base.GetMovementInput, SetJumpCount);

            TogglePlayerModel();
        }
        
        protected override void SetupPlayerInput()
        {
            base.SetupPlayerInput();
            if (_dashBehaviour != null)
            {
                InputAction dashInputAction = actions.FindAction("Dash");
                _dashBehaviour.SetupPlayerInput(dashInputAction);
            }
        }

        protected override void AnimateEye()
        {
            // Base class animates the camera for crouching here, cinemachine handles that
        }

        protected override Vector3 CalcDesiredVelocity()
        {
            return IsSliding() ? Vector3.zero : base.CalcDesiredVelocity();
        }

        protected override void OnCrouched()
        {
            base.OnCrouched();
            if (_slideBehaviour != null)
            {
                _slideBehaviour.Slide();
            }
            if (!_isThirdPerson)
            {
                cmWalkingCamera.gameObject.SetActive(false);
                cmCrouchedCamera.gameObject.SetActive(true);
            }
        }

        protected override void OnUncrouched()
        {
            base.OnUncrouched();
            if (_slideBehaviour != null)
            {
                _slideBehaviour.StopSliding();
            }
            if (!_isThirdPerson)
            {
                cmCrouchedCamera.gameObject.SetActive(false);
                cmWalkingCamera.gameObject.SetActive(true);
            }
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
            return IsWallRunning() ? _wallRunBehaviour.CalcJumpVelocity() : base.CalcJumpVelocity();
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

        protected override void OnOnDisable()
        {
            base.OnOnDisable();
            if (_dashBehaviour != null)
            {
                _dashBehaviour.OnOnDisable();
            }
        }
        
        protected override void OnOnEnable()
        {
            base.OnOnEnable();
            if (_dashBehaviour != null)
            {
                _dashBehaviour.OnOnEnable();
            }
        }

        protected override void OnOnDestroy()
        {
            base.OnOnDestroy();
            if (_dashBehaviour != null)
            {
                _dashBehaviour.OnOnDestroy();
            }
        }

        private void SetJumpCount(int count)
        {
            _jumpCount = count;
        }
        
        protected override void OnOnValidate()
        {
            base.OnOnValidate();
            if(IsThirdPerson != IsThirdPersonCamActive())
            {
                ToggleThirdPerson();
            }
        }

        #region Helper

        private bool IsThirdPersonCamActive()
        {
            return cmThirdPersonCamera != null && cmThirdPersonCamera.gameObject.activeSelf;
        }

        private bool IsSliding()
        {
            return _slideBehaviour != null && _slideBehaviour.IsSliding;
        }

        private bool IsWallRunning()
        {
            return _wallRunBehaviour != null && _wallRunBehaviour.IsWallRunning;
        }

        #endregion
    }
}
