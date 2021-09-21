using ECM2.Common;
using ECM2.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ECM2.Characters
{
    /// <summary>
    /// FirstPersonCharacter.
    ///
    /// This extends the Character class to add controls for a typical first person movement. 
    /// </summary>

    [RequireComponent(typeof(CharacterLook))]
    public class FirstPersonCharacter : Character
    {
        #region EDITOR EXPOSED FIELDS

        [Header("First Person")]
        [Tooltip("The first person rig root pivot. This handles the Yaw rotation.")]
        public Transform rootPivot;

        [Tooltip("The first person rig eye pivot. This handles the Pitch rotation.")]
        public Transform eyePivot;

        [Space(15f)]
        [Tooltip("The default eye height (eg: walking).")]
        [SerializeField]
        private float _eyeHeight;

        [Tooltip("The eye height while Character is crouched.")]
        [SerializeField]
        private float _eyeHeightCrouched;

        [Space(15f)]
        [Tooltip("The speed multiplier while Character is walking forward.")]
        [SerializeField]
        private float _forwardSpeedMultiplier;

        [Tooltip("The speed multiplier while Character is walking backward.")]
        [SerializeField]
        private float _backwardSpeedMultiplier;

        [Tooltip("The speed multiplier while Character is walking sideways.")]
        [SerializeField]
        private float _strafeSpeedMultiplier;
        
        #endregion

        #region FIELDS

        private CharacterLook _characterLook;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Cached CharacterLook component.
        /// </summary>

        protected CharacterLook characterLook
        {
            get
            {
                if (_characterLook == null)
                    _characterLook = GetComponent<CharacterLook>();

                return _characterLook;
            }
        }

        /// <summary>
        /// Default eye height (in meters).
        /// </summary>

        public float eyeHeight
        {
            get => _eyeHeight;
            set => _eyeHeight = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Default crouched eye height (in meters).
        /// </summary>

        public float eyeHeightCrouched
        {
            get => _eyeHeightCrouched;
            set => _eyeHeightCrouched = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The speed multiplier while Character is walking forward.
        /// </summary>

        public float forwardSpeedMultiplier
        {
            get => _forwardSpeedMultiplier;
            set => _forwardSpeedMultiplier = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The speed multiplier while Character is walking backwards.
        /// </summary>

        public float backwardSpeedMultiplier
        {
            get => _backwardSpeedMultiplier;
            set => _backwardSpeedMultiplier = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The speed multiplier while Character is strafing.
        /// </summary>

        public float strafeSpeedMultiplier
        {
            get => _strafeSpeedMultiplier;
            set => _strafeSpeedMultiplier = Mathf.Max(0.0f, value);
        }

        #endregion

        #region INPUT ACTIONS

        /// <summary>
        /// Mouse Look InputAction.
        /// </summary>

        protected InputAction mouseLookInputAction { get; set; }

        /// <summary>
        /// Controller Look InputAction.
        /// </summary>

        protected InputAction controllerLookInputAction { get; set; }

        /// <summary>
        /// Cursor lock InputAction.
        /// </summary>

        protected InputAction cursorLockInputAction { get; set; }

        /// <summary>
        /// Cursor unlock InputAction.
        /// </summary>

        protected InputAction cursorUnlockInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        /// <summary>
        /// Gets the mouse look value.
        /// Return its current value or zero if no valid InputAction found.
        /// </summary>
        
        protected virtual Vector2 GetMouseLookInput()
        {
            if (mouseLookInputAction != null)
                return mouseLookInputAction.ReadValue<Vector2>();

            return Vector2.zero;
        }

        /// <summary>
        /// Gets the controller look input value.
        /// Return its current value or zero if no valid InputAction found.
        /// </summary>
        
        protected virtual Vector2 GetControllerLookInput()
        {
            if (controllerLookInputAction != null)
                return controllerLookInputAction.ReadValue<Vector2>();

            return Vector2.zero;
        }

        /// <summary>
        /// Cursor lock input action handler.
        /// </summary>

        protected virtual void OnCursorLock(InputAction.CallbackContext context)
        {
            // Do not allow to lock cursor if using UI

            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
                return;

            if (context.started)
                characterLook.LockCursor();
        }

        /// <summary>
        /// Cursor unlock input action handler.
        /// </summary>

        protected virtual void OnCursorUnlock(InputAction.CallbackContext context)
        {
            if (context.started)
                characterLook.UnlockCursor();
        }

        #endregion

        #region METHODS        

        /// <summary>
        /// Return the CharacterLook component.
        /// This is guaranteed to be not null.
        /// </summary>

        public virtual CharacterLook GetCharacterLook()
        {
            return characterLook;
        }

        /// <summary>
        /// This current right vector.
        /// </summary>

        public override Vector3 GetRightVector()
        {
            return rootPivot.right;
        }

        /// <summary>
        /// This current forward vector.
        /// </summary>

        public override Vector3 GetForwardVector()
        {
            return rootPivot.forward;
        }

        /// <summary>
        /// The Eye right vector.
        /// </summary>

        public virtual Vector3 GetEyeRightVector()
        {
            return eyePivot.right;
        }

        /// <summary>
        /// The Eye Up vector.
        /// </summary>

        public virtual Vector3 GetEyeUpVector()
        {
            return eyePivot.up;
        }

        /// <summary>
        /// The Eye forward vector.
        /// </summary>

        public virtual Vector3 GetEyeForwardVector()
        {
            return eyePivot.forward;
        }

        /// <summary>
        /// Sets the yaw value. This will reset the current pitch and roll values.
        /// </summary>
        
        public override void SetYaw(float value)
        {
            // Extends this to perform yaw on rootPivot transform

            rootPivot.localRotation = Quaternion.Euler(0.0f, value, 0.0f);
        }

        /// <summary>
        /// Extends AddYawRotation method. Adds Yaw rotation to root pivot.
        /// </summary>

        public override void AddYawInput(float value)
        {
            if (value != 0.0f)
                rootPivot.localRotation *= Quaternion.Euler(0.0f, value, 0.0f);
        }

        /// <summary>
        /// Look up / down. Adds Pitch rotation to eyePivot.
        /// </summary>

        public void AddEyePitchInput(float value)
        {
            if (value == 0.0f)
                return;

            eyePivot.localRotation *= Quaternion.Euler(characterLook.invertLook ? -value : value, 0.0f, 0.0f);

            if (characterLook.clampPitchRotation)
                eyePivot.localRotation = eyePivot.localRotation.clampPitch(characterLook.minPitchAngle, characterLook.maxPitchAngle);
        }

        /// <summary>
        /// The current speed multiplier based on movement direction,
        /// eg: walking forward, walking backwards or strafing side to side.
        /// </summary>

        protected virtual float GetSpeedMultiplier()
        {
            // Compute planar move direction

            Vector3 up = GetUpVector();
            Vector3 planarMoveDirection = Vector3.ProjectOnPlane(GetMovementDirection(), up);

            // Compute actual walk speed factoring movement direction

            Vector3 characterForward = Vector3.ProjectOnPlane(GetForwardVector(), up);

            float dot = Vector3.Dot(planarMoveDirection.normalized, characterForward.normalized);

            float speedMultiplier = dot >= 0.0f
                ? Mathf.Lerp(strafeSpeedMultiplier, forwardSpeedMultiplier, dot)
                : Mathf.Lerp(strafeSpeedMultiplier, backwardSpeedMultiplier, -dot);

            return speedMultiplier;
        }

        /// <summary>
        /// The maximum speed for current movement mode, factoring walking movement direction.
        /// </summary>

        public override float GetMaxSpeed()
        {
            return base.GetMaxSpeed() * GetSpeedMultiplier();
        }
        
        /// <summary>
        /// Setup player InputActions (if any).
        /// </summary>

        protected override void SetupPlayerInput()
        {
            // Init base character controller input actions (if any)

            base.SetupPlayerInput();

            // Attempts to cache this InputActions (if any)

            if (actions == null)
                return;

            // Setup input action handlers

            mouseLookInputAction = actions.FindAction("Mouse Look");
            controllerLookInputAction = actions.FindAction("Controller Look");

            cursorLockInputAction = actions.FindAction("Cursor Lock");
            if (cursorLockInputAction != null)
                cursorLockInputAction.started += OnCursorLock;

            cursorUnlockInputAction = actions.FindAction("Cursor Unlock");
            if (cursorUnlockInputAction != null)
                cursorUnlockInputAction.started += OnCursorUnlock;
        }

        /// <summary>
        /// Extends HandleInput method to add camera look input.
        /// </summary>

        protected override void HandleInput()
        {
            // Handle default movement input

            base.HandleInput();
            
            // Is Character is disabled, or externally controlled (eg: by a controller), halts camera look

            if (actions == null || IsDisabled())
                return;

            // Mouse Look Input

            Vector2 mouseLookInput = GetMouseLookInput();

            if (mouseLookInput.sqrMagnitude > 0.0f)
            {
                // Perform 'Look' rotation with Mouse

                if (mouseLookInput.x != 0.0f)
                    AddYawInput(mouseLookInput.x * characterLook.mouseHorizontalSensitivity);

                if (mouseLookInput.y != 0.0f)
                    AddEyePitchInput(mouseLookInput.y * characterLook.mouseVerticalSensitivity);
            }
            else
            {
                // Perform 'Look' rotation with Controller, this will perform rotation at (rotationRate) rate

                Vector2 controllerLookInput = GetControllerLookInput();

                if (controllerLookInput.x != 0.0f)
                    AddYawInput(controllerLookInput.x * characterLook.controllerHorizontalSensitivity * rotationRate * Time.deltaTime);

                if (controllerLookInput.y != 0.0f)
                    AddEyePitchInput(controllerLookInput.y * characterLook.controllerVerticalSensitivity * rotationRate * Time.deltaTime);
            }
        }

        /// <summary>
        /// Helper method used to perform camera animation.
        /// Default implementation do basic programatically crouch animation.
        /// </summary>

        protected virtual void AnimateEye()
        {
            // Modify camera's height to simulate crouching state

            float actualEyeHeight = IsCrouching() ? eyeHeightCrouched : eyeHeight;

            eyePivot.localPosition =
                Vector3.MoveTowards(eyePivot.localPosition, new Vector3(0.0f, actualEyeHeight, 0.0f), 6.0f * Time.deltaTime);
        }

        /// <summary>
        /// Set this default values.
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected override void OnReset()
        {
            // Character defaults

            base.OnReset();

            // This defaults

            eyeHeight = 1.6f;
            eyeHeightCrouched = 1.0f;

            forwardSpeedMultiplier = 1.0f;
            backwardSpeedMultiplier = 0.5f;
            strafeSpeedMultiplier = 0.75f;

            SetRotationMode(RotationMode.None);
        }

        /// <summary>
        /// Validate editor exposed fields. 
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected override void OnOnValidate()
        {
            // Validates Character fields

            base.OnOnValidate();

            // Validate this

            eyeHeight = _eyeHeight;
            eyeHeightCrouched = _eyeHeightCrouched;

            forwardSpeedMultiplier = _forwardSpeedMultiplier;
            backwardSpeedMultiplier = _backwardSpeedMultiplier;
            strafeSpeedMultiplier = _strafeSpeedMultiplier;
        }

        /// <summary>
        /// Initialize this.
        /// </summary>

        protected override void OnAwake()
        {
            // Call base method

            base.OnAwake();

            // Disable Character rotation

            SetRotationMode(RotationMode.None);
        }

        /// <summary>
        /// Called when the object becomes enabled and active (OnEnabled).
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected override void OnOnEnable()
        {
            // Init Character

            base.OnOnEnable();

            // Enable input actions (if any)
            
            mouseLookInputAction?.Enable();
            controllerLookInputAction?.Enable();
            cursorLockInputAction?.Enable();
            cursorUnlockInputAction?.Enable();
        }

        /// <summary>
        /// Called when the behaviour becomes disabled (OnDisable).
        /// If overriden, must call base method in order to fully de-initialize the class.
        /// </summary>

        protected override void OnOnDisable()
        {
            // De-Init Character

            base.OnOnDisable();

            // Disable input actions (if any)

            mouseLookInputAction?.Disable();
            controllerLookInputAction?.Disable();
            cursorLockInputAction?.Disable();
            cursorUnlockInputAction?.Disable();
        }

        /// <summary>
        /// Extends OnLateUpdate to perform programatically camera animation (eg: crouch anim).
        /// </summary>

        protected override void OnLateUpdate()
        {
            // Is Character is disabled, return

            if (IsDisabled())
                return;

            AnimateEye();
        }

        #endregion
    }
}
