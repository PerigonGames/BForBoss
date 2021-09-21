using ECM2.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECM2.Components
{
    #region ENUMS

    /// <summary>
    /// The hit location WRT Character's capsule.
    /// </summary>

    public enum CapsuleHitLocation
    {
        Bottom,
        Sides,
        Top
    }

    /// <summary>
    /// The axis that constraints movement.
    /// </summary>
    
    public enum PlaneConstraint
    {
        None,
        ConstraintXAxis,
        ConstraintYAxis,
        ConstraintZAxis
    }

    #endregion

    #region STRUCTS

    /// <summary>
    /// Data about the ground for walking movement, used by CharacterMovement component.
    /// </summary>

    public struct GroundDistance
    {
        /// <summary>
        /// True if the hit found a valid walkable floor (angle is less than or equal to slopeLimit).
        /// </summary>

        public bool isWalkable;

        /// <summary>
        /// The distance to the floor, computed from the swept capsule trace.
        /// </summary>

        public float sweepDistance;

        /// <summary>
        /// The swept volume position on ground.
        /// </summary>

        public Vector3 sweepPosition;

        /// <summary>
        /// True if the hit found a valid walkable ground using a raycast (rather than a sweep test, which happens when the sweep test fails to yield a walkable surface).
        /// </summary>

        public bool isRaycast;

        /// <summary>
        /// The distance to the floor, computed from the raycast. Only valid if isRaycast is true.
        /// </summary>

        public float raycastDistance;

        /// <summary>
        /// Hit result of the test that found a floor.
        /// </summary>
            
        public RaycastHit hitResult;
    }

    /// <summary>
    /// Holds information about a ledge.
    /// </summary>

    public struct LedgeHit
    {
        /// <summary>
        /// Is a ledge ?
        /// </summary>

        public bool isLedge;

        /// <summary>
        /// Is the Character's on the 'empty' side of the ledge ?
        /// </summary>

        public bool isOnEmptySide;

        /// <summary>
        /// Is the Character's walking off the ledge?
        /// </summary>

        public bool isWalkingOff;

        /// <summary>
        /// The horizontal distance from the Character's position to the ledge.
        /// </summary>

        public float ledgeDistance;

        /// <summary>
        /// The ledge surface normal.
        /// </summary>

        public Vector3 ledgeNormal;

        /// <summary>
        /// The ledge forward vector (orthogonal to surface normal).
        /// </summary>

        public Vector3 ledgeForward;
    }

    /// <summary>
    /// Holds information about a step.
    /// </summary>

    public struct StepHit
    {
        /// <summary>
        /// Is a valid step ?
        /// </summary>

        public bool isStep;

        /// <summary>
        /// Is a climbable step ? (eg: its height is higher than capsule radius).
        /// </summary>
        
        public bool isClimbable;

        /// <summary>
        /// The Character's position on step.
        /// </summary>

        public Vector3 position;

        /// <summary>
        /// The impact point in world space.
        /// </summary>

        public Vector3 point;

        /// <summary>
        /// The normal of the hit surface.
        /// </summary>

        public Vector3 normal;

        /// <summary>
        /// The collider of the hit object.
        /// </summary>

        public Collider collider;

        /// <summary>
        /// The rigidbody of the hit object (if any).
        /// </summary>

        public Rigidbody rigidbody => collider ? collider.attachedRigidbody : null;
    }

    /// <summary>
    /// Holds information about the ground.
    /// </summary>

    public struct GroundHit
    {
        /// <summary>
        /// The hit location WRT Character's capsule, eg: Bottom, Sides, Top.
        /// </summary>

        public CapsuleHitLocation hitLocation;

        /// <summary>
        /// Did we hit ground ? Eg. impacted capsule bottom sphere.
        /// </summary>

        public bool hitGround;

        /// <summary>
        /// Is the impacted ground walkable ?
        /// </summary>

        public bool isWalkable;

        /// <summary>
        /// Is walkable ground ?
        /// </summary>

        public bool hitWalkableGround => hitGround && isWalkable;

        /// <summary>
        /// The Character's position.
        /// </summary>

        public Vector3 position;

        /// <summary>
        /// The impact point in world space.
        /// </summary>

        public Vector3 point;

        /// <summary>
        /// The normal of the hit surface.
        /// </summary>

        public Vector3 normal;

        /// <summary>
        /// The collider of the hit object.
        /// </summary>

        public Collider collider;

        /// <summary>
        /// The rigidbody of the hit object (if any).
        /// </summary>

        public Rigidbody rigidbody => collider ? collider.attachedRigidbody : null;

        /// <summary>
        /// If the 'ground' is a step, holds information about it.
        /// </summary>

        public StepHit stepHitResult;
    }

    /// <summary>
    /// Holds information about hits found during movement (eg: CollideAndSlide).
    /// </summary>

    public struct MovementHit
    {
        /// <summary>
        /// The hit location WRT Character's capsule, eg: Bottom, Sides, Top.
        /// </summary>

        public CapsuleHitLocation hitLocation;

        /// <summary>
        /// Did we hit ground ?
        /// </summary>

        public bool hitGround;

        /// <summary>
        /// Is the ground walkable ?
        /// </summary>

        public bool isWalkable;

        /// <summary>
        /// Is walkable ground ?
        /// </summary>

        public bool hitWalkableGround => hitGround && isWalkable;

        /// <summary>
        /// The initial displacement.
        /// </summary>

        public Vector3 displacement;

        /// <summary>
        /// The displacement up to the hit.
        /// </summary>

        public Vector3 displacementToHit;

        /// <summary>
        /// The impact point in world space.
        /// </summary>

        public Vector3 point;

        /// <summary>
        /// The normal of the hit surface.
        /// </summary>

        public Vector3 normal;

        /// <summary>
        /// The collider of the hit object.
        /// </summary>

        public Collider collider;

        /// <summary>
        /// The rigidbody of the hit object (if any).
        /// </summary>

        public Rigidbody rigidbody => collider ? collider.attachedRigidbody : null;
    }
    
    /// <summary>
    /// Holds information about the rigidbody impacted during Character's movement (CollideAndSlide).
    /// </summary>

    public struct RigidbodyHit
    {
        /// <summary>
        /// The hit location WRT Character's capsule, eg: Bottom, Sides, Top.
        /// </summary>

        public CapsuleHitLocation hitLocation;

        /// <summary>
        /// Is the hit walkable ?
        /// </summary>

        public bool isWalkable;

        /// <summary>
        /// The Character's velocity at impact time.
        /// </summary>

        public Vector3 velocity;

        /// <summary>
        /// Other rigidbody velocity.
        /// </summary>

        public Vector3 otherVelocity;

        /// <summary>
        /// The impact point in world space.
        /// </summary>

        public Vector3 point;

        /// <summary>
        /// The normal of the hit surface.
        /// </summary>

        public Vector3 normal;

        /// <summary>
        /// The collider of the hit object.
        /// </summary>

        public Collider collider;

        /// <summary>
        /// The rigidbody of the hit object.
        /// </summary>

        public Rigidbody rigidbody => collider ? collider.attachedRigidbody : null;
    }

    #endregion

    #region CLASS

    /// <summary>
    /// Character Movement Component
    /// 
    /// The CharacterMovement component allows you to easily perform movement constrained by collisions without having to deal with its inherent difficulty.
    /// This is based in the collide-and-slide algorithm, a collision detection algorithm capable of detect 'future' collisions and gracefully
    /// react to those (eg: slide along obstacles), providing a robust and well-known behavior for Character control.
    /// 
    /// </summary>

    public class CharacterMovement : MonoBehaviour, IColliderFilter
    {
        #region CONSTANTS

        protected const int kMaxHitCount = 8;
        protected const int kMaxOverlapCount = 8;

        protected const int kMaxMovementIterations = 3;
        protected const int kMaxOverlapTests = 1;

        protected const float kContactOffset = 0.001f;

        protected const float kRaycastVerticalOffset = 0.05f;
        protected const float kRaycastHorizontalOffset = 0.001f;

        protected const float kMinGroundProbingDistance = 0.005f;
        protected const float kMinMovementDistance = 0.0001f;

        protected const float kMinHeightForLedge = 0.05f;
        protected const float kMinStepDepth = 0.05f;

        protected const float kGroundSweepBackstepDistance = 0.1f;
        protected const float kDisplacementSweepBackstepDistance = 0.05f;
        protected const float kStepSweepBackstepDistance = 0.002f;

        #endregion

        #region EDITOR EXPOSED FIELDS

        [Space(15f)]
        [Tooltip("Allow to constrain the Character so movement along the locked axis is not possible.")]
        [SerializeField]
        private PlaneConstraint _constrainToPlane;

        [Space(15f)]
        [Tooltip("The Character's capsule collider radius.")]
        [SerializeField]
        private float _capsuleRadius;

        [Tooltip("The Character's capsule collider height")]
        [SerializeField]
        private float _capsuleHeight;

        [Space(15f)]
        [Tooltip("The maximum angle (in degrees) for a walkable slope.")]
        [SerializeField]
        private float _slopeLimit;
        
        [Tooltip("The maximum height (in meters) for a valid step (up to Character's height).")]
        [SerializeField]
        private float _stepOffset;
        
        [Tooltip("Allow a Character to perch on the edge of a surface if the horizontal distance from the Character's position to the edge is closer than this.\n" +
                 "Note that characters will not fall off if they are within stepOffset of a walkable surface below.")]
        [SerializeField]
        private float _perchOffset;

        [Tooltip("Allow a Character to unperch from the edge of a surface if the horizontal distance from the Character's position to the edge is greater than this.\n" +
                 "Note that characters will not fall off if they are within stepOffset of a walkable surface below.")]
        [SerializeField]
        private float _unperchOffset;

        [Tooltip("When perching on a ledge, add this additional distance to stepOffset when determining how high above a walkable floor we can perch.\n" +
        		 "Note that we still enforce stepOffset to start the step up, this just allows the Character to hang off the edge or step slightly higher off the ground.")]
        [SerializeField]
        private float _perchAdditionalHeight;

        [Tooltip("Determines if a Character should collide with triggers.")]
        [SerializeField]
        private bool _collideWithTriggers;

        [Space(15f)]
        [Tooltip("Determines the layers to be considered as walkable ground.")]
        [SerializeField]
        private LayerMask _groundMask;

        [Tooltip("Determines the maximum length of the cast.\n" +
                 "As a rule of thumb, configure it to your Character's collider radius.")]
        [SerializeField]
        private float _groundProbingDistance;

        #endregion

        #region FIELDS

        protected RaycastHit[] _hits = new RaycastHit[kMaxHitCount];
        protected Collider[] _overlaps = new Collider[kMaxOverlapCount];

        protected HashSet<Collider> _ignoredColliders = new HashSet<Collider>();

        protected GroundHit _lastGroundHitResult;
        protected GroundHit _groundHitResult;

        protected int _movementHitCount;
        protected MovementHit[] _movementHits = new MovementHit[kMaxHitCount];
        
        protected List<PhysicsVolume> _volumes = new List<PhysicsVolume>();

        protected List<RigidbodyHit> _rigidbodyHits = new List<RigidbodyHit>();

        private Transform _transform;
        private Rigidbody _rigidbody;
        private CapsuleCollider _capsuleCollider;

        private Vector3 _capsuleCenter;
        private Vector3 _capsuleTopCenter;
        private Vector3 _capsuleBottomCenter;

        private Vector3 _planeConstraintNormal = Vector3.zero;
        
        protected bool _isConstrainedToGround = true;
        protected float _unconstrainedTimer;

        protected bool _detectCollisions = true;

        protected Vector3 _probingPosition;
        protected Quaternion _probingRotation;
        protected float _probingDistance;

        protected float _minSlopLimit;

        protected Collider _recordedPlatform;

        protected Collider _lastActivePlatform;
        protected Collider _activePlatform;

        protected Vector3 _lastActivePlatformVelocity;
        protected Vector3 _activePlatformVelocity;

        private ICharacterMovementCallbacks _callbackTarget;

        protected Coroutine _lateFixedUpdateCoroutine;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The Character's transform.
        /// </summary>

        public new Transform transform
        {
            get
            {
#if UNITY_EDITOR
                if (_transform == null)
                    _transform = GetComponent<Transform>();
#endif

                return _transform;
            }
        }

        /// <summary>
        /// The Character's rigidbody.
        /// </summary>

        public new Rigidbody rigidbody
        {
            get
            {
#if UNITY_EDITOR
                if (_rigidbody == null)
                    _rigidbody = GetComponent<Rigidbody>();
#endif

                return _rigidbody;
            }
        }

        /// <summary>
        /// The Character's capsule collider.
        /// </summary>

        public CapsuleCollider capsuleCollider
        {
            get
            {
#if UNITY_EDITOR
                if (_capsuleCollider == null)
                    _capsuleCollider = GetComponent<CapsuleCollider>();
#endif

                return _capsuleCollider;
            }
        }

        /// <summary>
        /// The Character's capsule collider radius.
        /// </summary>

        public float capsuleRadius => _capsuleRadius;

        /// <summary>
        /// The Character's capsule collider height.
        /// </summary>

        public float capsuleHeight => _capsuleHeight;

        /// <summary>
        /// The maximum angle (in degrees) for a walkable slope.
        /// </summary>

        public float slopeLimit
        {
            get => _slopeLimit;
            set
            {
                _slopeLimit = Mathf.Clamp(value, 0.0f, 89.0f);
                _minSlopLimit = Mathf.Cos(_slopeLimit * Mathf.Deg2Rad);
            }
        }

        /// <summary>
        /// The maximum height (in meters) for a valid step (up to Character's height).
        /// </summary>

        public float stepOffset
        {
            get => _stepOffset;
            set => _stepOffset = Mathf.Clamp(value, 0.0f, _capsuleHeight);
        }

        /// <summary>
        /// Allow a Character to perch on the edge of a surface if the horizontal distance from the Character's position to the edge is closer than this.
        /// Note that we still enforce stepOffset to start the step up, this just allows the Character to hang off the edge or step slightly higher off the ground.
        /// </summary>

        public float perchOffset
        {
            get => _perchOffset;
            set => _perchOffset = Mathf.Clamp(value, 0.0f, _capsuleRadius);
        }

        /// <summary>
        /// When perching on a ledge, add this additional distance to stepOffset when determining how high above a walkable floor we can perch.
        /// </summary>

        public float perchAdditionalHeight
        {
            get => _perchAdditionalHeight;
            set => _perchAdditionalHeight = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Allow a Character to unperch from the edge of a surface if the horizontal distance from the Character's position to the edge is greater than this.
        /// Note that characters will not fall off if they are within stepOffset of a walkable surface below.
        /// </summary>

        public float unperchOffset
        {
            get => _unperchOffset;
            set => _unperchOffset = Mathf.Clamp(value, 0.0f, _capsuleRadius);
        }

        /// <summary>
        /// Layers to be considered as walkable ground.
        /// </summary>

        public float groundProbingDistance
        {
            get => _groundProbingDistance;
            set => _groundProbingDistance = Mathf.Max(0.0f, value);
        }

        /// <summary>
        /// Layers to be considered as walkable ground.
        /// </summary>

        public LayerMask groundMask
        {
            get => _groundMask;
            set => _groundMask = value;
        }

        /// <summary>
        /// Layers to be considered during collision detection.
        /// </summary>

        public LayerMask collisionMask { get; protected set; }

        /// <summary>
        /// Determines if Character should collide with triggers.
        /// </summary>

        public bool collideWithTriggers
        {
            get => _collideWithTriggers;
            set => _collideWithTriggers = value;
        }

        /// <summary>
        /// The Character's current position, this complies with the interpolation settings.
        /// Setting it result in a smooth transition between the two positions in any intermediate frames rendered.
        /// </summary>

        public Vector3 position
        {
            get => _rigidbody.position;
            set => _rigidbody.MovePosition(value);
        }

        /// <summary>
        /// The Character's current rotation, this complies with the interpolation settings.
        /// Setting it result in a smooth transition between the two rotations in any intermediate frames rendered.
        /// </summary>

        public Quaternion rotation
        {
            get => _rigidbody.rotation;
            set => _rigidbody.MoveRotation(value);
        }

        /// <summary>
        /// The Rigidbody pre internal physics update velocity.
        /// </summary>

        protected Vector3 lastRigidbodyVelocity { get; set; }

        /// <summary>
        /// The Character's current velocity.
        /// </summary>

        public Vector3 velocity { get; set; }

        /// <summary>
        /// The velocity of the platform the Character is standing on,
        /// zero (Vector3.zero) if not on a platform.
        /// </summary>

        public Vector3 activePlatformVelocity
        {
            get => _activePlatformVelocity;
            protected set => _activePlatformVelocity = value;
        }

        /// <summary>
        /// Was the Character on ground the last frame ?
        /// </summary>

        public bool wasOnGround => _lastGroundHitResult.hitGround;

        /// <summary>
        /// Was the Character on walkable ground the last frame ?
        /// </summary>

        public bool wasOnWalkableGround => _lastGroundHitResult.hitWalkableGround;

        /// <summary>
        /// Is the Character on ground ?
        /// </summary>

        public bool isOnGround => _groundHitResult.hitGround;

        /// <summary>
        /// Is the Character on walkable ground ?
        /// </summary>

        public bool isOnWalkableGround => _groundHitResult.hitWalkableGround;

        /// <summary>
        /// Current ground detection test result.
        /// </summary>

        public ref GroundHit groundHit => ref _groundHitResult;

        /// <summary>
        /// PhysicsVolume overlapping this component. NULL if none.
        /// </summary>

        public PhysicsVolume physicsVolume { get; set; }

        #endregion

        #region INTERFACES

        /// <summary>
        /// Determines if the given collider should be filtered (eg: ignored).
        /// Return true if should be filtered, false otherwise.
        /// </summary>

        bool IColliderFilter.Filter(Collider otherCollider)
        {
            return IgnoreCollider(otherCollider);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Helper function to create a capsule of given dimensions.
        /// </summary>
        
        protected static void MakeCapsule(float radius, float height, out Vector3 center, out Vector3 bottomCenter, out Vector3 topCenter)
        {
            radius = Mathf.Max(radius, 0.0f);
            height = Mathf.Max(height, radius * 2.0f);

            center = height * 0.5f * Vector3.up;

            float sideHeight = height - radius * 2.0f;

            bottomCenter = center - sideHeight * 0.5f * Vector3.up;
            topCenter = center + sideHeight * 0.5f * Vector3.up;
        }

        /// <summary>
        /// Determines if the given collider should be filtered (eg: ignored).
        /// Return true if filtered, false otherwise.
        /// </summary>

        protected virtual bool IgnoreCollider(Collider otherCollider)
        {
            return otherCollider == capsuleCollider || _ignoredColliders.Contains(otherCollider);
        }

        /// <summary>
        /// Makes the collision detection system ignore all collisions between Character's capsule collider and otherCollider.
        /// </summary>
        
        public virtual void IgnoreCollision(Collider otherCollider, bool ignore = true)
        {
            if (otherCollider == null)
                return;

            if (ignore && _ignoredColliders.Add(otherCollider))
                Physics.IgnoreCollision(capsuleCollider, otherCollider, true);
            else if (_ignoredColliders.Remove(otherCollider))
                Physics.IgnoreCollision(capsuleCollider, otherCollider, false);
        }

        /// <summary>
        /// Should collision detection be enabled ?
        /// </summary>

        public virtual void DetectCollisions(bool detectCollisions)
        {
            _detectCollisions = detectCollisions;

            rigidbody.detectCollisions = _detectCollisions;
        }

        /// <summary>
        /// Casts a ray, from point origin, in direction direction, of length distance, against specified colliders (by layerMask) in the Scene.
        /// </summary>
        
        protected bool Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask,
            out RaycastHit closestHit, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            return CollisionDetection.Raycast(origin, direction, distance, layerMask, triggerInteraction,
                out closestHit, _hits, this) > 0;
        }

        /// <summary>
        /// Casts a capsule against specified colliders (by layerMask) in the Scene and returns detailed information on what was hit.
        /// </summary>

        protected int CapsuleCast(Vector3 characterPosition, Quaternion characterRotation, Vector3 direction,
            float distance, int layerMask, out RaycastHit closestHit, RaycastHit[] hits, float backstepDistance,
            QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            Vector3 top = characterPosition + characterRotation * _capsuleTopCenter;
            Vector3 bottom = characterPosition + characterRotation * _capsuleBottomCenter;

            return CollisionDetection.CapsuleCast(bottom, top, capsuleRadius, direction, distance, layerMask,
                triggerInteraction, out closestHit, hits, this, backstepDistance);
        }

        /// <summary>
        /// Casts a capsule against specified colliders (by layerMask) in the Scene and returns detailed information on what was hit.
        /// Unlike the previous method, this allows to set a custom capsule dimensions.
        /// </summary>
        
        protected int CapsuleCast(Vector3 characterPosition, Quaternion characterRotation, float radius, float height,
            Vector3 direction, float distance, int layerMask, out RaycastHit closestHit, RaycastHit[] hits,
            float backstepDistance, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            MakeCapsule(radius, height, out Vector3 _, out Vector3 bottomCenter, out Vector3 topCenter);

            Vector3 top = characterPosition + characterRotation * topCenter;
            Vector3 bottom = characterPosition + characterRotation * bottomCenter;

            return CollisionDetection.CapsuleCast(bottom, top, radius, direction, distance, layerMask,
                triggerInteraction, out closestHit, hits, this, backstepDistance);
        }

        /// <summary>
        /// Check the given capsule against the physics world and return all overlapping colliders.
        /// </summary>
        
        protected int OverlapTest(Vector3 characterPosition, Quaternion characterRotation, int layerMask,
            Collider[] results, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            Vector3 top = characterPosition + characterRotation * _capsuleTopCenter;
            Vector3 bottom = characterPosition + characterRotation * _capsuleBottomCenter;

            return CollisionDetection.OverlapCapsule(bottom, top, capsuleRadius, layerMask, triggerInteraction, results, this);
        }

        /// <summary>
        /// Check the given capsule against the physics world and return all overlapping colliders.
        /// </summary>
        
        protected int OverlapCapsule(Vector3 characterPosition, Quaternion characterRotation, float radius, float height,
            int layerMask, Collider[] results, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            MakeCapsule(radius, height, out Vector3 _, out Vector3 bottomCenter, out Vector3 topCenter);

            Vector3 top = characterPosition + characterRotation * topCenter;
            Vector3 bottom = characterPosition + characterRotation * bottomCenter;

            return CollisionDetection.OverlapCapsule(bottom, top, radius, layerMask, triggerInteraction, results, this);
        }

        /// <summary>
        /// Tests if the Character would collide with anything, if it was moved through the Scene.
        /// </summary>
        
        public bool SweepTest(Vector3 direction, float distance, int layerMask, out RaycastHit closestHit,
            QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            return CapsuleCast(position, rotation, direction, distance, layerMask, out closestHit, _hits, 0.002f, triggerInteraction) > 0;
        }

        /// <summary>
        /// Check the Character's capsule-shaped volume against the physics world and return all overlapping colliders.
        /// </summary>

        public Collider[] OverlapTest(int layerMask, out int overlapCount, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            overlapCount = OverlapTest(position, rotation, layerMask, _overlaps, triggerInteraction);
            return overlapCount > 0 ? _overlaps : null;
        }

        /// <summary>
        /// Checks if any colliders overlap the Character's capsule-shaped volume in world space.
        /// </summary>

        public bool CheckCapsule(float radius, float height, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int overlapCount = OverlapCapsule(position, rotation, radius, height, groundMask & collisionMask, _overlaps, triggerInteraction);
            return overlapCount > 0;
        }

        /// <summary>
        /// Set capsule volume dimensions.
        /// Compute internal data, use this to modify capsule volume.
        /// </summary>

        public virtual void SetCapsuleDimensions(float radius, float height)
        {
            _capsuleRadius = Mathf.Max(radius, 0.0f);
            _capsuleHeight = Mathf.Max(height, radius * 2.0f);

            MakeCapsule(_capsuleRadius, _capsuleHeight, out _capsuleCenter, out _capsuleBottomCenter, out _capsuleTopCenter);

            capsuleCollider.radius = _capsuleRadius;
            capsuleCollider.height = _capsuleHeight;
            capsuleCollider.center = _capsuleCenter;
        }

        /// <summary>
        /// Set the capsule volume height.
        /// Compute internal data, use this to modify capsule height.
        /// </summary>

        public virtual void SetCapsuleHeight(float height)
        {
            _capsuleHeight = Mathf.Max(height, _capsuleRadius * 2.0f);

            MakeCapsule(_capsuleRadius, _capsuleHeight, out _capsuleCenter, out _capsuleBottomCenter, out _capsuleTopCenter);

            capsuleCollider.height = _capsuleHeight;
            capsuleCollider.center = _capsuleCenter;
        }

        /// <summary>
        /// Is the Character currently constrained to a plane ?
        /// </summary>

        public virtual bool IsConstrainedToPlane()
        {
            return _constrainToPlane != PlaneConstraint.None;
        }

        /// <summary>
        /// Defines the axis that constraints movement, so movement along the given axis is not possible.
        /// </summary>

        public virtual void ConstrainToPlane(PlaneConstraint axis)
        {
            _constrainToPlane = axis;

            switch (_constrainToPlane)
            {
                case PlaneConstraint.None:
                {
                    _planeConstraintNormal = Vector3.zero;
                    rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                    
                    break;
                }

                case PlaneConstraint.ConstraintXAxis:
                {
                    _planeConstraintNormal = Vector3.right;
                    rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;

                    break;
                }

                case PlaneConstraint.ConstraintYAxis:
                {
                    _planeConstraintNormal = Vector3.up;
                    rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

                    break;
                }

                case PlaneConstraint.ConstraintZAxis:
                {
                    _planeConstraintNormal = Vector3.forward;
                    rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

                    break;
                }
            }
        }

        /// <summary>
        /// Returns the given vector constrained to current constraint plane (if _constrainToPlane != None)
        /// or given vector (if _constrainToPlane == None).
        /// </summary>

        public virtual Vector3 ConstrainDirectionToPlane(Vector3 vector)
        {
            return _constrainToPlane == PlaneConstraint.None ? vector : vector.projectedOnPlane(_planeConstraintNormal);
        }

        /// <summary>
        /// Is the Character constrained to walkable ground ?
        /// </summary>
        
        public virtual bool IsConstrainedToGround()
        {
            return _isConstrainedToGround && _unconstrainedTimer <= 0.0f;
        }

        /// <summary>
        /// Should movement be constrained to ground when on walkable ground ?
        /// Toggles ground constraint. 
        /// </summary>

        public virtual void ConstrainToGround(bool constrainToGround)
        {
            if (constrainToGround)
            {
                _isConstrainedToGround = true;
                _unconstrainedTimer = 0.0f;
            }
            else
            {
                _isConstrainedToGround = false;
                _unconstrainedTimer = 0.0f;

                _probingDistance = kMinGroundProbingDistance;
            }
        }

        /// <summary>
        /// Temporarily disable ground constraint allowing the Character to freely leave the ground.
        /// Eg: LaunchCharacter, Jump, etc.
        /// </summary>

        public virtual void PauseGroundConstraint(float unconstrainedTime = 0.1f)
        {
            _unconstrainedTimer = unconstrainedTime;
            _probingDistance = kMinGroundProbingDistance;
        }

        /// <summary>
        /// Compute distance to the ground from bottom sphere of capsule and store the result in outGroundResult.
        /// This distance is the swept distance of the capsule to the first point impacted by the lower hemisphere,
        /// or distance from the bottom of the capsule in the case of a raycast (rayLength > 0.0f).
        /// </summary>

        public virtual bool ComputeGroundDistance(Vector3 characterPosition, Quaternion characterRotation,
            float sweepDistance, float sweepRadius, out GroundDistance distanceResult, float rayLength = 0.0f)
        {
            distanceResult = default;

            Vector3 characterUp = characterRotation * Vector3.up;
            
            Vector3 sweepOrigin = characterPosition;
            Quaternion sweepRotation = characterRotation;

            sweepRadius = Mathf.Clamp(sweepRadius, kContactOffset, capsuleRadius);

            Vector3 sweepDirection = -characterUp;
            
            int sweepLayerMask = groundMask;

            QueryTriggerInteraction sweepTriggerInteraction =
                collideWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            int hitCount = CapsuleCast(sweepOrigin, sweepRotation, sweepRadius, capsuleHeight, sweepDirection,
                sweepDistance + kContactOffset, sweepLayerMask, out RaycastHit sweepHitResult, _hits, kStepSweepBackstepDistance, sweepTriggerInteraction);

            if (hitCount > 0)
            {
                Vector3 positionOnGround = sweepOrigin + sweepDirection * sweepDistance + characterUp * kContactOffset;

                distanceResult = new GroundDistance
                {
                    isWalkable = Vector3.Dot(sweepHitResult.normal, characterUp) > _minSlopLimit,

                    sweepDistance = Vector3.Distance(characterPosition, positionOnGround),

                    sweepPosition = positionOnGround,

                    hitResult = sweepHitResult
                };

                return true;
            }
            
            if (rayLength > 0.0f)
            {
                bool hit = Raycast(sweepOrigin, sweepDirection, rayLength + kContactOffset, sweepLayerMask, out RaycastHit raycastHitResult, sweepTriggerInteraction);
                if (hit)
                {
                    distanceResult = new GroundDistance
                    {
                        isWalkable = Vector3.Dot(raycastHitResult.normal, characterUp) > _minSlopLimit,

                        isRaycast = true,
                        raycastDistance = raycastHitResult.distance,

                        hitResult = raycastHitResult
                    };

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Compute the location of the hit WRT the capsule. Eg: Top, Bottom, Sides.
        /// </summary>
        
        public virtual CapsuleHitLocation ComputeCapsuleHitLocation(Vector3 characterPosition,
            Quaternion characterRotation, Vector3 inNormal)
        {
            // Mario 64 world classification into ground, walls and ceiling.
            // source: https://youtu.be/UnU7DJXiMAQ

            Vector3 characterUp = characterRotation * Vector3.up;
            float verticalComponent = Vector3.Dot(inNormal, characterUp);

            if (verticalComponent > 0.01f)
                return CapsuleHitLocation.Bottom;
            
            if (verticalComponent < -0.01f)
                return CapsuleHitLocation.Top;

            return CapsuleHitLocation.Sides;
        }
        
        /// <summary>
        /// Attempts to detect a ledge at given position.
        /// It is considered a ledge if found no ground, found no walkable ground, or found any ground farther than step offset.
        /// </summary>

        protected virtual void ComputeLedgeHit(Vector3 characterPosition, Quaternion characterRotation,
            ref RaycastHit inHitResult, out LedgeHit outLedgeHitResult)
        {
            // Compute 'edge' sides hits

            Vector3 characterUp = characterRotation * Vector3.up;
            Vector3 projectedNormal = inHitResult.normal.projectedOnPlane(characterUp).normalized;

            Vector3 rayOrigin = inHitResult.point + characterUp * kRaycastVerticalOffset;
            Vector3 rayDirection = -characterUp;

            float rayLength = kMinHeightForLedge + kRaycastVerticalOffset;

            int layerMask = groundMask;

            QueryTriggerInteraction triggerInteraction =
                collideWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            bool farHit = Raycast(rayOrigin - projectedNormal * kRaycastHorizontalOffset, rayDirection, rayLength,
                layerMask, out RaycastHit farHitResult, triggerInteraction);

            bool isFarHitWalkable = farHit && Vector3.Dot(farHitResult.normal, characterUp) > _minSlopLimit;

            bool nearHit = Raycast(rayOrigin + projectedNormal * kRaycastHorizontalOffset, rayDirection, rayLength,
                layerMask, out RaycastHit nearHitResult, triggerInteraction);

            bool isNearHitWalkable = nearHit && Vector3.Dot(nearHitResult.normal, characterUp) > _minSlopLimit;

            bool isLedge = isNearHitWalkable ^ isFarHitWalkable;
            if (!isLedge)
            {
                outLedgeHitResult = default;
                return;
            }

            // Found a ledge,
            // eg: no ground, no walkable ground, or any ground farther than kMinHeightForLedge
            
            // Compute output result

            Vector3 ledgeUp = isNearHitWalkable ? nearHitResult.normal : farHitResult.normal;
            Vector3 ledgeRight = inHitResult.normal.perpendicularTo(ledgeUp);
            Vector3 ledgeForward = ledgeUp.perpendicularTo(ledgeRight);

            bool isOnEmptySide = isFarHitWalkable;
            float ledgeDistance = (inHitResult.point - characterPosition).projectedOnPlane(characterUp).magnitude;

            bool isWalkingOff = wasOnGround && Vector3.Dot(velocity, ledgeForward.projectedOnPlane(characterUp)) > 0.0f;

            outLedgeHitResult = new LedgeHit
            {
                isLedge = true,

                isWalkingOff = isWalkingOff,
                isOnEmptySide = isOnEmptySide,

                ledgeDistance = ledgeDistance,

                ledgeNormal = ledgeUp,
                ledgeForward = ledgeForward
            };
        }

        /// <summary>
        /// Determines if Character can stand on a ledge by stepOffset + perchAdditionalHeight.
        /// </summary>

        protected virtual void ComputePerchResult(Vector3 characterPosition, Quaternion characterRotation,
            Vector3 inPoint, Vector3 inNormal, out bool perchResult)
        {
            Vector3 characterUp = characterRotation * Vector3.up;
            Vector3 projectedNormal = inNormal.projectedOnPlane(characterUp).normalized;

            Vector3 rayOrigin = inPoint + projectedNormal * Mathf.Max(kRaycastHorizontalOffset, perchOffset);
            Vector3 rayDirection = -characterUp;

            float rayLength = Mathf.Max(kMinHeightForLedge, stepOffset) + perchAdditionalHeight;
            float actualRayLength = rayLength + capsuleRadius;

            QueryTriggerInteraction triggerInteraction =
                collideWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            bool nearHit = Raycast(rayOrigin, rayDirection, actualRayLength, groundMask, out RaycastHit nearHitResult, triggerInteraction);

            bool isNearHitWalkable = nearHit && nearHitResult.distance <= rayLength &&
                                     Vector3.Dot(nearHitResult.normal, characterUp) > _minSlopLimit;

            perchResult = isNearHitWalkable;
        }

        /// <summary>
        /// Attempts to detect any step.
        /// Eg: Walkable steps (height less than capsule radius) or climbable steps (height higher than capsule radius).
        /// </summary>        

        protected virtual void ComputeStepHit(Vector3 characterPosition, Quaternion characterRotation,
            Vector3 displacement, ref RaycastHit inHitResult, out StepHit outStepHitResult)
        {
            outStepHitResult = default;

            // Cant step up if hit is above us

            CapsuleHitLocation hitLocation =
                ComputeCapsuleHitLocation(characterPosition, characterRotation, inHitResult.normal);

            if (hitLocation == CapsuleHitLocation.Top)
                return;

            // Check if Character can step up...

            bool canStepUp = _callbackTarget.CanStepUp(ref inHitResult);
            if (!canStepUp)
                return;

            // Downcast from FAR hit point at stepOffset height (otherwise MeshColliders using Fast Midphase algorithm could fail)

            Vector3 characterUp = characterRotation * Vector3.up;
            Vector3 projectedNormal = inHitResult.normal.projectedOnPlane(characterUp).normalized;

            Vector3 sweepOrigin = 
                MathLib.ProjectPointOnPlane(inHitResult.point, characterPosition, characterUp) + characterUp * stepOffset - projectedNormal * kRaycastHorizontalOffset;

            Quaternion sweepRotation = characterRotation;

            Vector3 sweepDirection = -characterUp;

            float sweepDistance = stepOffset;

            QueryTriggerInteraction triggerInteraction =
                collideWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;            

            int hitCount = CapsuleCast(sweepOrigin, sweepRotation, sweepDirection, sweepDistance + kContactOffset,
                groundMask, out RaycastHit stepHitResult, _hits, kStepSweepBackstepDistance, triggerInteraction);
            if (hitCount == 0)
                return;

            // Do we have any room on top of detected step ?

            Vector3 positionOnStep =
                sweepOrigin + sweepDirection * stepHitResult.distance + characterUp * kContactOffset;

            int overlapCount = OverlapTest(positionOnStep, sweepRotation, groundMask & collisionMask, _overlaps,
                triggerInteraction);
            if (overlapCount != 0)
                return;

            // Can step on top of this ?

            canStepUp = _callbackTarget.CanStepUp(ref stepHitResult);
            if (!canStepUp)
                return;
            
            // Is step walkable ?            

            Vector3 rayOrigin = stepHitResult.point + characterUp * kRaycastVerticalOffset;

            Vector3 rayDirection = sweepDirection;

            float rayLength = stepOffset + kRaycastVerticalOffset;

            bool farHit = stepHitResult.collider.Raycast(new Ray(rayOrigin, rayDirection), out RaycastHit farHitResult, rayLength);
            bool isFarHitWalkable = farHit && Vector3.Dot(farHitResult.normal, characterUp) > _minSlopLimit;
            if (!isFarHitWalkable)
                return;
            
            // Will have walkable ground below us if step up ?

            // Downcast from in-hit near point at step height

            Vector3 nearPoint = stepHitResult.point +
                                (inHitResult.point - stepHitResult.point).projectedOn(projectedNormal);

            rayOrigin = nearPoint + projectedNormal * 2.0f * kRaycastHorizontalOffset + characterUp * kRaycastVerticalOffset;

            rayLength = stepOffset + perchAdditionalHeight + kRaycastVerticalOffset;

            bool nearHit = Raycast(rayOrigin, rayDirection, rayLength, groundMask, out RaycastHit nearHitResult, triggerInteraction);
            bool isNearHitWalkable = nearHit && Vector3.Dot(nearHitResult.normal, characterUp) > _minSlopLimit;
            if (!isNearHitWalkable)
            {
                // Are we on top ?
                
                // Downcast at step-hit near point

                rayOrigin = stepHitResult.point + projectedNormal * kRaycastHorizontalOffset * 2.0f + characterUp * kRaycastVerticalOffset;

                rayLength = stepOffset + perchAdditionalHeight + kRaycastVerticalOffset;

                nearHit = Raycast(rayOrigin, rayDirection, rayLength, groundMask, out nearHitResult, triggerInteraction);

                isNearHitWalkable = nearHit && Vector3.Dot(nearHitResult.normal, characterUp) > _minSlopLimit;
                if (!isNearHitWalkable)
                    return;

            }

            // Is a climbable step ? (eg: its height is higher than capsule radius)

            bool isClimbable = hitLocation == CapsuleHitLocation.Sides;
            if (isClimbable)
            {
                // On climbable step, compute Character's position on step accounting its current displacement...

                // Move forward (at step height)

                sweepOrigin = characterPosition + characterUp * Mathf.Max(0.0f, Vector3.Dot(stepHitResult.point - characterPosition, characterUp));

                displacement = displacement.projectedOnPlane(characterUp);

                sweepDirection = displacement.normalized;
                sweepDistance = displacement.magnitude;

                hitCount = CapsuleCast(sweepOrigin, sweepRotation, sweepDirection, sweepDistance,
                    groundMask & collisionMask, out stepHitResult, _hits, kDisplacementSweepBackstepDistance, triggerInteraction);

                if (hitCount == 0)
                {
                    // No hit, apply full displacement

                    sweepOrigin += displacement;
                }
                else
                {
                    // On hit, move as far as we can

                    Vector3 displacementToHit = sweepDirection * stepHitResult.distance + stepHitResult.normal * kContactOffset;

                    sweepOrigin += displacementToHit;
                }

                // Move down

                sweepDirection = -characterUp;
                sweepDistance = stepOffset;

                hitCount = CapsuleCast(sweepOrigin, sweepRotation, sweepDirection, sweepDistance, groundMask,
                    out stepHitResult, _hits, kGroundSweepBackstepDistance, triggerInteraction);

                if (hitCount == 0)
                    return;

                positionOnStep = sweepOrigin + sweepDirection * stepHitResult.distance + characterUp * kContactOffset;

            }

            // Output step hit result

            outStepHitResult = new StepHit
            {
                isStep = true,
                isClimbable = isClimbable,

                position = positionOnStep,

                point = stepHitResult.point,
                normal = stepHitResult.normal,

                collider = stepHitResult.collider
            };
        }


        /// <summary>
        /// 'Parse' the hit info at given position looking for walkable ground.
        /// This will detect ledges, walkable steps (height less than capsule radius)
        /// and / or climbable steps (height higher than capsule radius).
        /// </summary>

        protected virtual void TestWalkableGround(Vector3 characterPosition, Quaternion characterRotation,
            Vector3 displacement, ref RaycastHit inHitResult, out GroundHit outGroundHitResult)
        {
            CapsuleHitLocation hitLocation =
                ComputeCapsuleHitLocation(characterPosition, characterRotation, inHitResult.normal);

            Vector3 characterUp = characterRotation * Vector3.up;
            bool isWalkable = hitLocation == CapsuleHitLocation.Bottom &&
                              Vector3.Dot(inHitResult.normal, characterUp) > _minSlopLimit;

            // Only care for bottom hits here...

            if (hitLocation == CapsuleHitLocation.Bottom)
            {
                // Is Character standing on a ledge ?

                ComputeLedgeHit(characterPosition, characterRotation, ref inHitResult, out LedgeHit ledgeHitResult);
                {
                    // On ledge empty side ?

                    if (ledgeHitResult.isLedge && ledgeHitResult.isOnEmptySide)
                    {
                        // Can stand by perchOffset ?

                        isWalkable = ledgeHitResult.ledgeDistance <= perchOffset && IsConstrainedToGround();

                        // Should unperch from ledge ?

                        if (ledgeHitResult.isWalkingOff && ledgeHitResult.ledgeDistance > unperchOffset)
                            isWalkable = false;

                        // Can stand by stepOffset + perchAdditionalHeight ?

                        if (!isWalkable && IsConstrainedToGround())
                        {
                            ComputePerchResult(characterPosition, characterRotation, inHitResult.point,
                                ledgeHitResult.ledgeForward, out isWalkable);
                        }

                        outGroundHitResult = new GroundHit
                        {
                            hitLocation = hitLocation,

                            hitGround = true,
                            isWalkable = isWalkable,

                            position = characterPosition,

                            point = inHitResult.point,
                            normal = inHitResult.normal,

                            collider = inHitResult.collider
                        };

                        return;
                    }
                }
            }

            // If no walkable, could be a step or climbable step

            StepHit stepHitResult = default;
            if (!isWalkable && wasOnGround && IsConstrainedToGround())
            {
                ComputeStepHit(characterPosition, characterRotation, displacement, ref inHitResult, out stepHitResult);
                {
                    if (stepHitResult.isStep || stepHitResult.isClimbable)
                        isWalkable = true;
                }
            }

            // Output test result

            outGroundHitResult = new GroundHit
            {
                hitLocation = hitLocation,

                hitGround = hitLocation == CapsuleHitLocation.Bottom || stepHitResult.isClimbable,
                isWalkable = isWalkable,

                position = characterPosition,

                point = inHitResult.point,
                normal = inHitResult.normal,

                collider = inHitResult.collider,

                stepHitResult = stepHitResult
            };
        }

        /// <summary>
        /// Sweep along Character's down vector looking for walkable 'ground'.
        /// </summary>
        
        public virtual void ComputeGroundHit(Vector3 characterPosition, Quaternion characterRotation,
            float sweepDistance, out GroundHit outGroundHitResult)
        {
            outGroundHitResult = default;

            // Downcast from our current position against 'ground'

            Vector3 sweepOrigin = characterPosition;
            Quaternion sweepRotation = characterRotation;

            Vector3 characterUp = sweepRotation * Vector3.up;
            Vector3 sweepDirection = -characterUp;

            QueryTriggerInteraction triggerInteraction =
                collideWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            int hitCount = CapsuleCast(sweepOrigin, sweepRotation, sweepDirection, sweepDistance, groundMask.value,
                out RaycastHit hitResult, _hits, kGroundSweepBackstepDistance, triggerInteraction);

            if (hitCount == 0)
                return;

            // Move up to near contact point

            Vector3 positionOnGround = sweepOrigin + sweepDirection * hitResult.distance + characterUp * kContactOffset;

            // Test if hit is walkable

            TestWalkableGround(positionOnGround, characterRotation, Vector3.zero, ref hitResult, out outGroundHitResult);
            {
                // Found walkable ground, we done

                if (outGroundHitResult.hitWalkableGround)
                    return;
            }
            
            // No walkable found (mostly flushed against non-walkable ground). Try to retrieve the walkable ground below us,
            // casting (from ~our current position) down along the non-walkable surface, while maintaining contact offset separation

            if (hitResult.distance < kContactOffset)
                sweepOrigin += sweepDirection * hitResult.distance + characterUp * kContactOffset;

            Vector3 hitNormal = hitResult.normal;
            sweepDirection = hitNormal.perpendicularTo(sweepDirection).perpendicularTo(hitNormal);

            sweepDistance = kMinHeightForLedge;

            hitCount = CapsuleCast(sweepOrigin, sweepRotation, sweepDirection, sweepDistance, groundMask, out hitResult,
                _hits, kGroundSweepBackstepDistance, triggerInteraction);

            if (hitCount == 0)
                return;

            // Move up to near contact point and test if walkable

            positionOnGround = sweepOrigin + sweepDirection * hitResult.distance - sweepDirection * kContactOffset;

            TestWalkableGround(positionOnGround, characterRotation, Vector3.zero, ref hitResult, out outGroundHitResult);
        }

        /// <summary>
        /// Perform ground detection and apply ground constraint (if enabled).
        /// </summary>

        protected virtual void PerformGroundDetection()
        {
            _lastGroundHitResult = _groundHitResult;
            _groundHitResult = default;

            ComputeGroundHit(_probingPosition, _probingRotation, _probingDistance, out _groundHitResult);
            {
                // Is constrained to ground ?

                if (IsConstrainedToGround())
                {
                    // On walkable ground, apply ground constrain (aka: snap to ground)

                    bool hitWalkableGround = _groundHitResult.hitWalkableGround;
                    if (hitWalkableGround)
                        _probingPosition = _groundHitResult.position;

                    // If standing on a dynamic rigidbody, apply downwards force (delegated to controller)

                    if (hitWalkableGround)
                    {
                        Rigidbody otherRigidbody = _groundHitResult.rigidbody;
                        if (otherRigidbody && !otherRigidbody.isKinematic)
                            _callbackTarget.OnApplyStandingDownwardForce(otherRigidbody);
                    }

                    // Choose appropriate probing distance for current state

                    _probingDistance = hitWalkableGround
                        ? Mathf.Max(groundProbingDistance, stepOffset)
                        : kMinGroundProbingDistance;

                }
                else if (_unconstrainedTimer > 0.0f)
                {
                    // If temporarily unconstrained to ground, update timer

                    _unconstrainedTimer -= Time.deltaTime;
                    if (_unconstrainedTimer <= 0.0f)
                        _unconstrainedTimer = 0.0f;
                }

                // If hit ground, trigger GroundHit 'event'

                bool hitGround = _groundHitResult.hitGround;
                if (hitGround)
                    _callbackTarget.OnGroundHit(ref _lastGroundHitResult, ref _groundHitResult);
            }
        }

        /// <summary>
        /// Handle when Character is standing on a moving 'platform'.
        /// Eg: Computes platform velocity, impart platform's velocity when character leaves, etc.
        /// </summary>

        protected virtual void UpdateMovingPlatform()
        {
            // Keep track of the moving platform the character is interacting with

            if (groundHit.collider && groundHit.collider != _recordedPlatform)
                _recordedPlatform = groundHit.collider;

            if (_recordedPlatform)
            {
                Rigidbody attachedRigidbody = _recordedPlatform.attachedRigidbody;
                if (attachedRigidbody == null || !attachedRigidbody.isKinematic)
                    _recordedPlatform = null;
            }

            // If character is on a moving platform, compute active platform velocity

            _lastActivePlatform = _activePlatform;
            _activePlatform = groundHit.hitGround && groundHit.rigidbody ? groundHit.collider : null;

            _lastActivePlatformVelocity = _activePlatformVelocity;
            _activePlatformVelocity = _activePlatform
                ? _activePlatform.attachedRigidbody.getVelocityAtPoint(_probingPosition)
                : Vector3.zero;

            // If not allowed to move when Character is on this active platform, reset active platform velocity

            if (_activePlatform &&
                !_callbackTarget.ShouldMoveCharacterWhenStandingOn(_activePlatform.attachedRigidbody))
            {
                _lastActivePlatformVelocity =
                    _activePlatformVelocity = Vector3.zero;
            }

            // If character has left a moving platform...
            
            if (_lastActivePlatform && _activePlatform != _lastActivePlatform)
            {
                // Cancel other platform (eg: character changed platform) velocity

                velocity -= _activePlatformVelocity;

                // Impart platform velocity, delegated to the target

                _callbackTarget.ImpartPlatformVelocity(_lastActivePlatformVelocity);
            }

            // If character 'landed' on a moving platform, cancel its velocity

            if (_lastActivePlatform == null && _activePlatform)
                velocity -= _activePlatformVelocity;
        }

        /// <summary>
        /// Sweeps the capsule volume along its displacement vector, stopping at near hit point if collision is detected or applies full displacement if not.
        /// Return remaining displacement (unmodified)
        /// </summary>

        protected virtual Vector3 ComputeMovementHit(Vector3 displacement, out MovementHit outMovementHitResult)
        {
            outMovementHitResult = default;

            // Do not sweep if displacement is minimal

            float sweepDistance = displacement.magnitude;
            if (sweepDistance < kMinMovementDistance)
                return Vector3.zero;

            // Sweep along displacement

            Vector3 sweepDirection = displacement / sweepDistance;

            int sweepLayerMask = groundMask & collisionMask;

            QueryTriggerInteraction triggerInteraction =
                collideWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            int hitCount = CapsuleCast(_probingPosition, _probingRotation, sweepDirection,
                sweepDistance + kContactOffset, sweepLayerMask, out RaycastHit hitResult, _hits,
                kDisplacementSweepBackstepDistance, triggerInteraction);

            if (hitCount == 0)
            {
                // No hit, apply full displacement

                _probingPosition += displacement;
            }
            else
            {
                // On sweep hit...
                
                // Move up to near contact point

                Vector3 displacementToHit = sweepDirection * hitResult.distance + hitResult.normal * kContactOffset;

                _probingPosition += displacementToHit;

                // Compute remaining displacement

                Vector3 remainingDisplacement = sweepDirection * (displacement - displacementToHit).magnitude;

                // Test if hit is walkable
                
                TestWalkableGround(_probingPosition, _probingRotation, displacement, ref hitResult, out GroundHit groundHitResult);
                {
                    // Is hit a climbable step ?

                    StepHit stepHitResult = groundHitResult.stepHitResult;
                    if (stepHitResult.isClimbable)
                    {
                        // Yes, step up and ignore this contact as it was a resolved as a step up

                        Physics.IgnoreCollision(capsuleCollider, stepHitResult.collider);

                        _probingPosition = stepHitResult.position;

                        Vector3 characterUp = _probingRotation * Vector3.up;
                        velocity = velocity.projectedOnPlane(characterUp);

                        return Vector3.zero;
                    }

                    // If hit non-walkable...

                    if (!groundHitResult.hitWalkableGround && isOnWalkableGround && IsConstrainedToGround())
                    {
                        // Modify hit normal to better constrain the character's movement,
                        // eg: prevent to climb non-walkable surfaces

                        Vector3 groundNormal = groundHit.normal;
                        Vector3 hitNormal = groundHitResult.normal;

                        Vector3 characterUp = _probingRotation * Vector3.up;
                        hitResult.normal = groundNormal.perpendicularTo(hitNormal).perpendicularTo(characterUp);
                    }

                    // Did we hit other rigidbody ?

                    Rigidbody otherRigidbody = hitResult.rigidbody;
                    if (otherRigidbody)
                    {
                        // Is dynamic rigidbody ?

                        if (!otherRigidbody.isKinematic)
                        {
                            // Generate RigidbodyHit result

                            RigidbodyHit rigidbodyHit = new RigidbodyHit()
                            {
                                hitLocation =
                                    ComputeCapsuleHitLocation(_probingPosition, _probingRotation, hitResult.normal),

                                isWalkable = groundHitResult.isWalkable,

                                velocity = velocity,
                                otherVelocity = otherRigidbody.getVelocityAtPoint(hitResult.point),

                                point = hitResult.point,
                                normal = hitResult.normal,

                                collider = hitResult.collider
                            };

                            //  Add it to uniques list to be resolved later by the Character for better control

                            if (!_rigidbodyHits.Contains(rigidbodyHit))
                                _rigidbodyHits.Add(rigidbodyHit);
                        }
                        else
                        {
                            // Ignore hits against our recorded moving platform

                            if (groundHitResult.collider == _recordedPlatform)
                            {
                                _probingPosition += remainingDisplacement;

                                return Vector3.zero;
                            }

                            // Ignores blocking hits against kinematic rigidbody, let PhysX take care

                            if (!groundHitResult.hitWalkableGround)
                            {
                                _probingPosition += remainingDisplacement;

                                return Vector3.zero;
                            }
                        }
                    }                    

                    // Output movement hit result

                    outMovementHitResult = new MovementHit
                    {
                        hitLocation = groundHitResult.hitLocation,

                        hitGround = groundHitResult.hitGround,
                        isWalkable = groundHitResult.isWalkable,

                        displacement = displacement,
                        displacementToHit = displacementToHit,

                        point = hitResult.point,
                        normal = hitResult.normal,

                        collider = hitResult.collider
                    };
                }

                // Return remaining displacement

                return remainingDisplacement;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Performs collision constrained movement.
        /// This refers to the process of smoothly sliding a moving entity along any obstacles encountered.
        /// Updates _probingPosition.
        /// </summary>

        protected virtual void CollideAndSlide(Vector3 displacement)
        {
            _movementHitCount = 0;

            // If collision detection is disabled move without collision detection

            if (_detectCollisions == false)
            {
                _probingPosition += displacement;
                
                return;
            }

            // If not enough displacement, return

            if (displacement.sqrMagnitude <= MathLib.Square(kMinMovementDistance))
                return;
            
            Vector3 originalVelocity = velocity, prevNormal = Vector3.zero;
            for (int i = 0, iteration = 0; iteration < kMaxMovementIterations && i < kMaxHitCount; ++i)
            {
                // Sweep along given displacement

                displacement = ComputeMovementHit(displacement, out MovementHit movementHitResult);
                
                // No remaining displacement, we done

                if (displacement.isZero())
                    break;

                // On hit...

                // Register this movement hit to be triggered after internal physics update

                _movementHits[_movementHitCount++] = movementHitResult;

                // Resolve movement hit (compute slide direction)

                if (movementHitResult.hitWalkableGround && IsConstrainedToGround())
                {
                    // On walkable hit, resolve movement hit

                    displacement = _callbackTarget.ComputeCollisionResponseDisplacement(displacement, ref movementHitResult);

                    velocity = _callbackTarget.ComputeCollisionResponseDisplacement(velocity, ref movementHitResult);
                }
                else
                {
                    // On non-walkable hit...

                    switch (iteration)
                    {
                        case 0:

                            // Slide along non-walkable surface

                            displacement = _callbackTarget.ComputeCollisionResponseDisplacement(displacement, ref movementHitResult);

                            velocity = _callbackTarget.ComputeCollisionResponseDisplacement(velocity, ref movementHitResult);

                            prevNormal = movementHitResult.normal;

                            ++iteration;

                            break;

                        case 1:

                            // Two plane constrained, handle 'crease'
                            // source: https://www.gamedev.net/forums/topic/355676-motion-vectors-and-creases/3334148/

                            // Create a vector that follows the 'crease' between the two normals

                            Vector3 crease = prevNormal.perpendicularTo(movementHitResult.normal);

                            // Project the original and new velocities into the plane of the crease

                            Vector3 oVel = originalVelocity.projectedOnPlane(crease).normalized;

                            Vector3 nVel =
                                _callbackTarget.ComputeCollisionResponseDisplacement(velocity, ref movementHitResult);

                            nVel = nVel.projectedOnPlane(crease).normalized;

                            // If velocities are opposed or angle of corner is acute, restrict the velocity to the crease

                            if (Vector3.Dot(oVel, nVel) <= 0.0f || Vector3.Dot(prevNormal, movementHitResult.normal) < 0.0f)
                            {
                                displacement = displacement.projectedOn(crease);

                                velocity = velocity.projectedOn(crease);

                                ++iteration;
                            }
                            else
                            {
                                // Otherwise handle regular projection

                                displacement = _callbackTarget.ComputeCollisionResponseDisplacement(displacement, ref movementHitResult);

                                velocity = _callbackTarget.ComputeCollisionResponseDisplacement(velocity, ref movementHitResult);

                                prevNormal = movementHitResult.normal;
                            }

                            break;

                        case 2:

                            // Three plane constrained, we ran out of degrees of freedom

                            velocity = displacement = Vector3.zero;

                            ++iteration;

                            break;
                    }
                }

                // If not grounded and sweep found walkable ground, restore probing distance

                if (movementHitResult.hitWalkableGround && !isOnWalkableGround && IsConstrainedToGround())
                    _probingDistance = Mathf.Max(groundProbingDistance, stepOffset);
            }
        }

        /// <summary>
        /// Let the Character handle if and how push other rigidbodies.
        /// </summary>

        protected virtual void ApplyPushForce()
        {
            int hitCount = _rigidbodyHits.Count;
            if (hitCount == 0)
                return;

            for (int i = 0; i < hitCount; i++)
            {
                RigidbodyHit rigidbodyHit = _rigidbodyHits[i];
                if (rigidbodyHit.rigidbody != null)
                    _callbackTarget.OnApplyPushForce(ref rigidbodyHit);
            }

            _rigidbodyHits.Clear();
        }

        /// <summary>
        /// Perform Character's movement. Must be called once per frame on FixedUpdate.
        /// </summary>
        
        public virtual void Move()
        {
            // Initialize current probing pose for this move

            _probingPosition = position;
            _probingRotation = rotation;
            
            // Perform ground detection and apply ground constraint (if enabled)

            PerformGroundDetection();

            // Handle when character is on a moving platform (eg: kinematic rigidbody)
            
            UpdateMovingPlatform();
            
            // Let the Character determine how should move

            _callbackTarget.OnMove();

            // Constrain velocity to constraint plane

            velocity = ConstrainDirectionToPlane(velocity);

            // Perform collision constrained displacement

            Vector3 displacement = velocity * Time.deltaTime;
            
            CollideAndSlide(displacement);
            
            // Let the Character resolve hits found during movement
            // against other non-kinematic rigidbodies (including other characters)

            ApplyPushForce();

            // Compute output rigidbody velocity,
            // the required velocity to move the rigidbody to our computed _probingPosition factoring if standing on a moving platform.

            _rigidbody.velocity = ConstrainDirectionToPlane((_probingPosition - position) / Time.deltaTime + activePlatformVelocity);

            lastRigidbodyVelocity = _rigidbody.velocity;
        }

        /// <summary>
        /// Trigger MovementHit events for each hit found during Character movement (CollideAndSlide).
        /// This needed since we resolve future collisions, we cache hits and trigger after PhysX internal update, like regular OnCollisionXXX events.
        /// </summary>
        
        protected virtual void OnMovementHit()
        {
            for (int i = 0; i < _movementHitCount; i++)
                _callbackTarget.OnMovementHit(ref _movementHits[i]);

            _movementHitCount = 0;
        }

        /// <summary>
        /// Resolve overlaps against static and dynamic objects, after internal physics update.
        /// </summary>

        protected virtual void OverlapRecovery()
        {
            const float kSkinWidth = kContactOffset;

            capsuleCollider.radius = capsuleRadius + kSkinWidth;
            capsuleCollider.height = capsuleHeight + kSkinWidth * 2.0f;

            int overlapMask = groundMask.value & collisionMask.value;

            QueryTriggerInteraction triggerInteraction =
                collideWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            int overlapCount = OverlapTest(_probingPosition, _probingRotation, overlapMask, _overlaps, triggerInteraction);
            if (overlapCount > 0)
            {
                for (int i = 0; i < overlapCount; i++)
                {
                    Collider overlappedCollider = _overlaps[i];
                    Transform overlappedColliderTransform = overlappedCollider.transform;

                    if (Physics.ComputePenetration(capsuleCollider, _probingPosition, _probingRotation,
                        overlappedCollider, overlappedColliderTransform.position, overlappedColliderTransform.rotation,
                        out Vector3 direction, out float distance))
                    {
                        _probingPosition += ConstrainDirectionToPlane(direction) * distance;
                    }
                }
            }

            capsuleCollider.radius = capsuleRadius;
            capsuleCollider.height = capsuleHeight;
        }

        /// <summary>
        /// Compute and resolve overlaps (if any), after internal physics update.
        /// </summary>

        protected virtual void ResolveOverlaps()
        {
            // If collision detection is disabled, return

            if (_detectCollisions == false)
                return;

            for (int i = 0; i < kMaxOverlapTests; i++)
                OverlapRecovery();
        }

        /// <summary>
        /// Moves the character when standing on a non-kinematic rigidbody.
        /// </summary>

        protected virtual void MoveWithDynamicPlatform()
        {
            // If not on walkable ground, or should not move with ground, return

            if (!isOnWalkableGround || !IsConstrainedToGround())
                return;

            // If no rigidbody, or not dynamic, return

            Rigidbody otherRigidbody = _groundHitResult.rigidbody;
            if (!otherRigidbody || otherRigidbody.isKinematic)
                return;

            // Should move when standing over this Rigidbody ?
            
            bool shouldMoveCharacter = _callbackTarget.ShouldMoveCharacterWhenStandingOn(otherRigidbody);
            if (!shouldMoveCharacter)
                return;

            // Update Character's position

            Vector3 newPlatformVelocity = otherRigidbody.getVelocityAtPoint(_probingPosition);

            Vector3 deltaPlatformVelocity = newPlatformVelocity - activePlatformVelocity;

            _probingPosition += deltaPlatformVelocity * Time.deltaTime;
        }

        /// <summary>
        /// Appends platform rotation to Character's current rotation.
        /// Called in LateFixedUpdate.
        /// </summary>

        protected virtual void RotateWithPlatform()
        {
            // If not on walkable ground, or should not move with ground, return

            if (!isOnWalkableGround || !IsConstrainedToGround())
                return;

            // If not over a rigidbody, we done

            Rigidbody otherRigidbody = _groundHitResult.rigidbody;
            if (otherRigidbody == null)
                return;

            // Should rotate the Character ?

            bool shouldRotateCharacter = _callbackTarget.ShouldRotateCharacterWhenStandingOn(otherRigidbody);
            if (!shouldRotateCharacter)
                return;

            // Is actually rotating ?

            if (otherRigidbody.angularVelocity.isZero())
                return;

            // Append platform's yaw rotation to Character's current rotation

            Vector3 yaw = Vector3.Project(otherRigidbody.angularVelocity, _probingRotation * Vector3.up);
            Quaternion yawRotation = Quaternion.Euler(yaw * (Mathf.Rad2Deg * Time.deltaTime));

            _probingRotation *= yawRotation;
        }

        /// <summary>
        /// Will apply velocity from external sources (eg: physical interactions).
        /// </summary>

        protected virtual void ApplyExternalVelocity()
        {
            // Compute external velocity, velocity from external sources (eg: physical interactions)

            Vector3 externalVelocity = ConstrainDirectionToPlane(rigidbody.velocity - lastRigidbodyVelocity);
            if (externalVelocity.isZero())
                return;

            // On a moving platform, ignore external velocity as it can cause slippery

            if (_groundHitResult.hitGround)
            {
                Rigidbody otherRigidbody = _groundHitResult.rigidbody;
                if (otherRigidbody && otherRigidbody.isKinematic)
                    externalVelocity = Vector3.zero;
            }

            // Impart external velocity (delegated to controller)

            _callbackTarget.ImpartExternalVelocity(externalVelocity);

            // Sync rigidbody velocity and Character's velocity
            
            rigidbody.velocity = velocity;
        }

        /// <summary>
        /// Implements a LateFixedUpdate.
        /// </summary>

        protected virtual void OnLateFixedUpdate()
        {
            // If climbed a high step, re-enable PhysX collision (capsule vs stepped collider)

            StepHit stepHitResult = _groundHitResult.stepHitResult;
            if (stepHitResult.isClimbable && stepHitResult.collider)
                Physics.IgnoreCollision(capsuleCollider, stepHitResult.collider);

            // Trigger MovementHit events

            OnMovementHit();

            // If movement is paused (eg: rigidbody is kinematic), return

            if (rigidbody.isKinematic)
                return;
            
            // Cache current position and rotation, could be modified by functions below

            _probingPosition = position;
            _probingRotation = rotation;

            // Resolve any possible overlap

            ResolveOverlaps();

            // Move the Character with dynamic rigidbody as platform (if any)

            MoveWithDynamicPlatform();
            
            // Rotate the Character with platform (if any)

            RotateWithPlatform();

            // Apply external velocity (eg: rigidbody forces) to Character

            ApplyExternalVelocity();

            // Apply position and rotation changes (if any)

            position = _probingPosition;
            rotation = _probingRotation;
        }

        /// <summary>
        /// Sets the give new volume as our current Physics Volume.
        /// Will call PhysicsVolumeChanged delegate.
        /// </summary>
        
        protected virtual void SetPhysicsVolume(PhysicsVolume newPhysicsVolume)
        {
            if (newPhysicsVolume == physicsVolume)
                return;

            _callbackTarget.OnPhysicsVolumeChanged(newPhysicsVolume);

            physicsVolume = newPhysicsVolume;
        }

        /// <summary>
        /// Sets / removes the physics volume.
        /// Called OnTriggerStay.
        /// </summary>
        
        protected virtual void UpdatePhysicsVolume(Collider other)
        {
            // Check if Character is inside or outside a PhysicsVolume,
            // It uses the Character's center as reference point

            Vector3 characterCenter = position + rotation * _capsuleCenter;

            if (other.ClosestPoint(characterCenter) == characterCenter)
            {
                // Entering physics volume

                PhysicsVolume newPhysicsVolume = other.gameObject.GetComponent<PhysicsVolume>();
                if (newPhysicsVolume)
                    SetPhysicsVolume(newPhysicsVolume);
            }
            else
            {
                // Leaving physics volume

                PhysicsVolume newPhysicsVolume = other.gameObject.GetComponent<PhysicsVolume>();
                if (newPhysicsVolume && newPhysicsVolume == physicsVolume)
                    SetPhysicsVolume(null);
            }
        }

        /// <summary>
        /// Sets as current physics volume the one with higher priority.
        /// </summary>

        protected virtual void UpdateVolumes()
        {
            // Find volume with higher priority

            PhysicsVolume volume = null;
            int maxPriority = int.MinValue;

            for (int i = 0, c = _volumes.Count; i < c; i++)
            {
                PhysicsVolume vol = _volumes[i];
                if (vol.priority <= maxPriority)
                    continue;

                maxPriority = vol.priority;
                volume = vol;
            }

            // Assign current volume (one with higher priority)

            if (volume != null)
                UpdatePhysicsVolume(volume.boxCollider);
        }

        /// <summary>
        /// Attempts to add a new physics volume to our volumes list.
        /// </summary>

        protected virtual void OnOnTriggerEnter(Collider other)
        {
            // Is a physics volume ?

            PhysicsVolume volume = other.GetComponent<PhysicsVolume>();
            if (volume == null)
                return;

            // Adds new volume (if not already contained)

            if (!_volumes.Contains(volume))
                _volumes.Insert(0, volume);
        }

        /// <summary>
        /// Attempts to remove a physics volume from our volumes list.
        /// </summary>

        protected virtual void OnOnTriggerExit(Collider other)
        {
            // Is a physics volume ?

            PhysicsVolume volume = other.GetComponent<PhysicsVolume>();
            if (volume == null)
                return;

            // Remove physics volume (if contained)

            if (_volumes.Contains(volume))
                _volumes.Remove(volume);
        }

        /// <summary>
        /// Pause / Resume rigidbody interaction.
        /// While paused, will turn the Rigidbody into Kinematic, preventing any physical interaction.
        /// </summary>

        public void Pause(bool pause)
        {
            if (pause)
            {
                rigidbody.isKinematic = true;

                velocity = Vector3.zero;
                activePlatformVelocity = Vector3.zero;

                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
            else
            {
                rigidbody.isKinematic = false;

                velocity = Vector3.zero;
                activePlatformVelocity = Vector3.zero;

                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;

                rigidbody.WakeUp();
            }
        }

        /// <summary>
        /// Initialize the Rigidbody with its required settings.
        /// </summary>

        protected void InitRigidbody()
        {
            rigidbody.centerOfMass = Vector3.zero;

            rigidbody.drag = 0.0f;
            rigidbody.angularDrag = 0.0f;

            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;
            rigidbody.freezeRotation = true;

            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        /// <summary>
        /// Create a collision mask from current GameObject's layers.
        /// </summary>

        protected virtual void InitCollisionMask()
        {
            int layer = gameObject.layer;

            collisionMask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                    collisionMask |= 1 << i;
            }
        }

        /// <summary>
        /// Specifies the target responsible of handle this callbacks (ICharacterMovementCallbacks), typically a Character.
        /// </summary>

        public virtual void SetCallbackTarget(ICharacterMovementCallbacks target)
        {
            _callbackTarget = target;
        }

        /// <summary>
        /// Removes the callback target.
        /// </summary>

        public virtual void RemoveCallbackTarget()
        {
            _callbackTarget = null;
        }

        /// <summary>
        /// Set this default values.
        ///
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnReset()
        {
            InitRigidbody();

            SetCapsuleDimensions(0.5f, 2.0f);

            _slopeLimit = 60.0f;
            
            _stepOffset = 0.45f;
            
            _perchOffset = 0.5f;
            _unperchOffset = 0.0f;
            _perchAdditionalHeight = 0.4f;

            _constrainToPlane = PlaneConstraint.None;
            _planeConstraintNormal = Vector3.zero;

            _groundProbingDistance = 0.5f;
            _groundMask = LayerMask.GetMask($"Default");

            _collideWithTriggers = false;
        }

        /// <summary>
        /// Validate this editor exposed fields.
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnOnValidate()
        {
            ConstrainToPlane(_constrainToPlane);

            SetCapsuleDimensions(_capsuleRadius, _capsuleHeight);

            slopeLimit = _slopeLimit;

            stepOffset = _stepOffset;

            perchOffset = _perchOffset;
            unperchOffset = _unperchOffset;
            perchAdditionalHeight = _perchAdditionalHeight;

            groundProbingDistance = _groundProbingDistance;
        }

        /// <summary>
        /// Initialize this.
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnAwake()
        {
            _transform = GetComponent<Transform>();

            _rigidbody = GetComponent<Rigidbody>();

            _capsuleCollider = GetComponent<CapsuleCollider>();

            InitRigidbody();

            SetCapsuleDimensions(_capsuleRadius, _capsuleHeight);
            
            InitCollisionMask();

            _minSlopLimit = Mathf.Cos(slopeLimit * Mathf.Deg2Rad);

            _probingDistance = kMinGroundProbingDistance;

            // Attempt to validate frictionless material

            PhysicMaterial physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial != null)
                return;

            physicMaterial = new PhysicMaterial("Frictionless")
            {
                dynamicFriction = 0.0f,
                staticFriction = 0.0f,
                bounciness = 0.0f,
                frictionCombine = PhysicMaterialCombine.Multiply,
                bounceCombine = PhysicMaterialCombine.Average
            };

            capsuleCollider.material = physicMaterial;

            Debug.LogWarning(
                string.Format(
                    "CharacterMovement: No 'PhysicMaterial' found for '{0}'s Collider, a frictionless one has been created and assigned.\n" +
                    "Please add a Frictionless 'PhysicMaterial' to '{0}' game object.",
                    name));
        }

        /// <summary>
        /// Initialize LateFixedUpdate coroutine.
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnOnEnable()
        {
            if (_lateFixedUpdateCoroutine != null)
                StopCoroutine(_lateFixedUpdateCoroutine);

            _lateFixedUpdateCoroutine = StartCoroutine(LateFixedUpdate());
        }

        /// <summary>
        /// Stop LateFixedUpdate coroutine.
        /// If overriden, must call base method in order to fully initialize the class.
        /// </summary>

        protected virtual void OnOnDisable()
        {
            if (_lateFixedUpdateCoroutine != null)
                StopCoroutine(_lateFixedUpdateCoroutine);
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

        private void OnEnable()
        {
            OnOnEnable();
        }

        private void OnDisable()
        {
            OnOnDisable();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnOnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnOnTriggerExit(other);
        }

        private IEnumerator LateFixedUpdate()
        {
            WaitForFixedUpdate waitTime = new WaitForFixedUpdate();

            while (true)
            {
                yield return waitTime;

                OnLateFixedUpdate();

                UpdateVolumes();
            }
        }

        #endregion
    }

    #endregion
}