using ECM2.Common;
using ECM2.Components;
using ECM2.Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Characters
{
    #region ENUMS

    /// <summary>
    /// Character's current movement mode (walking, falling, etc):
    /// 
    ///    - walking:  Walking on a surface, under the effects of friction, and able to "step up" barriers. Vertical velocity is zero.
    ///    - falling:  Falling under the effects of gravity, after jumping or walking off the edge of a surface.
    ///    - flying:   Flying, ignoring the effects of gravity.
    ///    - swimming: Swimming through a fluid volume, under the effects of gravity and buoyancy.
    ///    - custom:   User-defined custom movement mode, including many possible sub-modes.
    /// </summary>

    public enum MovementMode
    {
        None,
        Walking,
        Falling,
        Swimming,
        Flying,
        Custom
    }

    /// <summary>
    /// Character's current rotation mode:
    /// 
    ///     - None:                        Disables character's rotation.
    ///     - OrientToMovement:            Rotates the Character towards the given input move direction vector, using rotationRate as the rate of rotation change.
    ///     - OrientToCameraViewDirection: Rotates the Character towards the camera's current view direction, using rotationRate as the rate of rotation change.
    ///     - OrientWithRootMotion:        Append root motion rotation to Character's rotation.
    /// </summary>

    public enum RotationMode
    {
        None,
        OrientToMovement,
        OrientToCameraViewDirection,
        OrientWithRootMotion
    }

    #endregion

    #region CLASSES

    /// <summary>
    /// The Character class is the base class of all characters that can be controlled by players or AI,
    /// and has been designed for a vertically-oriented player representation that can walk, run, jump, fly, and swim through the world.
    ///
    /// This Character class serves as a robust platform to build your game mechanics on top of it, as this has been developed with extensibility in mind, because in the end,
    /// no one knows your game better than you!
    /// 
    /// </summary>

    public class Character : MonoBehaviour, ICharacterMovementCallbacks
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Input")]
        [Tooltip("Input actions associated with this Character\n." +
                 "If not assigned, Character can be externally controlled.")]
        [SerializeField]
        private InputActionAsset _actions;

        [Header("Rotation")]
        [Tooltip("Change in rotation per second (Deg / s).")]
        [SerializeField]
        private float _rotationRate;

        [Tooltip("Character's current rotation mode.")]
        [SerializeField]
        private RotationMode _rotationMode;        

        [Header("Walking")]
        [Tooltip("The maximum ground speed when walking.\n" +
                 "Also determines maximum lateral speed when falling.")]
        [SerializeField]
        private float _maxWalkSpeed;

        [Tooltip("The ground speed that we should accelerate up to when walking at minimum analog stick tilt.")]
        [SerializeField]
        private float _minAnalogWalkSpeed;

        [Tooltip("Max Acceleration (rate of change of velocity).")]
        [SerializeField]
        private float _maxAcceleration;

        [Tooltip("Deceleration when walking and not applying acceleration.\n" +
                 "This is a constant opposing force that directly lowers velocity by a constant value.")]
        [SerializeField]
        private float _brakingDecelerationWalking;

        [Tooltip("Setting that affects movement control.\n" +
                 "Higher values allow faster changes in direction.\n" +
                 "If useSeparateBrakingFriction is false, also affects the ability to stop more quickly when braking (whenever acceleration is zero).")]
        [SerializeField]
        private float _groundFriction;

        [Header("Sprinting")]
        [Tooltip("The walk speed multiplier while sprinting.")]
        [SerializeField]
        private float _sprintSpeedMultiplier;

        [Tooltip("The walk acceleration multiplier while sprinting.")]
        [SerializeField]
        private float _sprintAccelerationMultiplier;
        
        [Header("Crouching")]
        [Tooltip("The maximum ground speed when walking and crouched.")]
        [SerializeField]
        private float _maxWalkSpeedCrouched;

        [Tooltip("The Character's capsule height while crouched.")]
        [SerializeField]
        private float _crouchedHeight;

        [Tooltip("If true, allows to jump while crouched (uncrouch character).")]
        [SerializeField]
        private bool _crouchedJump;
        
        [Header("Falling")]
        [Tooltip("The maximum vertical velocity a Character can reach when falling. Eg: Terminal velocity.")]
        [SerializeField]
        private float _maxFallSpeed;

        [Tooltip("Lateral deceleration when falling and not applying acceleration.")]
        [SerializeField]
        private float _brakingDecelerationFalling;

        [Tooltip("Friction to apply to lateral movement when falling. \n" +
                 "If useSeparateBrakingFriction is false, also affects the ability to stop more quickly when braking (whenever acceleration is zero).")]
        [SerializeField]
        private float _fallingLateralFriction;

        [Range(0.0f, 1.0f)]
        [Tooltip("When falling, amount of lateral movement control available to the Character.\n" +
                 "0 = no control, 1 = full control at max acceleration.")]
        [SerializeField]
        private float _airControl;
        
        [Header("Jumping")]
        [Tooltip("The max number of jumps the Character can perform.")]
        [SerializeField]
        private int _jumpMaxCount;

        [Tooltip("Initial velocity (instantaneous vertical velocity) when jumping.")]
        [SerializeField]
        private float _jumpImpulse;

        [Tooltip("The maximum time (in seconds) to hold the jump. eg: Variable height jump.")]
        [SerializeField]
        private float _jumpMaxHoldTime;

        [Tooltip("How early before hitting the ground you can trigger a jump (in seconds).")]
        [SerializeField]
        private float _jumpPreGroundedTime;

        [Tooltip("How long after leaving the ground you can trigger a jump (in seconds).")]
        [SerializeField]
        private float _jumpPostGroundedTime;

        [Space(15f)]
        [Tooltip("The gravity applied to this Character.")]
        [SerializeField]
        private Vector3 _gravity;
        
        [Header("Swimming")]
        [Tooltip("If True, this Character is capable to Swim or move through fluid volumes.")]
        [SerializeField]
        private bool _canSwim;

        [Tooltip("The maximum swimming speed.")]
        [SerializeField]
        private float _maxSwimSpeed;

        [Tooltip("Deceleration when swimming and not applying acceleration.")]
        [SerializeField]
        private float _brakingDecelerationSwimming;

        [Tooltip("Friction to apply to movement when swimming.")]
        [SerializeField]
        private float _swimmingFriction;

        [Tooltip("Water buoyancy ratio. 1 = Neutral Buoyancy, 0 = No Buoyancy.")]
        [SerializeField]
        private float _buoyancy;
        
        [Header("Flying")]
        [Tooltip("The maximum flying speed.")]
        [SerializeField]
        private float _maxFlySpeed;
        
        [Tooltip("Deceleration when flying and not applying acceleration.")]
        [SerializeField]
        private float _brakingDecelerationFlying;
        
        [Tooltip("Friction to apply to movement when flying.")]
        [SerializeField]
        private float _flyingFriction;
        
        [Header("Physics")]
        [Tooltip("This Character's mass.")]
        [SerializeField]
        private float _mass;

        [Tooltip("If true, impart the external velocity caused by external forces during internal physics update.")]
        [SerializeField]
        private bool _impartExternalVelocity;

        [Tooltip("If true, impart the platform's velocity when jumping or falling off it.")]
        [SerializeField]
        private bool _impartPlatformVelocity;

        [Tooltip("Whether the Character moves with the moving platform it is standing on.")]
        [SerializeField]
        private bool _impartPlatformMovement;

        [Tooltip("Whether the Character receives the changes in rotation of the platform it is standing on.")]
        [SerializeField]
        private bool _impartPlatformRotation;

        [Tooltip("Should apply a push force to rigidbodies when walking into them ?")]
        [SerializeField]
        private bool _applyPushForce;

        [Tooltip("Should apply push force to characters when walking into them ?")]
        [SerializeField]
        private bool _pushForceAffectCharacters;

        [Tooltip("Should apply a downward force to rigidbodies we stand on ?")]
        [SerializeField]
        private bool _applyStandingDownwardForce;

        [Tooltip("Force applied to rigidbodies when walking into them (due to mass and relative velocity) is scaled by this amount.")]
        [SerializeField]
        private float _pushForceScale;

        [Tooltip("Force applied to rigidbodies we stand on (due to mass and gravity) is scaled by this amount.")]
        [SerializeField]
        private float _standingDownwardForceScale;

        [Header("Root Motion")]
        [Tooltip("Should animation determines the Character's movement ?")]
        [SerializeField]
        private bool _useRootMotion;
        
        [Header("Camera")]
        [Tooltip("Reference to the Player's Camera.\n" +
                 "If assigned, the Character's movement will be relative to this camera, otherwise movement will be relative to world axis.")]
        [SerializeField]
        private Camera _camera;

        #endregion

        #region FIELDS

        private CharacterMovement _characterMovement;

        private Animator _animator;
        private RootMotionController _rootMotionController;

        private Transform _cameraTransform;

        protected float _lastCapsuleHeight;

        private float _brakingFriction;

        protected Vector3 _pendingForces;
        protected Vector3 _pendingLaunchVelocity;

        protected float _fallingTime;

        protected bool _jumpButtonPressed;
        protected float _jumpButtonHeldDownTime;
        protected float _jumpHoldTime;
        protected int _jumpCount;
        protected bool _isJumping;

        protected bool _crouchButtonPressed;
        protected bool _isCrouched;

        protected bool _sprintButtonPressed;

        private Vector3 _movementDirection;

        protected MovementMode _movementMode = MovementMode.None;

        /// <summary>
        /// Character's User-defined custom movement mode (sub-mode).
        /// Only applicable if _movementMode == Custom.
        /// </summary>

        protected int _customMovementMode;

        #endregion

        #region PROPERTIES

        protected CharacterMovement characterMovement
        {
            get
            {
#if UNITY_EDITOR
                if (_characterMovement == null)
                    _characterMovement = GetComponent<CharacterMovement>();
#endif

                return _characterMovement;
            }
        }

        /// <summary>
        /// Cached Animator component (if any).
        /// </summary>

        protected Animator animator
        {
            get
            {
#if UNITY_EDITOR
                if (_animator == null)
                    _animator = GetComponentInChildren<Animator>();
#endif

                return _animator;
            }
        }

        /// <summary>
        /// Cached Character's RootMotionController component (if any).
        /// </summary>

        protected RootMotionController rootMotionController
        {
            get
            {
#if UNITY_EDITOR
                if (_rootMotionController == null)
                    _rootMotionController = GetComponentInChildren<RootMotionController>();
#endif

                return _rootMotionController;
            }
        }

        /// <summary>
        /// Change in rotation per second (Deg / s).
        /// </summary>
        
        public float rotationRate
        {
            get => _rotationRate;
            set => _rotationRate = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The maximum ground speed when walking. Also determines maximum lateral speed when falling.
        /// </summary>

        public float maxWalkSpeed
        {
            get => _maxWalkSpeed;
            set => _maxWalkSpeed = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The ground speed that we should accelerate up to when walking at minimum analog stick tilt.
        /// </summary>

        public float minAnalogWalkSpeed
        {
            get => _minAnalogWalkSpeed;
            set => _minAnalogWalkSpeed = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Max Acceleration (rate of change of velocity).
        /// </summary>

        public float maxAcceleration
        {
            get => _maxAcceleration;
            set => _maxAcceleration = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Deceleration when walking and not applying acceleration.
        /// This is a constant opposing force that directly lowers velocity by a constant value.
        /// </summary>

        public float brakingDecelerationWalking
        {
            get => _brakingDecelerationWalking;
            set => _brakingDecelerationWalking = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Setting that affects movement control.
        /// Higher values allow faster changes in direction.
        /// If useSeparateBrakingFriction is false, also affects the ability to stop more quickly when braking (whenever acceleration is zero).
        /// </summary>

        public float groundFriction
        {
            get => _groundFriction;
            set => _groundFriction = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Should use a separate braking friction ?
        /// </summary>

        public bool useSeparateBrakingFriction { get; set; }

        /// <summary>
        /// Friction (drag) coefficient applied when braking (whenever Acceleration = 0, or if Character is exceeding max speed).
        /// This is the value, used in all movement modes IF useSeparateBrakingFriction is True.
        /// </summary>

        public float brakingFriction
        {
            get => _brakingFriction;
            set => _brakingFriction = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The walk speed multiplier while sprinting.
        /// </summary>

        public float sprintSpeedMultiplier
        {
            get => _sprintSpeedMultiplier;
            set => _sprintSpeedMultiplier = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The walk acceleration multiplier while sprinting.
        /// </summary>

        public float sprintAccelerationMultiplier
        {
            get => _sprintAccelerationMultiplier;
            set => _sprintAccelerationMultiplier = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The maximum ground speed when walking and crouched.
        /// </summary>

        public float maxWalkSpeedCrouched
        {
            get => _maxWalkSpeedCrouched;
            set => _maxWalkSpeedCrouched = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The Character's capsule height while crouched.
        /// </summary>

        public float crouchedHeight
        {
            get => _crouchedHeight;
            set => _crouchedHeight = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// If true, allows to jump while crouched (cancels crouch).
        /// </summary>

        public bool crouchedJump
        {
            get => _crouchedJump;
            set => _crouchedJump = value;
        }

        /// <summary>
        /// The maximum vertical velocity (in m/s) a Character can reach when falling.
        /// Eg: Terminal velocity.
        /// </summary>

        public float maxFallSpeed
        {
            get => _maxFallSpeed;
            set => _maxFallSpeed = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Lateral deceleration when falling and not applying acceleration.
        /// </summary>

        public float brakingDecelerationFalling
        {
            get => _brakingDecelerationFalling;
            set => _brakingDecelerationFalling = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Friction to apply to lateral air movement when falling.
        /// </summary>

        public float fallingLateralFriction
        {
            get => _fallingLateralFriction;
            set => _fallingLateralFriction = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The Character's time in falling movement mode.
        /// </summary>

        public float fallingTime => _fallingTime;

        /// <summary>
        /// When falling, amount of lateral movement control available to the Character.
        /// 0 = no control, 1 = full control at max acceleration.
        /// </summary>

        public float airControl
        {
            get => _airControl;
            set => _airControl = Mathf.Clamp01(value);
        }

        /// <summary>
        /// The max number of jumps the Character can perform.
        /// </summary>

        public int jumpMaxCount
        {
            get => _jumpMaxCount;
            set => _jumpMaxCount = Mathf.Max(0, value);
        }

        /// <summary>
        /// Initial velocity (instantaneous vertical velocity) when jumping.
        /// </summary>

        public float jumpImpulse
        {
            get => _jumpImpulse;
            set => _jumpImpulse = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The maximum time (in seconds) to hold the jump. eg: Variable height jump.
        /// </summary>

        public float jumpMaxHoldTime
        {
            get => _jumpMaxHoldTime;
            set => _jumpMaxHoldTime = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// How early before hitting the ground you can trigger a jump (in seconds).
        /// </summary>

        public float jumpPreGroundedTime
        {
            get => _jumpPreGroundedTime;
            set => _jumpPreGroundedTime = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// How long after leaving the ground you can trigger a jump (in seconds).
        /// </summary>

        public float jumpPostGroundedTime
        {
            get => _jumpPostGroundedTime;
            set => _jumpPostGroundedTime = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The gravity applied to this Character.
        /// </summary>

        public Vector3 gravity
        {
            get => _gravity;
            set => _gravity = value;
        }

        /// <summary>
        /// True is _jumpButtonPressed is true, false otherwise.
        /// </summary>

        public bool jumpButtonPressed => _jumpButtonPressed;

        /// <summary>
        /// This is the time (in seconds) that the player has held the jump button.
        /// </summary>

        public float jumpButtonHeldDownTime => _jumpButtonHeldDownTime;

        /// <summary>
        /// Tracks the current number of jumps performed.
        /// </summary>

        public int jumpCount => _jumpCount;

        /// <summary>
        /// This is the time (in seconds) that the player has been holding the jump.
        /// Eg: Variable height jump.
        /// </summary>

        public float jumpHoldTime => _jumpHoldTime;

        /// <summary>
        /// Should notify a jump apex ?
        /// Set to true to receive OnReachedJumpApex event.
        /// </summary>

        public bool notifyJumpApex { get; set; }

        /// <summary>
        /// If True, this Character is capable to Swim or move through fluid volumes.
        /// </summary>

        public bool canSwim
        {
            get => _canSwim;
            set => _canSwim = value;
        }

        /// <summary>
        /// The maximum swimming speed.
        /// </summary>

        public float maxSwimSpeed
        {
            get => _maxSwimSpeed;
            set => _maxSwimSpeed = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Deceleration when swimming and not applying acceleration.
        /// </summary>

        public float brakingDecelerationSwimming
        {
            get => _brakingDecelerationSwimming;
            set => _brakingDecelerationSwimming = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Friction to apply to movement when swimming.
        /// </summary>

        public float swimmingFriction
        {
            get => _swimmingFriction;
            set => _swimmingFriction = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Water buoyancy ratio. 1 = Neutral Buoyancy, 0 = No Buoyancy.
        /// </summary>

        public float buoyancy
        {
            get => _buoyancy;
            set => _buoyancy = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// The maximum flying speed.
        /// </summary>

        public float maxFlySpeed
        {
            get => _maxFlySpeed;
            set => _maxFlySpeed = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Deceleration when flying and not applying acceleration.
        /// </summary>
        
        public float brakingDecelerationFlying
        {
            get => _brakingDecelerationFlying;
            set => _brakingDecelerationFlying = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Friction to apply to movement when flying.
        /// </summary>
        
        public float flyingFriction
        {
            get => _flyingFriction;
            set => _flyingFriction = Mathf.Max(0.0f, value);
        }
        
        /// <summary>
        /// This Character's mass (in Kg).
        /// </summary>

        public float mass
        {
            get => _mass;
            set
            {
                _mass = Mathf.Max(1e-07f, value);
                characterMovement.rigidbody.mass = _mass;
            }
        }

        /// <summary>
        /// Should apply a push force to rigidbodies when walking into them ?
        /// </summary>

        public bool applyPushForce
        {
            get => _applyPushForce;
            set => _applyPushForce = value;
        }

        /// <summary>
        /// Should apply push force to characters when walking into them ?
        /// </summary>

        public bool pushForceAffectCharacters
        {
            get => _pushForceAffectCharacters;
            set => _pushForceAffectCharacters = value;
        }

        /// <summary>
        /// Should apply a downward force to rigidbodies we stand on ?
        /// </summary>

        public bool applyStandingDownwardForce
        {
            get => _applyStandingDownwardForce;
            set => _applyStandingDownwardForce = value;
        }

        /// <summary>
        /// Force applied to rigidbodies when walking into them (due to mass and relative velocity) is scaled by this amount.
        /// </summary>

        public float pushForceScale
        {
            get => _pushForceScale;
            set => _pushForceScale = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Force applied to rigidbodies we stand on (due to mass and gravity) is scaled by this amount.
        /// </summary>

        public float standingDownwardForceScale
        {
            get => _standingDownwardForceScale;
            set => _standingDownwardForceScale = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// If true, impart the external velocity caused by external forces during internal physics update.
        /// </summary>

        public bool impartExternalVelocity
        {
            get => _impartExternalVelocity;
            set => _impartExternalVelocity = value;
        }

        /// <summary>
        /// If true, impart the platform's velocity when jumping or falling off it.
        /// </summary>

        public bool impartPlatformVelocity
        {
            get => _impartPlatformVelocity;
            set => _impartPlatformVelocity = value;
        }

        /// <summary>
        /// Whether the Character moves with the moving platform it is standing on.
        /// If true, the Character moves with the moving platform.
        /// </summary>

        public bool impartPlatformMovement
        {
            get => _impartPlatformMovement;
            set => _impartPlatformMovement = value;
        }

        /// <summary>
        /// Whether the Character receives the changes in rotation of the platform it is standing on.
        /// If true, the Character rotates with the moving platform.
        /// </summary>

        public bool impartPlatformRotation
        {
            get => _impartPlatformRotation;
            set => _impartPlatformRotation = value;
        }

        /// <summary>
        /// Should animation determines the Character' movement ?
        /// </summary>

        public bool useRootMotion
        {
            get => _useRootMotion;
            set => _useRootMotion = value;
        }

        /// <summary>
        /// This Character's camera transform.
        /// If assigned, the Character's movement will be relative to this, otherwise movement will be relative to world.
        /// </summary>

        public new Camera camera
        {
            get => _camera;
            set => _camera = value;
        }

        /// <summary>
        /// Cached camera transform (if any).
        /// </summary>

        public Transform cameraTransform
        {
            get
            {
                if (_camera != null)
                    _cameraTransform = _camera.transform;

                return _cameraTransform;
            }
        }

        #endregion

        #region INPUT ACTIONS

        /// <summary>
        /// InputActions assets.
        /// </summary>

        public InputActionAsset actions
        {
            get => _actions;
            set => _actions = value;
        }

        /// <summary>
        /// Movement InputAction.
        /// </summary>

        protected InputAction movementInputAction { get; set; }

        /// <summary>
        /// Jump InputAction.
        /// </summary>

        protected InputAction jumpInputAction { get; set; }

        /// <summary>
        /// Crouch InputAction.
        /// </summary>

        protected InputAction crouchInputAction { get; set; }

        /// <summary>
        /// Sprint InputAction.
        /// </summary>

        protected InputAction sprintInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        /// <summary>
        /// Polls movement InputAction (if any).
        /// Return its current value or zero if no valid InputAction found.
        /// </summary>

        protected virtual Vector2 GetMovementInput()
        {
            if (movementInputAction != null)
                return movementInputAction.ReadValue<Vector2>();

            return Vector2.zero;
        }
        
        /// <summary>
        /// Jump input action handler.
        /// </summary>

        protected virtual void OnJump(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                Jump();
            else if (context.canceled)
                StopJumping();
        }

        /// <summary>
        /// Crouch input action handler.
        /// </summary>

        protected virtual void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                Crouch();
            else if (context.canceled)
                StopCrouching();
        }

        /// <summary>
        /// Sprint input action handler.
        /// </summary>

        protected virtual void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                Sprint();
            else if (context.canceled)
                StopSprinting();
        }

        #endregion

        #region EVENTS

        // Event Handlers

        public delegate void GroundHitEventHandler(ref GroundHit prevGroundHitResult, ref GroundHit groundHitResult);
        public delegate void MovementHitEventHandler(ref MovementHit movementHitResult);

        public delegate void LandedEventHandler();
        public delegate void WillLandEventHandler();

        public delegate void JumpedEventHandler();
        public delegate void ReachedJumpApexEventHandler();

        public delegate void LaunchedEventHandler(Vector3 launchVelocity, bool overrideVerticalVelocity, bool overrideLateralVelocity);

        public delegate void CrouchedEventHandler();
        public delegate void UncrouchedEventHandler();

        public delegate void PhysicsVolumeChangedEventHandler(PhysicsVolume newVolume);

        public delegate void MovementModeChangedEventHandler(MovementMode prevMovementMode, int prevCustomMode);

        /// <summary>
        /// Triggered when the Character hits a 'ground' collider.
        /// </summary>

        public event GroundHitEventHandler GroundHit;

        /// <summary>
        /// Triggered when the Character hits a collider when walking into them.
        /// </summary>

        public event MovementHitEventHandler MovementHit;

        /// <summary>
        /// Triggered when the Character was not on walkable ground (last frame) and hits walkable ground.
        /// </summary>
        
        public event LandedEventHandler Landed;

        /// <summary>
        /// Triggered when the Character will hit walkable ground.
        /// </summary>
        
        public event WillLandEventHandler WillLand;

        /// <summary>
        /// Triggered when a jump has been successfully triggered and Character is about to leave ground.
        /// </summary>

        public event JumpedEventHandler Jumped;

        /// <summary>
        /// Triggered when Character reaches jump apex (eg: change in vertical speed from positive to negative).
        /// Only triggered if notifyJumpApex == true.
        /// </summary>

        public event ReachedJumpApexEventHandler ReachedJumpApex;

        /// <summary>
        /// Triggered when Character has been launched.
        /// </summary>

        public event LaunchedEventHandler Launched;

        /// <summary>
        /// Triggered when Character crouches.
        /// </summary>

        public event CrouchedEventHandler Crouched;

        /// <summary>
        /// Triggered when Character stops crouching.
        /// </summary>

        public event UncrouchedEventHandler Uncrouched;

        /// <summary>
        /// Triggered when Character enters a new physics volume.
        /// </summary>

        public event PhysicsVolumeChangedEventHandler PhysicsVolumeChanged;

        /// <summary>
        /// Triggered after MovementMode has changed.
        /// </summary>

        public event MovementModeChangedEventHandler MovementModeChanged;

        /// <summary>
        /// Called when the Character hits a 'ground' collider while performing a Move.
        /// Receives the previous and current GroundHitResult.
        /// </summary>

        protected virtual void OnGroundHit(ref GroundHit prevGroundHit, ref GroundHit groundHit)
        {
            // Trigger ground hit event

            GroundHit?.Invoke(ref prevGroundHit, ref groundHit);

            // Has Landed eg: Character was not on walkable ground last frame, but is on walkable ground,
            // Trigger Landed event

            bool hasLanded = !prevGroundHit.hitWalkableGround && groundHit.hitWalkableGround;
            if (hasLanded)
                OnLanded();
        }

        /// <summary>
        /// Called when the Character hits a collider while performing a Move (Can be called multiple times during frame).
        /// Receives the MovementHitResult.
        /// </summary>

        protected virtual void OnMovementHit(ref MovementHit movementHit)
        {
            // Trigger MovementHit event

            MovementHit?.Invoke(ref movementHit);

            // Will land, eg: Character not on walkable ground, but movement hit walkable ground

            bool willLand = !characterMovement.isOnWalkableGround && movementHit.hitWalkableGround;
            if (willLand)
                OnWillLand();
        }

        /// <summary>
        /// Called when the Character was not on walkable ground (last frame) and hits walkable ground.
        /// </summary>
        
        protected virtual void OnLanded()
        {
            // Trigger landed event

            Landed?.Invoke();
        }

        /// <summary>
        /// Called when the Character will hit walkable ground.
        /// </summary>

        protected virtual void OnWillLand()
        {
            // Trigger WillBeLanding event

            WillLand?.Invoke();
        }

        /// <summary>
        /// Called when a jump has been successfully triggered.
        /// </summary>

        protected virtual void OnJumped()
        {
            // Trigger Jumped event

            Jumped?.Invoke();
        }

        /// <summary>
        /// Called when Character reaches jump apex (eg: change in vertical speed from positive to negative).
        /// Only triggered if notifyJumpApex == true.
        /// </summary>

        protected virtual void OnReachedJumpApex()
        {
            // Trigger ReachedJumpApex event

            ReachedJumpApex?.Invoke();
        }

        /// <summary>
        /// Called when Character has been launched.
        /// Receives the given launch velocity and if velocity was overriden in any of its components.
        /// </summary>

        protected virtual void OnLaunched(Vector3 launchVelocity, bool overrideVerticalVelocity, bool overrideLateralVelocity)
        {
            // Trigger Launched event

            Launched?.Invoke(launchVelocity, overrideVerticalVelocity, overrideLateralVelocity);
        }

        /// <summary>
        /// Called when Character crouches.
        /// </summary>

        protected virtual void OnCrouched()
        {
            // Trigger crouched event

            Crouched?.Invoke();
        }

        /// <summary>
        /// Called when Character stops crouching.
        /// </summary>

        protected virtual void OnUncrouched()
        {
            // Trigger uncrouched event

            Uncrouched?.Invoke();
        }

        /// <summary>
        /// Delegate called when the physics volume has changed. newVolume can be null (eg: left water volume).
        /// Will change to Swimming mode if newVolume == waterVolume.
        /// Trigger PhysicsVolumeChanged event.
        /// </summary>

        protected virtual void OnPhysicsVolumeChanged(PhysicsVolume newPhysicsVolume)
        {
            if (newPhysicsVolume && newPhysicsVolume.waterVolume)
            {
                // Entering a water volume

                Swim();
            }
            else if (IsInWater() && newPhysicsVolume == null)
            {
                // Left a water volume

                StopSwimming();
            }

            // Trigger PhysicsVolumeChanged event

            PhysicsVolumeChanged?.Invoke(newPhysicsVolume);
        }

        /// <summary>
        /// Called after MovementMode has changed.
        /// Does special handling for starting certain modes, eg: enable / disable ground constraint, etc.
        /// If overriden, must call base.OnMovementModeChanged.
        /// </summary>

        protected virtual void OnMovementModeChanged(MovementMode prevMovementMode, int prevCustomMode)
        {
            // If movement was disabled, re-enable it

            if (prevMovementMode == MovementMode.None)
                characterMovement.Pause(false);

            // Perform additional tasks on mode change

            switch (_movementMode)
            {
                case MovementMode.None:
                {
                    // Entering None mode...

                    // Disable Character's movement

                    ClearAccumulatedForces();

                    characterMovement.Pause(true);

                    break;
                }

                case MovementMode.Walking:
                {
                    // Entering Walking mode...

                    // Reset jump count and clear apex notification flag

                    _jumpCount = 0;
                    notifyJumpApex = false;

                    // If was flying or swimming, enable ground constraint

                    if (prevMovementMode == MovementMode.Flying || prevMovementMode == MovementMode.Swimming)
                        characterMovement.ConstrainToGround(true);

                    break;
                }

                case MovementMode.Falling:
                {
                    // Entering Falling mode...

                    // If was flying or swimming, enable ground constraint as it could lands on walkable ground

                    if (prevMovementMode == MovementMode.Flying || prevMovementMode == MovementMode.Swimming)
                        characterMovement.ConstrainToGround(true);

                    break;
                }

                case MovementMode.Swimming:
                {
                    // Entering Swimming mode...

                    // Stop the Character from holding jump.

                    StopJumping();

                    // Disable ground constraint

                    characterMovement.ConstrainToGround(false);

                    break;
                }

                case MovementMode.Flying:
                {
                    // Entering Flying mode...

                    // Stop the Character from holding jump.

                    StopJumping();

                    // Disable ground constraint

                    characterMovement.ConstrainToGround(false);

                    break;
                }
            }

            // Left Falling mode, reset falling timer

            if (!IsFalling())
                _fallingTime = 0.0f;

            // Attempts to uncrouch if not walking

            if (!IsWalking() && IsCrouching())
                StopCrouching();

            // Trigger movement mode changed event

            MovementModeChanged?.Invoke(prevMovementMode, prevCustomMode);
        }
        
        #endregion

        #region CALLBACKS

        void ICharacterMovementCallbacks.OnGroundHit(ref GroundHit prevGroundHit, ref GroundHit groundHit)
        {
            OnGroundHit(ref prevGroundHit, ref groundHit);
        }

        void ICharacterMovementCallbacks.OnMovementHit(ref MovementHit movementHitResult)
        {
            OnMovementHit(ref movementHitResult);
        }
        
        void ICharacterMovementCallbacks.OnMove()
        {
            OnMove();
        }

        Vector3 ICharacterMovementCallbacks.ComputeCollisionResponseDisplacement(Vector3 displacement,
            ref MovementHit movementHit)
        {
            return ComputeCollisionResponseDisplacement(displacement, ref movementHit);
        }

        bool ICharacterMovementCallbacks.CanStepUp(ref RaycastHit hitResult)
        {
            return CanStepUp(ref hitResult);
        }

        void ICharacterMovementCallbacks.OnApplyStandingDownwardForce(Rigidbody otherRigidbody)
        {
            OnApplyStandingDownwardForce(otherRigidbody);
        }

        void ICharacterMovementCallbacks.OnApplyPushForce(ref RigidbodyHit rigidbodyHit)
        {
            OnApplyPushForce(ref rigidbodyHit);
        }

        bool ICharacterMovementCallbacks.ShouldMoveCharacterWhenStandingOn(Rigidbody otherRigidbody)
        {
            return ShouldMoveCharacterWhenStandingOn(otherRigidbody);
        }

        bool ICharacterMovementCallbacks.ShouldRotateCharacterWhenStandingOn(Rigidbody otherRigidbody)
        {
            return ShouldRotateCharacterWhenStandingOn(otherRigidbody);
        }

        void ICharacterMovementCallbacks.ImpartPlatformVelocity(Vector3 platformVelocity)
        {
            ImpartPlatformVelocity(platformVelocity);
        }

        void ICharacterMovementCallbacks.ImpartExternalVelocity(Vector3 externalVelocity)
        {
            ImpartExternalVelocity(externalVelocity);
        }

        void ICharacterMovementCallbacks.OnPhysicsVolumeChanged(PhysicsVolume newPhysicsVolume)
        {
            OnPhysicsVolumeChanged(newPhysicsVolume);
        }

        #endregion

        #region METHODS        

        /// <summary>
        /// Return the CharacterMovement component. This is guaranteed to be not null.
        /// </summary>

        public virtual CharacterMovement GetCharacterMovement()
        {
            return characterMovement;
        }

        /// <summary>
        /// Return the Animator component or null is not found.
        /// </summary>

        public virtual Animator GetAnimator()
        {
            return animator;
        }

        /// <summary>
        /// Return the RootMotionController or null is not found.
        /// </summary>

        public virtual RootMotionController GetRootMotionController()
        {
            return rootMotionController;
        }

        /// <summary>
        /// Return the Character's current PhysicsVolume, null if none.
        /// </summary>

        public virtual PhysicsVolume GetPhysicsVolume()
        {
            return characterMovement.physicsVolume;
        }

        /// <summary>
        /// The Character's current position.
        /// </summary>

        public virtual Vector3 GetPosition()
        {
            return characterMovement.position;
        }

        /// <summary>
        /// Sets the Character's position.
        /// This complies with the interpolation resulting in a smooth transition between the two positions in any intermediate frames rendered.
        /// </summary>

        public virtual void SetPosition(Vector3 position)
        {
            characterMovement.position = position;
        }

        /// <summary>
        /// Sets the Character's position.
        /// Unlike SetPosition, this cause an instant change in position without any interpolation.
        /// </summary>        
        
        public virtual void TeleportPosition(Vector3 position)
        {
            transform.position = position;
        }

        /// <summary>
        /// The Character's current rotation.
        /// </summary>

        public virtual Quaternion GetRotation()
        {
            return characterMovement.rotation;
        }

        /// <summary>
        /// Sets the Character's current rotation.
        /// This complies with the interpolation resulting in a smooth transition between the two rotations in any intermediate frames rendered.
        /// </summary>

        public virtual void SetRotation(Quaternion rotation)
        {
            characterMovement.rotation = rotation;
        }

        /// <summary>
        /// Sets the Character's rotation.
        /// Unlike SetRotation, this cause an instant change in rotation without any interpolation.
        /// </summary>
        
        public virtual void TeleportRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        /// <summary>
        /// Sets the Character's current rotation mode:
        ///     - None:                        Disables character's rotation.
        ///     - OrientToMovement:            Orient the Character towards the given input move direction vector, using rotationRate as the rate of rotation change.
        ///     - OrientToCameraViewDirection: Rotates the character towards the camera's current view direction (eg: forward vector), using rotationRate as the rate of rotation change.
        ///     OrientWithRootMotion:          Append root motion rotation to Character's rotation.
        /// </summary>

        public virtual void SetRotationMode(RotationMode rotationMode)
        {
            _rotationMode = rotationMode;
        }

        /// <summary>
        /// Returns the Character's current rotation mode.
        /// </summary>

        public virtual RotationMode GetRotationMode()
        {
            return _rotationMode;
        }

        /// <summary>
        /// The Character's current up vector.
        /// NOTE: This can be different from this.transform.up.
        /// </summary>

        public virtual Vector3 GetUpVector()
        {
            return characterMovement.rotation * Vector3.up;
        }

        /// <summary>
        /// The Character's current right vector.
        /// NOTE: This can be different from this.transform.right.
        /// </summary>

        public virtual Vector3 GetRightVector()
        {
            return characterMovement.rotation * Vector3.right;
        }

        /// <summary>
        /// The Character's current forward vector.
        /// NOTE: This can be different from this.transform.forward.
        /// </summary>

        public virtual Vector3 GetForwardVector()
        {
            return characterMovement.rotation * Vector3.forward;
        }

        /// <summary>
        /// The Character's current velocity.
        /// </summary>

        public virtual Vector3 GetVelocity()
        {
            return characterMovement.velocity;
        }

        /// <summary>
        /// Update the Character's velocity.
        /// </summary>

        public virtual void SetVelocity(Vector3 velocity)
        {
            characterMovement.velocity = velocity;
        }

        /// <summary>
        /// Returns the Character's animation velocity if using root motion, otherwise returns Character's current velocity.
        /// </summary>

        public virtual Vector3 GetRootMotionVelocity()
        {
            if (useRootMotion && rootMotionController)
                return rootMotionController.animRootMotionVelocity;

            return characterMovement.velocity;
        }

        /// <summary>
        /// The Character's current speed.
        /// </summary>

        public virtual float GetSpeed()
        {
            return characterMovement.velocity.magnitude;
        }

        /// <summary>
        /// The Character's current movement mode.
        /// </summary>

        public virtual MovementMode GetMovementMode()
        {
            return _movementMode;
        }

        /// <summary>
        /// Change movement mode.
        /// The new custom sub-mode (newCustomMode), is only applicable if newMovementMode == Custom.
        ///
        /// Trigger OnMovementModeChanged event.
        /// </summary>

        public virtual void SetMovementMode(MovementMode newMovementMode, int newCustomMode = 0)
        {
            // Do nothing if nothing is changing

            if (newMovementMode == _movementMode)
            {
                // Allow changes in custom sub-modes

                if (newMovementMode != MovementMode.Custom || newCustomMode == _customMovementMode)
                    return;
            }

            // Performs movement mode change

            MovementMode prevMovementMode = _movementMode;
            int prevCustomMode = _customMovementMode;

            _movementMode = newMovementMode;
            _customMovementMode = newCustomMode;

            OnMovementModeChanged(prevMovementMode, prevCustomMode);
        }

        /// <summary>
        /// Was the Character on ground the last frame ?
        /// </summary>

        public virtual bool WasOnGround()
        {
            return characterMovement.wasOnGround;
        }

        /// <summary>
        /// Was the Character on walkable ground the last frame ?
        /// </summary>

        public virtual bool WasOnWalkableGround()
        {
            return characterMovement.wasOnWalkableGround;
        }

        /// <summary>
        /// Is the Character on ground ?
        /// </summary>

        public virtual bool IsOnGround()
        {
            return characterMovement.isOnGround;
        }

        /// <summary>
        /// Is the Character on walkable ground ?
        /// </summary>

        public virtual bool IsOnWalkableGround()
        {
            return characterMovement.isOnWalkableGround;
        }

        /// <summary>
        /// Returns true if the Character's movement mode is None (eg: is disabled).
        /// </summary>

        public virtual bool IsDisabled()
        {
            return _movementMode == MovementMode.None;
        }

        /// <summary>
        /// Returns true if the Character is in the 'Walking' movement mode (eg: on walkable ground).
        /// </summary>

        public virtual bool IsWalking()
        {
            return _movementMode == MovementMode.Walking;
        }

        /// <summary>
        /// Returns true if currently falling, eg: on air (not flying) or in not walkable ground.
        /// </summary>

        public virtual bool IsFalling()
        {
            return _movementMode == MovementMode.Falling;
        }

        /// <summary>
        /// Returns true if currently swimming (moving through a water volume).
        /// </summary>

        public virtual bool IsSwimming()
        {
            return _movementMode == MovementMode.Swimming;
        }

        /// <summary>
        /// Returns true if currently flying (moving through a non-water volume without resting on the ground).
        /// </summary>

        public virtual bool IsFlying()
        {
            return _movementMode == MovementMode.Flying;
        }

        /// <summary>
        /// Returns true if character is actually crouching.
        /// </summary>

        public virtual bool IsCrouching()
        {
            return _isCrouched;
        }

        /// <summary>
        /// Is the Character jumping ?
        /// </summary>

        public virtual bool IsJumping()
        {
            return _isJumping;
        }

        /// <summary>
        /// Returns the current jump count.
        /// </summary>

        public virtual int GetJumpCount()
        {
            return _jumpCount;
        }

        /// <summary>
        /// The maximum speed for current movement mode (accounting crouching / sprinting state).
        /// </summary>
        
        public virtual float GetMaxSpeed()
        {
            switch (_movementMode)
            {
                case MovementMode.Walking:
                {
                    if (IsCrouching())
                        return maxWalkSpeedCrouched;

                    return IsSprinting() ? maxWalkSpeed * sprintSpeedMultiplier : maxWalkSpeed;
                }

                case MovementMode.Falling:
                    return maxWalkSpeed;

                case MovementMode.Swimming:
                    return maxSwimSpeed;

                case MovementMode.Flying:
                    return maxFlySpeed;

                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// The ground speed that we should accelerate up to when walking at minimum analog stick tilt.
        /// </summary>

        public virtual float GetMinAnalogSpeed()
        {
            switch (_movementMode)
            {
                case MovementMode.Walking:
                case MovementMode.Falling:
                    return minAnalogWalkSpeed;

                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// The acceleration for current movement mode.
        /// </summary>
        
        public virtual float GetMaxAcceleration()
        {
            if (IsFalling())
                return maxAcceleration * airControl;

            return IsSprinting() ? maxAcceleration * sprintAccelerationMultiplier : maxAcceleration;
        }

        /// <summary>
        /// The braking deceleration for current movement mode.
        /// </summary>
        
        public virtual float GetBrakingDeceleration()
        {
            switch (_movementMode)
            {
                case MovementMode.Walking:
                    return brakingDecelerationWalking;

                case MovementMode.Falling:
                {
                    // Falling,
                    // BUT ON non-walkable ground, bypass braking deceleration to force slide off

                    return characterMovement.isOnGround ? 0.0f : brakingDecelerationFalling;
                }

                case MovementMode.Swimming:
                    return brakingDecelerationSwimming;

                case MovementMode.Flying:
                    return brakingDecelerationFlying;

                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// The current gravity vector (in world space).
        /// </summary>
        
        public virtual Vector3 GetGravityVector()
        {
            return gravity;
        }
        
        /// <summary>
        /// Computes the analog input modifier (0.0f to 1.0f) based on current input vector and desired velocity.
        /// </summary>

        protected virtual float ComputeAnalogInputModifier(Vector3 desiredVelocity)
        {
            float actualMaxSpeed = GetMaxSpeed();

            if (actualMaxSpeed > 0.0f && desiredVelocity.sqrMagnitude > 0.0f)
                return Mathf.Clamp01(desiredVelocity.magnitude / actualMaxSpeed);

            return 0.0f;
        }

        /// <summary>
        /// Apply friction and braking deceleration to given velocity.
        /// Returns modified input velocity.
        /// </summary>

        protected virtual Vector3 ApplyVelocityBraking(Vector3 velocity, float friction, float deceleration)
        {
            // If no friction or no deceleration, return

            bool isZeroFriction = friction == 0.0f;
            bool isZeroBraking = deceleration == 0.0f;

            if (isZeroFriction && isZeroBraking)
                return velocity;
            
            // Decelerate to brake to a stop

            Vector3 oldVel = velocity;
            Vector3 revAcceleration = isZeroBraking ? Vector3.zero : -deceleration * velocity.normalized;

            // Apply friction and braking

            velocity += (-friction * velocity + revAcceleration) * Time.deltaTime;

            // Don't reverse direction

            if (Vector3.Dot(velocity, oldVel) <= 0.0f)
                return Vector3.zero;

            // Clamp to zero if nearly zero, or if below min threshold and braking

            float sqrSpeed = velocity.sqrMagnitude;
            if (sqrSpeed <= 0.00001f || !isZeroBraking && sqrSpeed <= MathLib.Square(0.1f))
                return Vector3.zero;

            return velocity;
        }

        /// <summary>
        /// Calculates a new velocity for the given state, applying the effects of friction or braking friction and acceleration or deceleration.
        /// Calls GetMaxSpeed(), GetMaxAcceleration(), GetBrakingDeceleration().
        /// </summary>

        protected virtual Vector3 CalcVelocity(Vector3 velocity, Vector3 desiredVelocity, float friction, bool isFluid = false)
        {
            // Compute requested move direction

            float desiredSpeed = desiredVelocity.magnitude;
            Vector3 desiredMoveDirection = desiredSpeed > 0.0f ? desiredVelocity / desiredSpeed : Vector3.zero;

            // Requested acceleration (factoring analog input)

            float analogInputModifier = ComputeAnalogInputModifier(desiredVelocity);
            Vector3 requestedAcceleration = GetMaxAcceleration() * analogInputModifier * desiredMoveDirection;

            // Actual max speed (factoring analog input)

            float actualMaxSpeed = Mathf.Max(GetMinAnalogSpeed(), GetMaxSpeed() * analogInputModifier);

            // Friction
            // Only apply braking if there is no input acceleration,
            // or we are over our max speed and need to slow down to it

            bool isZeroAcceleration = requestedAcceleration.isZero();
            bool isVelocityOverMax = velocity.isExceeding(actualMaxSpeed);

            if (isZeroAcceleration || isVelocityOverMax)
            {
                // Pre-braking velocity

                Vector3 oldVelocity = velocity;

                // Apply friction and braking

                float actualBrakingFriction = useSeparateBrakingFriction ? brakingFriction : friction;
                velocity = ApplyVelocityBraking(velocity, actualBrakingFriction, GetBrakingDeceleration());

                // Don't allow braking to lower us below max speed if we started above it

                if (isVelocityOverMax && velocity.sqrMagnitude < MathLib.Square(actualMaxSpeed) &&
                    Vector3.Dot(requestedAcceleration, oldVelocity) > 0.0f)
                    velocity = oldVelocity.normalized * actualMaxSpeed;
            }
            else
            {
                // Friction, this affects our ability to change direction

                velocity -= (velocity - desiredMoveDirection * velocity.magnitude) * Mathf.Min(friction * Time.deltaTime, 1.0f);
            }

            // Apply fluid friction

            if (isFluid)
                velocity *= 1.0f - Mathf.Min(friction * Time.deltaTime, 1.0f);

            // Apply acceleration

            if (!isZeroAcceleration)
            {
                float newMaxSpeed = velocity.isExceeding(actualMaxSpeed) ? velocity.magnitude : actualMaxSpeed;

                velocity += requestedAcceleration * Time.deltaTime;
                velocity = velocity.clampedTo(newMaxSpeed);
            }

            // Return new velocity

            return velocity;
        }

        /// <summary>
        /// Determines the Character's movement when moving on walkable ground.
        /// Ignores desiredVelocity vertical component.
        /// </summary>

        protected virtual void Walking(Vector3 desiredVelocity)
        {
            // Discards desiredVelocity vertical component

            Vector3 characterUp = GetUpVector();
            Vector3 desiredLateralVelocity = desiredVelocity.projectedOnPlane(characterUp);

            // Reorient desired velocity along walkable surface

            Vector3 groundNormal = characterMovement.groundHit.normal;
            desiredLateralVelocity = desiredLateralVelocity.tangentTo(groundNormal, characterUp);

            if (useRootMotion)
                characterMovement.velocity = desiredLateralVelocity;
            else
            {
                // Current movement velocity

                Vector3 velocity = characterMovement.velocity;

                // Reorient velocity along walkable surface

                velocity = velocity.tangentTo(groundNormal, characterUp);

                // Calculate new velocity
            
                characterMovement.velocity = CalcVelocity(velocity, desiredLateralVelocity, groundFriction);
            }
        }

        /// <summary>
        /// Clamps the current falling speed to maxFallSpeed or if within a PhysicsVolume, PhysicsVolume.maxFallSpeed
        /// </summary>

        protected virtual void LimitFallingSpeed()
        {
            // Defaults to our maxFallSpeed value

            float actualTerminalVelocity = maxFallSpeed;

            // If within a physics volume, use its maxFallSpeed

            if (characterMovement.physicsVolume)
                actualTerminalVelocity = characterMovement.physicsVolume.maxFallSpeed;

            // Should limit falling speed ?

            if (characterMovement.velocity.sqrMagnitude <= MathLib.Square(actualTerminalVelocity))
                return;

            // Apply speed limit (velocity component along gravity direction)

            Vector3 gravityDirection = GetGravityVector().normalized;
                
            if (Vector3.Dot(characterMovement.velocity, gravityDirection) > actualTerminalVelocity)
            {
                characterMovement.velocity = characterMovement.velocity.projectedOnPlane(gravityDirection) +
                                             gravityDirection * actualTerminalVelocity;
            }
        }

        /// <summary>
        /// Determines the Character's movement when falling on air or sliding off non-walkable ground.
        /// Do not override Character's velocity vertical component (ignores desiredVelocity vertical component) as we want to keep the effect of gravity.
        /// Apply gravity acceleration.
        /// </summary>

        protected virtual void Falling(Vector3 desiredVelocity)
        {
            // Do not override vertical component as we want to keep the effect of gravity

            Vector3 characterUp = GetUpVector();
            Vector3 desiredLateralVelocity = desiredVelocity.projectedOnPlane(characterUp);

            // On Non-walkable ground...

            if (characterMovement.isOnGround)
            {
                // Cancel any movement against it in order to prevent climb it

                Vector3 groundNormal =
                    characterUp.perpendicularTo(characterMovement.groundHit.normal).perpendicularTo(characterUp);
                
                if (Vector3.Dot(desiredLateralVelocity, groundNormal) < 0.0f)
                    desiredLateralVelocity = desiredLateralVelocity.projectedOnPlane(groundNormal);

                // If using root motion, preserve lateral velocity to slide off non-walkable surface

                if (useRootMotion)
                    desiredLateralVelocity += characterMovement.velocity.projectedOn(groundNormal);
            }

            // Using root motion...

            if (useRootMotion)
            {
                // Preserve current vertical velocity as we want to keep the effect of gravity

                Vector3 verticalVelocity = characterMovement.velocity.projectedOn(characterUp);

                // Update Character's velocity

                characterMovement.velocity = desiredLateralVelocity + verticalVelocity;
            }
            else
            {
                // Preserve current vertical velocity as we want to keep the effect of gravity

                Vector3 velocity = characterMovement.velocity;

                Vector3 verticalVelocity = velocity.projectedOn(characterUp);
                Vector3 lateralVelocity = velocity - verticalVelocity;

                lateralVelocity = CalcVelocity(lateralVelocity, desiredLateralVelocity, fallingLateralFriction);

                // Update Character's velocity

                characterMovement.velocity = lateralVelocity + verticalVelocity;
            }

            // Apply gravity

            Vector3 actualGravity = GetGravityVector();

            characterMovement.velocity += actualGravity * Time.deltaTime;

            // Limit maximum falling velocity

            LimitFallingSpeed();
        }

        /// <summary>
        /// Is the character in a water physics volume ?
        /// </summary>
        
        public virtual bool IsInWater()
        {
            return characterMovement.physicsVolume && characterMovement.physicsVolume.waterVolume;
        }

        /// <summary>
        /// Attempts to enter Swimming mode.
        /// Called when Character enters a water physics volume.
        /// </summary>
        
        public virtual void Swim()
        {
            // Is the Character able to swim ?

            if (!canSwim)
                return;

            // Already swimming ?

            if (IsSwimming())
                return;

            // Change to Swimming mode

            SetMovementMode(MovementMode.Swimming);
        }

        /// <summary>
        /// Exits swimming mode.
        /// Called when Character leaves a water physics volume.
        /// </summary>
        
        public virtual void StopSwimming()
        {
            // If Swimming, change to Falling mode

            if (IsSwimming())
                SetMovementMode(MovementMode.Falling);
        }

        /// <summary>
        /// How deep in water the character is immersed.
        /// Returns a float in range 0.0 = not in water, 1.0 = fully immersed.
        /// </summary>
        
        public virtual float ImmersionDepth()
        {
            if (!IsInWater())
                return 0.0f;

            Vector3 characterUp = GetUpVector();

            Vector3 rayOrigin = GetPosition() + characterMovement.capsuleHeight * characterUp;
            Vector3 rayDirection = -characterUp;

            float rayLength = characterMovement.capsuleHeight;

            BoxCollider waterVolumeCollider = characterMovement.physicsVolume.boxCollider;
            if (waterVolumeCollider.Raycast(new Ray(rayOrigin, rayDirection), out RaycastHit hitInfo, rayLength))
                return 1.0f - Mathf.InverseLerp(0.0f, rayLength, hitInfo.distance);

            return 1.0f;
        }

        /// <summary>
        /// Determines the Character's movement when Swimming through a fluid volume, under the effects of gravity and buoyancy.
        /// Ground-Unconstrained movement with full desiredVelocity (lateral AND vertical) applies gravity but scaled by (1.0f - buoyancy).
        /// </summary>
        
        protected virtual void Swimming(Vector3 desiredVelocity)
        {
            // Compute actual buoyancy factoring current immersion depth

            float depth = ImmersionDepth();
            float actualBuoyancy = buoyancy * depth;

            // Calculate new velocity

            Vector3 velocity = characterMovement.velocity;

            Vector3 characterUp = GetUpVector();
            float verticalSpeed = Vector3.Dot(velocity, characterUp);

            if (verticalSpeed > maxSwimSpeed * 0.33f && actualBuoyancy > 0.0f)
            {
                // Damp positive vertical speed (out of water)

                verticalSpeed = Mathf.Max(maxSwimSpeed * 0.33f, verticalSpeed * depth * depth);

                velocity = velocity.projectedOnPlane(characterUp) + characterUp * verticalSpeed;
            }
            else if (depth < 0.65f)
            {
                // Cancel vertical movement (to out of water)

                desiredVelocity = desiredVelocity.projectedOnPlane(characterUp);
            }

            // Using root motion...

            if (useRootMotion)
            {
                // Preserve current vertical velocity as we want to keep the effect of gravity

                Vector3 verticalVelocity = velocity.projectedOn(characterUp);

                // Update Character's velocity

                velocity = desiredVelocity.projectedOnPlane(characterUp) + verticalVelocity;
            }
            else
            {
                // Actual friction

                float actualFriction = characterMovement.physicsVolume && characterMovement.physicsVolume.waterVolume
                    ? 0.5f * characterMovement.physicsVolume.friction * depth
                    : 0.5f * swimmingFriction * depth;

                velocity = CalcVelocity(velocity, desiredVelocity, actualFriction, true);
            }

            // If swimming freely, apply gravity acceleration scaled by (1.0f - actualBuoyancy)

            Vector3 gravityVector = GetGravityVector();
            Vector3 actualGravity = gravityVector * (1.0f - actualBuoyancy);

            velocity += actualGravity * Time.deltaTime;

            // Update Character's velocity

            characterMovement.velocity = velocity;
        }

        /// <summary>
        /// Determines the Character's movement when 'flying'.
        /// Ground-Unconstrained movement with full desiredVelocity (lateral AND vertical) and gravity-less.
        /// </summary>

        protected virtual void Flying(Vector3 desiredVelocity)
        {
            if (useRootMotion)
                characterMovement.velocity = desiredVelocity;
            else
            {
                float actualFriction = characterMovement.physicsVolume && characterMovement.physicsVolume.waterVolume
                    ? 0.5f * characterMovement.physicsVolume.friction
                    : 0.5f * flyingFriction;

                characterMovement.velocity = CalcVelocity(characterMovement.velocity, desiredVelocity, 0.5f * actualFriction, true);
            }
        }

        /// <summary>
        /// Is the Character's sprinting ?
        /// </summary>

        public virtual bool IsSprinting()
        {
            return _sprintButtonPressed;
        }

        /// <summary>
        /// Request the character to sprint.
        /// Call this from an input event (such as a button 'down' event).
        /// </summary>

        public virtual void Sprint()
        {
            _sprintButtonPressed = true;
        }

        /// <summary>
        /// Request the character to stop sprinting.
        /// Call this from an input event (such as a button 'up' event).
        /// </summary>

        public virtual void StopSprinting()
        {
            _sprintButtonPressed = false;
        }

        /// <summary>
        /// Start a jump.
        /// Call this from an input event (such as a button 'down' event).
        /// </summary>

        public void Jump()
        {
            _jumpButtonPressed = true;
        }

        /// <summary>
        /// Stop the Character from jumping.
        /// Call this from an input event (such as a button 'up' event).
        /// </summary>

        public void StopJumping()
        {
            // Input state

            _jumpButtonPressed = false;
            _jumpButtonHeldDownTime = 0.0f;

            // Jump holding state

            _isJumping = false;
            _jumpHoldTime = 0.0f;
        }

        /// <summary>
        /// Determines if the Character is able to perform the requested jump.
        /// </summary>
        
        public virtual bool CanJump()
        {
            // Can jump while crouching ?

            if (IsCrouching() && !crouchedJump)
                return false;

            // Cant jump if no jumps available

            if (jumpMaxCount == 0 || _jumpCount >= jumpMaxCount)
                return false;

            // Is fist jump ?

            if (_jumpCount == 0)
            {
                // On first jump,
                // can jump if is walking or is falling BUT withing post grounded time

                bool canJump = IsWalking() ||
                               IsFalling() && jumpPostGroundedTime > 0.0f && fallingTime < jumpPostGroundedTime;

                // Missed post grounded time ?

                if (IsFalling() && !canJump)
                {
                    // Missed post grounded time,
                    // can jump if have any 'in-air' jumps but the first jump counts as the in-air jump

                    canJump = jumpMaxCount > 1;
                    if (canJump)
                        _jumpCount++;
                }

                return canJump;
            }

            // In air jump conditions...

            return IsFalling();
        }

        /// <summary>
        /// Determines the Character's velocity to perform the requested jump.
        /// </summary>

        protected virtual Vector3 CalcJumpVelocity()
        {
            Vector3 velocity = GetVelocity();

            Vector3 characterUp = GetUpVector();
            float verticalSpeed = Vector3.Dot(velocity, characterUp);

            return velocity.projectedOnPlane(characterUp) + characterUp * Mathf.Max(verticalSpeed, jumpImpulse);
        }

        /// <summary>
        /// Perform a jump.
        /// Called when a jump has been detected because _jumpButtonPressed == true, checks CanJump().
        /// Note that you should trigger a jump through Jump() method.
        /// </summary>

        protected virtual void PerformJump()
        {
            // Update held down timer

            if (_jumpButtonPressed)
                _jumpButtonHeldDownTime += Time.deltaTime;

            // Wants to jump and not already jumping..

            if (_jumpButtonPressed && !IsJumping())
            {
                // If jumpPreGroundedTime is enabled,
                // allow to jump only if held down time is less than tolerance

                if (jumpPreGroundedTime > 0.0f)
                {
                    bool canJump = _jumpButtonHeldDownTime <= jumpPreGroundedTime;
                    if (!canJump)
                        return;
                }

                // Can perform the requested jump ?
                
                if (!CanJump())
                    return;

                // Jump!

                SetMovementMode(MovementMode.Falling);

                PauseGroundConstraint();
                characterMovement.velocity = CalcJumpVelocity();

                _jumpCount++;
                _isJumping = true;

                OnJumped();
            }
        }

        /// <summary>
        /// Update jumping state, eg: check input, perform jump hold (jumpMaxHoldTime > 0), etc. 
        /// </summary>

        protected virtual void Jumping()
        {
            // Check jump input state and attempts to do the requested jump

            PerformJump();

            // Perform jump hold, applies an opposite gravity force proportional to _jumpHoldTime.

            if (IsJumping() && _jumpButtonPressed && jumpMaxHoldTime > 0.0f && _jumpHoldTime < jumpMaxHoldTime)
            {
                Vector3 actualGravity = GetGravityVector();

                float actualGravityMagnitude = actualGravity.magnitude;
                Vector3 actualGravityDirection = actualGravityMagnitude > 0.0f
                    ? actualGravity / actualGravityMagnitude
                    : Vector3.zero;

                float jumpProgress = Mathf.InverseLerp(0.0f, jumpMaxHoldTime, _jumpHoldTime);
                float proportionalForce = Mathf.LerpUnclamped(actualGravityMagnitude, 0.0f, jumpProgress);

                Vector3 proportionalJumpForce = -actualGravityDirection * proportionalForce;
                characterMovement.velocity += proportionalJumpForce * Time.deltaTime;

                _jumpHoldTime += Time.deltaTime;
            }

            // Should notify jump apex ?

            if (!notifyJumpApex)
                return;

            // Notify jump apex (eg: a change in vertical speed from positive to negative)

            float verticalSpeed = Vector3.Dot(characterMovement.velocity, GetUpVector());
            if (verticalSpeed < 0.0f)
            {
                notifyJumpApex = false;
                OnReachedJumpApex();
            }
        }

        /// <summary>
        /// Request the Character to start crouching.
        /// The request is processed on the next FixedUpdate.
        /// </summary>

        public virtual void Crouch()
        {
            _crouchButtonPressed = true;
        }

        /// <summary>
        /// Request the Character to stop crouching.
        /// The request is processed on the next FixedUpdate.
        /// </summary>

        public virtual void StopCrouching()
        {
            _crouchButtonPressed = false;
        }

        /// <summary>
        /// Determines if the Character is able to crouch in its current movement mode.
        /// Defaults to Walking mode only.
        /// </summary>
        
        public virtual bool CanCrouch()
        {
            return IsWalking();
        }

        /// <summary>
        /// Determines if the Character is able to uncrouch.
        /// </summary>

        public virtual bool CanUncrouch()
        {
            // Check if there's room to expand capsule

            bool overlapped = characterMovement.CheckCapsule(characterMovement.capsuleRadius, _lastCapsuleHeight);

            return !overlapped;
        }

        /// <summary>
        /// Crouch / UnCrouch logic.
        /// Called on OnFixedUpdate after Character's movement.
        /// </summary>

        protected virtual void Crouching()
        {
            // If Movement is disabled, return

            if (IsDisabled())
                return;

            // Wants to crouch and not already crouching ?

            if (_crouchButtonPressed && !IsCrouching())
            {
                // Its allowed to crouch ?

                if (!CanCrouch())
                    return;

                // Do crouch

                _isCrouched = true;

                _lastCapsuleHeight = characterMovement.capsuleHeight;

                float clampedCrouchedHeight =
                    Mathf.Clamp(crouchedHeight, characterMovement.capsuleRadius * 2.0f, characterMovement.capsuleHeight);

                characterMovement.SetCapsuleHeight(clampedCrouchedHeight);

                // Trigger Crouched event

                OnCrouched();
            }
            else if (IsCrouching() && _crouchButtonPressed == false)
            {
                // Can UnCrouch ?

                if (!CanUncrouch())
                    return;

                // Uncrouch

                _isCrouched = false;

                characterMovement.SetCapsuleHeight(_lastCapsuleHeight);

                // Trigger Uncrouched event

                OnUncrouched();
            }
        }

        /// <summary>
        /// Calculate the desired velocity for current movement mode.
        /// </summary>

        protected virtual Vector3 CalcDesiredVelocity()
        {
            // Current movement direction

            Vector3 movementDirection = GetMovementDirection();

            // The desired velocity from animation (if using root motion) or from input movement vector

            Vector3 desiredVelocity = !useRootMotion || !rootMotionController
                ? movementDirection * GetMaxSpeed()
                : rootMotionController.animRootMotionVelocity;
            
            // Return desired velocity (constrained to constraint plane if any)

            return characterMovement.ConstrainDirectionToPlane(desiredVelocity);
        }

        /// <summary>
        /// Allows to perform custom movement code here.
        /// This is not mandatory as you can simply extend the OnMove method,
        /// however using this method is simpler and cleaner when implementing custom movement modes.
        /// </summary>

        protected virtual void OnCustomMovementMode(Vector3 desiredVelocity)
        {
            // EMPTY
        }

        /// <summary>
        /// Determines the Character's movement for its current movement mode.
        /// Called during character's movement, CharacterMovement Move method (delegate).
        /// </summary>

        protected virtual void OnMove()
        {
            // Toggle walking / falling mode based on ground status

            if (IsWalking())
            {
                // Lost walkable ground or wants to leave the ground, change to Falling mode

                if (!characterMovement.IsConstrainedToGround() || !characterMovement.isOnWalkableGround)
                    SetMovementMode(MovementMode.Falling);
            }

            if (IsFalling())
            {
                // Found walkable ground and is constrained to it, change to Walking mode

                if (characterMovement.IsConstrainedToGround() && characterMovement.isOnWalkableGround)
                    SetMovementMode(MovementMode.Walking);
            }

            // Apply current movement mode

            Vector3 desiredVelocity = CalcDesiredVelocity();

            switch (_movementMode)
            {
                case MovementMode.Walking:
                {
                    Walking(desiredVelocity);
                    break;
                }

                case MovementMode.Falling:
                {
                    Falling(desiredVelocity);

                    _fallingTime = fallingTime + Time.deltaTime;

                    break;
                }

                case MovementMode.Swimming:
                {
                    Swimming(desiredVelocity);
                    break;
                }

                case MovementMode.Flying:
                {
                    Flying(desiredVelocity);
                    break;
                }

                case MovementMode.Custom:
                {
                    OnCustomMovementMode(desiredVelocity);
                    break;
                }
            }

            // Jumping state

            Jumping();
        }

        /// <summary>
        /// Set a pending launch velocity on the Character. This velocity will be processed next FixedUpdate.
        /// Triggers the OnLaunched event.
        /// </summary>
        /// <param name="launchVelocity">The velocity to impart to the Character.</param>
        /// <param name="overrideVerticalVelocity">If true replace the vertical component of the Character's velocity instead of adding to it.</param>
        /// <param name="overrideLateralVelocity">If true replace the XY part of the Character's velocity instead of adding to it.</param>

        public virtual void LaunchCharacter(Vector3 launchVelocity, bool overrideVerticalVelocity = false, bool overrideLateralVelocity = false)
        {
            // Compute final velocity

            Vector3 finalVelocity = launchVelocity;

            // If not override, add lateral velocity to given launch velocity

            Vector3 characterUp = GetUpVector();

            if (!overrideLateralVelocity)
                finalVelocity += characterMovement.velocity.projectedOnPlane(characterUp);

            // If not override, add vertical velocity to given launch velocity

            if (!overrideVerticalVelocity)
                finalVelocity += characterMovement.velocity.projectedOn(characterUp);

            _pendingLaunchVelocity = finalVelocity;

            // Trigger OnLaunched event

            OnLaunched(launchVelocity, overrideLateralVelocity, overrideLateralVelocity);
        }

        /// <summary>
        /// Applies a pending launch velocity, will clear pendingLaunchVelocity.
        /// Returns true if the launch was triggered.
        /// </summary>

        protected virtual bool ApplyPendingLaunch()
        {
            if (_pendingLaunchVelocity.isZero())
                return false;

            characterMovement.velocity = _pendingLaunchVelocity;
            _pendingLaunchVelocity = Vector3.zero;

            return true;
        }

        /// <summary>
        /// Adds a force to the Character.
        /// This forces will be accumulated and applied at the end of the Character's movement.
        /// </summary>

        public virtual void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            switch (forceMode)
            {
                case ForceMode.Force:
                {
                    _pendingForces += force / mass * Time.deltaTime;
                    break;
                }

                case ForceMode.Acceleration:
                {
                    _pendingForces += force * Time.deltaTime;
                    break;
                }

                case ForceMode.Impulse:
                {
                    _pendingForces += force / mass;
                    break;
                }

                case ForceMode.VelocityChange:
                {
                    _pendingForces += force;
                    break;
                }
            }
        }

        /// <summary>
        /// Applies the accumulated forces through AddForce, then clears those forces (reset _pendingForces).
        /// </summary>

        protected virtual void ApplyAccumulatedForces()
        {
            if (_pendingForces.isZero())
                return;

            characterMovement.velocity += _pendingForces;
            _pendingForces = Vector3.zero;
        }

        /// <summary>
        /// Clears any accumulated forces, including any pending launch velocity.
        /// </summary>

        protected virtual void ClearAccumulatedForces()
        {
            _pendingForces = Vector3.zero;
            _pendingLaunchVelocity = Vector3.zero;
        }

        /// <summary>
        /// Calculates the resultant displacement after a movement hit (eg: slide along collision).
        /// Called during CharacterMovement CollideAndSlide.
        /// </summary>

        protected virtual Vector3 ComputeCollisionResponseDisplacement(Vector3 displacement, ref MovementHit movementHit)
        {
            if (IsWalking())
            {
                // On walkable 'ground'

                if (movementHit.hitWalkableGround)
                {
                    // Hit walkable, reorient displacement along ground surface

                    displacement = displacement.tangentTo(movementHit.normal, GetUpVector());
                }
                else
                {
                    // Hit non-walkable, cancel displacement against hit and reorient remaining along ground surface

                    displacement = displacement.projectedOnPlane(movementHit.normal);
                    displacement = displacement.tangentTo(characterMovement.groundHit.normal, GetUpVector());
                }
            }
            else
            {
                // On 'Air' or Non-walkable 'ground'
                
                if (IsFalling() && movementHit.hitWalkableGround)
                {
                    // Hit walkable, cancel vertical velocity and reorient remaining along ground surface

                    Vector3 characterUp = GetUpVector();

                    displacement = displacement.projectedOnPlane(characterUp);
                    displacement = displacement.tangentTo(movementHit.normal, characterUp);
                }
                else
                {
                    // Hit non-walkable, project displacement on hit surface plane

                    displacement = displacement.projectedOnPlane(movementHit.normal);
                }
            }

            // Return slide displacement (constrained to constraint plane)

            return characterMovement.ConstrainDirectionToPlane(displacement);
        }

        /// <summary>
        /// Performs Character's movement.
        /// Handle forces, impulses, crouching state etc.
        /// </summary>

        protected virtual void PerformMovement()
        {
            // If Movement is disabled, return

            if (IsDisabled())
                return;

            // Apply accumulated forces (last frame)

            ApplyAccumulatedForces();

            // Handles any pending launch during last frame,
            // this has higher priority over accumulated forces, as this could be a velocity override

            ApplyPendingLaunch();

            // Perform Character's movement (collision constrained aka: collide and slide),
            // handle platforms, push others, etc.

            characterMovement.Move();

            // Handle crouching

            Crouching();
        }

        /// <summary>
        /// Determines if the Character is able to step up on given collider.
        /// Returns true if we can step up on the collider in the given hitResult.
        /// </summary>

        protected virtual bool CanStepUp(ref RaycastHit hitResult)
        {
            // Do not allow to step on dynamic rigidbodies

            return !hitResult.rigidbody || hitResult.rigidbody.isKinematic;
        }

        /// <summary>
        /// Apply a push force to other rigidbody or character (only if applyPushForce == true).
        /// </summary>

        protected virtual void OnApplyPushForce(ref RigidbodyHit rigidbodyHit)
        {
            // Not enabled, return

            if (!applyPushForce)
                return;

            // If Character is not on ground, cant push other

            if (!IsOnWalkableGround() || !characterMovement.IsConstrainedToGround())
                return;

            // Do not allow to push walkable

            if (rigidbodyHit.isWalkable)
                return;

            // Did we hit other character ?

            Character otherCharacter = rigidbodyHit.rigidbody.GetComponent<Character>();
            if (otherCharacter && !pushForceAffectCharacters)
                return;

            // Solve collision here applying push forces
            // Source: https://www.gamasutra.com/view/feature/131424/pool_hall_lessons_fast_accurate_.php?page=3

            Vector3 v1 = rigidbodyHit.velocity;
            Vector3 v2 = otherCharacter ? otherCharacter.GetVelocity() : rigidbodyHit.otherVelocity;

            Vector3 n = rigidbodyHit.normal;

            float a1 = Vector3.Dot(v1, n);
            float a2 = Vector3.Dot(v2, n);
            
            float m1 = mass;
            float m2 = otherCharacter ? otherCharacter.mass : rigidbodyHit.rigidbody.mass;

            float optimizedP = 1.0f * (a1 - a2) / (m1 + m2);

            characterMovement.velocity += (a1 - optimizedP * m2) * n;

            if (otherCharacter)
                otherCharacter.AddForce(optimizedP * m1 * pushForceScale * n, ForceMode.VelocityChange);
            else
                rigidbodyHit.rigidbody.AddForceAtPosition(optimizedP * m1 * pushForceScale * n, rigidbodyHit.point, ForceMode.VelocityChange);
        }

        /// <summary>
        /// Apply a downward force when standing on top of non-kinematic physics objects (if applyStandingDownwardForce == true).
        /// The force applied is: mass * gravity * standingDownwardForceScale
        /// </summary>

        protected virtual void OnApplyStandingDownwardForce(Rigidbody otherRigidbody)
        {
            // If not enabled, return

            if (!applyStandingDownwardForce)
                return;

            // If other is a Character, return

            Character character =
                otherRigidbody.gameObject.GetComponent<Character>();

            if (character != null)
                return;

            // Apply downward force

            Vector3 downwardForce = mass * GetGravityVector();

            otherRigidbody.AddForceAtPosition(downwardForce * standingDownwardForceScale, characterMovement.groundHit.point);
        }

        /// <summary>
        /// Determines if the Character should be moved with otherRigidbody when standing on top of it (eg: moving platform).
        /// </summary>

        protected virtual bool ShouldMoveCharacterWhenStandingOn(Rigidbody otherRigidbody)
        {
            // Should be moved along with the moving platform ?

            if (!impartPlatformMovement)
                return false;

            // Do not move if not Walking or Falling

            return IsWalking() || IsFalling();
        }

        /// <summary>
        /// Determines if the Character should be rotated with otherRigidbody when standing on top of it (eg: rotating platform).
        /// </summary>

        protected virtual bool ShouldRotateCharacterWhenStandingOn(Rigidbody otherRigidbody)
        {
            // Should be rotated along with the moving platform ?

            if (!impartPlatformRotation)
                return false;

            // Do not rotate if not Walking or Falling

            if (!IsWalking() && !IsFalling())
                return false;

            // Defaults to kinematic rigidbodies only

            return otherRigidbody && otherRigidbody.isKinematic;
        }

        /// <summary>
        /// Determines how to impart the platform's velocity Character leaves the platform.
        /// </summary>

        protected virtual void ImpartPlatformVelocity(Vector3 platformVelocity)
        {
            // Do not impart platform velocity if not Walking or Falling

            if (!IsWalking() && !IsFalling())
                return;

            // If Character is constrained to ground, ALWAYS impart platform's velocity,
            // this is to prevent a pullback when walking off a moving platform

            if (characterMovement.IsConstrainedToGround())
                characterMovement.velocity += platformVelocity;
            else
            {
                // If enabled, impart platform's velocity to Character

                if (impartPlatformVelocity)
                    characterMovement.velocity += platformVelocity;
            }
        }

        /// <summary>
        /// Should external forces modify the Character's velocity ?
        /// Determines how to impart the external velocity caused by external forces during internal physics simulation.
        /// </summary>

        protected virtual void ImpartExternalVelocity(Vector3 externalVelocity)
        {
            // Should impart external velocity

            if (!impartExternalVelocity)
                return;

            // Impart external velocity (velocity caused during internal physics update)

            characterMovement.velocity += externalVelocity;
        }

        /// <summary>
        /// Assigns the Character's movement direction (in world space), eg: our desired movement direction vector.
        /// </summary>
        /// <param name="movementDirection">The movement direction in world space.</param>

        public void SetMovementDirection(Vector3 movementDirection)
        {
            _movementDirection = movementDirection;
        }

        /// <summary>
        /// The current movement direction (in world space), eg: the movement direction used to move this Character.
        /// </summary>

        public Vector3 GetMovementDirection()
        {
            return _movementDirection;
        }

        /// <summary>
        /// Sets the yaw value. This will reset the current pitch and roll values.
        /// </summary>
        
        public virtual void SetYaw(float value)
        {
            transform.rotation = Quaternion.Euler(0.0f, value, 0.0f);
        }

        /// <summary>
        /// Amount to add to Yaw (up axis).
        /// </summary>

        public virtual void AddYawInput(float value)
        {
            if (value != 0.0f)
                characterMovement.rotation *= Quaternion.Euler(0.0f, value, 0.0f);
        }

        /// <summary>
        /// Amount to add to Pitch (right axis).
        /// </summary>

        public virtual void AddPitchInput(float value)
        {
            if (value != 0.0f)
                characterMovement.rotation *= Quaternion.Euler(value, 0.0f, 0.0f);
        }

        /// <summary>
        /// Amount to add to Roll (forward axis).
        /// </summary>

        public virtual void AddRollInput(float value)
        {
            if (value != 0.0f)
                characterMovement.rotation *= Quaternion.Euler(0.0f, 0.0f, value);
        }

        /// <summary>
        /// Orient the character's towards the given direction using rotationRate as the rate of rotation change.
        /// </summary>
        /// <param name="worldDirection">The target direction in world space.</param>
        /// <param name="isPlanar">If True, the rotation will be performed on the Character's plane (defined by its up-axis).</param>

        protected virtual void RotateTowards(Vector3 worldDirection, bool isPlanar = true)
        {
            Vector3 characterUp = GetUpVector();

            if (isPlanar)
                worldDirection = worldDirection.projectedOnPlane(characterUp);

            if (worldDirection.isZero())
                return;

            Quaternion targetRotation = Quaternion.LookRotation(worldDirection, characterUp);

            characterMovement.rotation = 
                Quaternion.Slerp(characterMovement.rotation, targetRotation, rotationRate * Mathf.Deg2Rad * Time.deltaTime);
        }

        /// <summary>
        /// Append root motion rotation to Character's rotation.
        /// </summary>

        public virtual void ApplyRootMotionRotation()
        {
            if (rootMotionController != null)
                characterMovement.rotation *= rootMotionController.animDeltaRotation;
        }

        /// <summary>
        /// Should collision detection be enabled ?
        /// </summary>

        public virtual void DetectCollisions(bool detectCollisions)
        {
            characterMovement.DetectCollisions(detectCollisions);
        }

        /// <summary>
        /// Makes the collision detection system ignore all collisions between Character's capsule collider and otherCollider.
        /// </summary>
        
        public virtual void IgnoreCollision(Collider otherCollider, bool ignore = true)
        {
            characterMovement.IgnoreCollision(otherCollider, ignore);
        }

        /// <summary>
        /// Temporarily disable ground constraint allowing the Character to freely leave the ground.
        /// Eg: LaunchCharacter, Jump, etc.
        /// </summary>

        public virtual void PauseGroundConstraint(float seconds = 0.1f)
        {
            characterMovement.PauseGroundConstraint(seconds);
        }

        /// <summary>
        /// Setup player InputActions (if any).
        /// </summary>
        
        protected virtual void SetupPlayerInput()
        {
            // Attempts to cache Character InputActions (if any)

            if (actions == null)
                return;

            // Movement input action (This is polled)

            movementInputAction = actions.FindAction("Movement");

            // Setup Jump input action handlers

            jumpInputAction = actions.FindAction("Jump");
            if (jumpInputAction != null)
            {
                jumpInputAction.started += OnJump;
                jumpInputAction.performed += OnJump;
                jumpInputAction.canceled += OnJump;
            }

            // Setup Crouch input action handlers

            crouchInputAction = actions.FindAction("Crouch");
            if (crouchInputAction != null)
            {
                crouchInputAction.started += OnCrouch;
                crouchInputAction.performed += OnCrouch;
                crouchInputAction.canceled += OnCrouch;
            }

            // Setup Sprint input action handlers

            sprintInputAction = actions.FindAction("Sprint");
            if (sprintInputAction != null)
            {
                sprintInputAction.started += OnSprint;
                sprintInputAction.performed += OnSprint;
                sprintInputAction.canceled += OnSprint;
            }
        }

        /// <summary>
        /// Handle Player input, only if actions are assigned (eg: actions != null).
        /// </summary>

        protected virtual void HandleInput()
        {
            // Should handle input ?

            if (actions == null)
                return;

            // Poll movement InputAction

            var movementInput = GetMovementInput();
            
            if (cameraTransform == null)
            {
                // If Camera is not assigned, add movement input relative to world axis

                Vector3 movementDirection = Vector3.zero;

                movementDirection += Vector3.right * movementInput.x;
                movementDirection += Vector3.forward * movementInput.y;

                SetMovementDirection(movementDirection);
            }
            else
            {
                // If Camera is assigned, add input movement relative to camera look direction

                Vector3 movementDirection = Vector3.zero;

                movementDirection += Vector3.right * movementInput.x;
                movementDirection += Vector3.forward * movementInput.y;
                
                movementDirection = movementDirection.relativeTo(cameraTransform);

                SetMovementDirection(movementDirection);
            }
        }

        /// <summary>
        /// Updates the Character's rotation based on its current RotationMode.
        /// </summary>

        protected virtual void UpdateRotation()
        {
            // Is Character movement is disabled, return

            if (IsDisabled())
                return;

            // Should update Character's rotation ?

            RotationMode rotationMode = GetRotationMode();
            if (rotationMode == RotationMode.None)
                return;

            if (rotationMode == RotationMode.OrientToMovement)
            {
                // Orient towards input move direction vector (in Character's plane)
                
                Vector3 movementDirection = GetMovementDirection();

                RotateTowards(movementDirection);
            }
            else if (rotationMode == RotationMode.OrientToCameraViewDirection)
            {
                // Orient towards camera view direction (in Character's plane)

                if (cameraTransform == null)
                    return;
                
                Vector3 viewDirection = cameraTransform.forward;

                RotateTowards(viewDirection);
            }
            else if (rotationMode == RotationMode.OrientWithRootMotion)
            {
                // Append root motion rotation to Character's rotation.

                if (rootMotionController)
                    ApplyRootMotionRotation();
            }
        }
        
        /// <summary>
        /// Helper method used to feed the Character's animator.
        /// </summary>

        protected virtual void Animate()
        {
            // EMPTY
        }

        /// <summary>
        /// Our Reset method. Set this default values.
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnReset()
        {
            SetRotationMode(RotationMode.OrientToMovement);

            rotationRate = 540.0f;

            maxWalkSpeed = 6.0f;
            minAnalogWalkSpeed = 0.0f;
            maxAcceleration = 20.0f;
            brakingDecelerationWalking = 20.0f;
            groundFriction = 8.0f;

            useSeparateBrakingFriction = false;
            brakingFriction = 0.0f;

            sprintSpeedMultiplier = 1.5f;
            sprintAccelerationMultiplier = 1.0f;

            maxWalkSpeedCrouched = 3.0f;
            crouchedHeight = 1.0f;
            crouchedJump = false;

            maxFallSpeed = 40.0f;
            brakingDecelerationFalling = 0.0f;
            fallingLateralFriction = 0.3f;
            airControl = 0.8f;

            jumpMaxCount = 1;
            jumpImpulse = 6.0f;
            jumpMaxHoldTime = 0.35f;
            jumpPreGroundedTime = 0.15f;
            jumpPostGroundedTime = 0.15f;

            gravity = new Vector3(0.0f, -30.0f, 0.0f);

            canSwim = true;
            maxSwimSpeed = 3.0f;
            brakingDecelerationSwimming = 0.0f;
            swimmingFriction = 0.5f;
            buoyancy = 1.0f;

            maxFlySpeed = 6.0f;
            brakingDecelerationFlying = 0.0f;
            flyingFriction = 0.3f;

            mass = 1.0f;
            impartExternalVelocity = true;
            impartPlatformMovement = true;
            impartPlatformRotation = true;
            impartPlatformVelocity = true;
            applyPushForce = true;
            pushForceAffectCharacters = true;
            applyStandingDownwardForce = true;

            pushForceScale = 1.0f;
            standingDownwardForceScale = 1.0f;
        }

        /// <summary>
        /// Our OnValidate method.
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnOnValidate()
        {
            rotationRate = _rotationRate;

            maxWalkSpeed = _maxWalkSpeed;
            minAnalogWalkSpeed = _minAnalogWalkSpeed;
            maxAcceleration = _maxAcceleration;
            brakingDecelerationWalking = _brakingDecelerationWalking;
            groundFriction = _groundFriction;

            sprintSpeedMultiplier = _sprintSpeedMultiplier;
            sprintAccelerationMultiplier = _sprintAccelerationMultiplier;

            maxWalkSpeedCrouched = _maxWalkSpeedCrouched;
            crouchedHeight = _crouchedHeight;

            maxFallSpeed = _maxFallSpeed;
            brakingDecelerationFalling = _brakingDecelerationFalling;
            fallingLateralFriction = _fallingLateralFriction;
            airControl = _airControl;

            jumpMaxCount = _jumpMaxCount;
            jumpImpulse = _jumpImpulse;
            jumpMaxHoldTime = _jumpMaxHoldTime;
            jumpPreGroundedTime = _jumpPreGroundedTime;
            jumpPostGroundedTime = _jumpPostGroundedTime;

            maxSwimSpeed = _maxSwimSpeed;
            brakingDecelerationSwimming = _brakingDecelerationSwimming;
            swimmingFriction = _swimmingFriction;
            buoyancy = _buoyancy;

            maxFlySpeed = _maxFlySpeed;
            brakingDecelerationFlying = _brakingDecelerationFlying;
            flyingFriction = _flyingFriction;

            mass = _mass;

            pushForceScale = _pushForceScale;
            standingDownwardForceScale = _standingDownwardForceScale;
        }

        /// <summary>
        /// Called when the script instance is being loaded (Awake).
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnAwake()
        {
            // Cache and init components

            _characterMovement = GetComponent<CharacterMovement>();
            _characterMovement.SetCallbackTarget(this);

            _animator = GetComponentInChildren<Animator>();
            _rootMotionController = GetComponentInChildren<RootMotionController>();

            // Initialize player input action (if any)

            SetupPlayerInput();
        }

        /// <summary>
        /// Our OnDestroy method.
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnOnDestroy()
        {
            _characterMovement.RemoveCallbackTarget();
        }

        /// <summary>
        /// Our OnEnable method.
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnOnEnable()
        {
            // Enable input actions

            movementInputAction?.Enable();            
            jumpInputAction?.Enable();
            crouchInputAction?.Enable();
            sprintInputAction?.Enable();
        }

        /// <summary>
        /// Called when the behaviour becomes disabled (OnDisable).
        /// If overriden, must call base method in order to fully de-initialize the class.
        /// </summary>

        protected virtual void OnOnDisable()
        {
            // Disable input actions

            movementInputAction?.Disable();            
            jumpInputAction?.Disable();
            crouchInputAction?.Disable();
            sprintInputAction?.Disable();
        }

        /// <summary>
        /// Called on the frame when a script is enabled just before any of the Update methods are called the first time (Start).
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnStart()
        {
            // Defaults to walking movement mode

            SetMovementMode(MovementMode.Walking);
        }

        /// <summary>
        /// Our FixedUpdate method.
        /// </summary>

        protected virtual void OnFixedUpdate()
        {
            PerformMovement();
        }

        /// <summary>
        /// Our Update method.
        /// </summary>

        protected virtual void OnUpdate()
        {
            HandleInput();

            UpdateRotation();

            Animate();
        }

        /// <summary>
        /// Our LateUpdate method.
        /// </summary>

        protected virtual void OnLateUpdate()
        {
            // EMPTY
        }

        #endregion

        #region MONOBEHAVIOUR
        
        private void Reset()
        {
            OnReset();
        }

        private void OnValidate()
        {
            OnOnValidate();
        }
        
        private void Awake()
        {
            OnAwake();
        }

        private void OnDestroy()
        {
            OnOnDestroy();
        }
        
        private void OnEnable()
        {
            OnOnEnable();
        }

        private void OnDisable()
        {
            OnOnDisable();
        }

        private void Start()
        {
            OnStart();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        private void Update()
        {
            OnUpdate();
        }

        private void LateUpdate()
        {
            OnLateUpdate();
        }

        #endregion
    }

    #endregion
}