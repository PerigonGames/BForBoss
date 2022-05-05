using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using ECM2.Common;
using ECM2.Components;

namespace Perigon.Character
{
    public class PlayerWallRunBehaviour : MonoBehaviour
    {
        private const string PARKOUR_WALL_LAYER = "ParkourWall";

        #region SERIALIZED_FIELDS
        [Header("Wall Run Movement properties")]
        [SerializeField] 
        private float _speedMultiplier = 1f;
        [SerializeField]
        private float _maxWallRunAcceleration = 20f;
        [SerializeField]
        private float _wallGravityDownForce = 0f;
        [SerializeField] 
        private bool _constantSpeed = false;
        [SerializeField, Tooltip("If true, uses Unity collision detection instead of manual detection. Better quality but only works on convex walls")] 
        private bool _useCollisionPhysics = true;

        [Header("Wall Run Conditions")]
        [SerializeField, Tooltip("Stop a wall run if speed dips below this")]
        private float _minSpeed = 0.9f;
        [SerializeField, Tooltip("Don't allow a wall run if the player is too close to the ground")] 
        private float _minHeight = 1f;
        [SerializeField]
        private float _wallMaxDistance = 1f;
        [SerializeField, Range(0f, 3f), Tooltip("Only allow for a wall run if jump is longer than this")]
        private float _minJumpDuration = 0.3f;

        [Header("Timers")]
        [SerializeField, Tooltip("Gravity won't apply until after this many seconds")]
        private float _gravityTimerDuration = 1f;
        [SerializeField, Tooltip("Wall runs on the same wall are only allowed after this long")]
        private float _wallResetTimer = 2f;

        [Header("Wall Run jump properties")]
        [SerializeField]
        private float _wallBounciness = 6f;
        [SerializeField]
        private float _jumpHeightMultiplier = 1f;
        [SerializeField, Range(0f, 2f)]
        private float _jumpForwardVelocityMultiplier = .75f;

        [Header("Camera Settings")]
        [SerializeField]
        private float _maxCameraAngleRoll = 30f;
        [SerializeField]
        private float _cameraRotateDuration = 1f;
        [SerializeField] 
        private float _lookAlongWallRotationSpeed = 3f;
        [SerializeField] 
        private float _minLookAlongWallStabilizationAngle = 5f;
        #endregion

        #region PRIVATE_FIELDS
        /// <summary>
        /// If not in a wall run, only check for walls ahead of the player
        /// </summary>
        private readonly Vector3[] _directions = new Vector3[]
        {
            Vector3.right, 
            Vector3.right + Vector3.forward,
            Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left
        };
        
        /// <summary>
        /// If we are in a wall run, check for walls all around the player
        /// </summary>
        private readonly Vector3[] _directionsCurrentlyWallRunning = new Vector3[]
        {
            Vector3.right, 
            Vector3.right + Vector3.forward,
            Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left,
            Vector3.left + Vector3.back,
            Vector3.back, 
            Vector3.back + Vector3.right
        };

        private ECM2.Characters.Character _baseCharacter = null;
        private FirstPersonPlayer _fpsCharacter = null;
        private LayerMask _mask;
        private int _layerIndex;
        private Vector3 _lastWallRunNormal;
        private Vector3 _constantVelocity;
        private Collider _lastWall;
        private float _baseMaxSpeed;
        private Func<Vector2> _movementInput;
        private Action<int> _OnWallRunFinished;

        private bool _hasCameraStabilized = false;
        private bool _isInitialHeadingSet = false;

        private float _currentJumpDuration = 0f;
        private float _timeSinceWallAttach = 0f;
        private float _timeSinceWallDetach = 0f;
#endregion

        #region PROPERTIES
        private Transform ChildTransform => _fpsCharacter != null ? _fpsCharacter.rootPivot : transform;

        public bool IsWallRunning { get; private set; } = false;

        #endregion

        #region PUBLIC_METHODS
        public void Initialize(ECM2.Characters.Character baseCharacter, Func<Vector2> getMovementInput, Action<int> onWallRunFinished)
        {
            _baseCharacter = baseCharacter;
            _fpsCharacter = baseCharacter as FirstPersonPlayer;
            _movementInput = getMovementInput;
            _OnWallRunFinished = onWallRunFinished;
            _mask = LayerMask.GetMask(PARKOUR_WALL_LAYER);
            _layerIndex = LayerMask.NameToLayer(PARKOUR_WALL_LAYER);
        }

        public void Falling(Vector3 _)
        {
            if (_useCollisionPhysics || !CanWallRun()) 
                return;
            if (IsWallRunning)
            {
                if (ProcessRaycasts(_directionsCurrentlyWallRunning, out var hit, ChildTransform, _mask, _wallMaxDistance) 
                    && IsClearOfGround())
                {
                    WallRun(hit);
                }
                else
                {
                    Debug.Log("Stopped wall run due to no nearby wall, or player was too close to ground");
                    StopWallRunning(false);
                }
            }
            else
            {
                if (ProcessRaycasts(_directions, out var hit, ChildTransform, _mask, _wallMaxDistance) 
                    && IsClearOfGround())
                {
                    if(hit.collider != _lastWall) 
                        WallRun(hit);
                }
            }
        }

        public void OnJumped()
        {
            _currentJumpDuration = 0f;
            if (IsWallRunning)
            {
                Debug.Log("Stopped wall run due to jump");
                StopWallRunning(true);
            }
        }

        public void OnLanded()
        {
            _currentJumpDuration = 0f;
            _lastWall = null;
            if (IsWallRunning)
            {
                Debug.Log("Stopped wall run due to landed on ground");
                StopWallRunning(false);
            }
        }

        public bool CanJump()
        {
            return IsWallRunning; // can always jump out of a wall run
        }

        public void OnWallRunning()
        {
            var deltaTime = Time.fixedDeltaTime;
            if (!IsWallRunning)
            {
                _timeSinceWallDetach += deltaTime;
                if(_timeSinceWallDetach > _wallResetTimer)
                {
                    _lastWall = null;
                }
                return;
            }
            var movement = _movementInput();
            if (movement.sqrMagnitude <= 0 || movement.y < 0)
            {
                Debug.Log("Stopped wall run due to no forward input");
                StopWallRunning(false);
                return;
            }

            var velocity = _baseCharacter.GetVelocity();
            var heading = ProjectOntoWallNormalized(ChildTransform.forward);
            
            if (!_isInitialHeadingSet && _constantSpeed)
            {
                _constantVelocity = heading.normalized * _baseCharacter.maxWalkSpeed;
                _isInitialHeadingSet = true;
            }

            if (_constantSpeed)
            {
                velocity = _constantVelocity;
            }
            else
            {
                velocity = velocity.dot(heading) * heading;
                if (velocity.sqrMagnitude < _minSpeed * _minSpeed)
                {
                    Debug.Log("Stopped wall run as player moved too slow");
                    StopWallRunning(false);
                    return;
                }
            }

            LookAlongWall(ChildTransform.forward, heading);

            _baseCharacter.AddForce(-_lastWallRunNormal * 100f);

            var downwardForce = _timeSinceWallAttach >= _gravityTimerDuration ? 
                Vector3.down * (_wallGravityDownForce * deltaTime)
                : Vector3.zero;
            _timeSinceWallAttach += deltaTime;
            _baseCharacter.SetVelocity(velocity + downwardForce);
        }

        public Vector3 CalcJumpVelocity()
        {
            var velocity = _baseCharacter.GetVelocity() * _jumpForwardVelocityMultiplier;
            if (IsWallRunning)
            {
                velocity += _lastWallRunNormal * _wallBounciness +
                            Vector3.up * (_baseCharacter.jumpImpulse * _jumpHeightMultiplier);
            }
            return velocity;
        }

        public float GetMaxAcceleration()
        {
            return _maxWallRunAcceleration;
        }

        public void OnLateUpdate()
        {
            if (_fpsCharacter != null)
                _fpsCharacter.cmWalkingCamera.m_Lens.Dutch = GetCameraRoll();
        }

        public float CalculateWallSideRelativeToPlayer()
        {
            if (!IsWallRunning) 
                return 0;
            var dir = ChildTransform.forward;
            Vector3 heading = ProjectOntoWallNormalized(dir);
            Vector3 perpendicular = Vector3.Cross(_lastWallRunNormal, heading);
            float wallDirection = Vector3.Dot(perpendicular, ChildTransform.up);
            return wallDirection;
        }
        
        public void OnMovementHit(ref MovementHit movementHit)
        {
            if (!_useCollisionPhysics)
                return;
            if (movementHit.hitLocation != CapsuleHitLocation.Sides)
                return;
            if (movementHit.collider.gameObject.layer != _layerIndex)
                return;
            if (!IsWallRunning && !CanWallRun())
                return;
            WallRun(ref movementHit);
        }
        #endregion

        #region PRIVATE_METHODS
        private void WallRun(RaycastHit wall)
        {
            WallRun(wall.collider, wall.normal);
        }
        
        private void WallRun(ref MovementHit wall)
        {
            WallRun(wall.collider, wall.normal);
            
        }

        private void WallRun(Collider wallCollider, Vector3 normal)
        {
            _lastWall = wallCollider;
            var oldNormal = _lastWallRunNormal;
            _lastWallRunNormal = normal;

            if (ShouldUpdateConstantVelocity(normal, oldNormal))
            {
                GetConstantVelocity(IsWallRunning ? _constantVelocity : ChildTransform.forward);
            }
            
            if (!IsWallRunning)
            {
                StartWallRun();
            }
        }

        private void StartWallRun()
        {
            IsWallRunning = true;
            _hasCameraStabilized = false;
            _isInitialHeadingSet = false;
            _baseMaxSpeed = _baseCharacter.maxWalkSpeed;
            _baseCharacter.maxWalkSpeed *= _speedMultiplier;
            _timeSinceWallAttach = 0f;
            _timeSinceWallDetach = 0f;
        }

        private bool CanWallRun()
        {
            if (_baseCharacter.IsOnGround()) return false;

            if (!_baseCharacter.IsJumping()) 
                return _movementInput().y > 0 && IsClearOfGround();
            
            _currentJumpDuration += Time.fixedDeltaTime;
            if (_currentJumpDuration < _minJumpDuration) 
                return false;
            return _movementInput().y > 0 && IsClearOfGround();
        }

        private void StopWallRunning(bool jumpedOutOfWallRun)
        {
            if (!IsWallRunning) 
                return;
            IsWallRunning = false;
            _baseCharacter.maxWalkSpeed = _baseMaxSpeed;
            _timeSinceWallAttach = 0f;
            _timeSinceWallDetach = 0f;
            _OnWallRunFinished?.Invoke(jumpedOutOfWallRun ? 1 : 0);
        }

        private bool IsClearOfGround()
        {
            var downwardHit = Physics.Raycast(ChildTransform.position, Vector3.down, out RaycastHit hit, _minHeight);
            if (downwardHit)
            {
                Debug.DrawRay(ChildTransform.position, Vector3.down * hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(ChildTransform.position, Vector3.down * _minHeight, Color.green);
            }
            return !downwardHit;
        }

        private float GetCameraRoll()
        {
            float wallDirection = CalculateWallSideRelativeToPlayer();
            var forwardDir = ChildTransform.forward;
            var heading = ProjectOntoWallNormalized(forwardDir).dot(forwardDir);
            float cameraAngle = _fpsCharacter.cmWalkingCamera.m_Lens.Dutch;
            float targetAngle = 0;
            if (wallDirection != 0)
            {
                targetAngle = Mathf.Sign(wallDirection) * heading * _maxCameraAngleRoll;
            }
            return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(_timeSinceWallAttach, _timeSinceWallDetach) / _cameraRotateDuration);
        }

        private Vector3 ProjectOntoWallNormalized(Vector3 direction)
        {
            direction.y = 0;
            return Vector3.ProjectOnPlane(direction, _lastWallRunNormal).normalized;
        }
        
        private void LookAlongWall(Vector3 characterForward, Vector3 heading)
        {
            if (_hasCameraStabilized) 
                return;
            
            var angleDifference = Vector3.SignedAngle(characterForward, heading, Vector3.up);
            if (Mathf.Abs(angleDifference) < _minLookAlongWallStabilizationAngle)
            {
                _hasCameraStabilized = true;
            }
            _baseCharacter.AddYawInput(angleDifference * Time.deltaTime * _lookAlongWallRotationSpeed);
        }
        
        private bool ShouldUpdateConstantVelocity(Vector3 newNormal, Vector3 oldNormal)
        {
            if (!_constantSpeed)
                return false;
            if (!IsWallRunning)
                return true;
            return Vector3.Distance(oldNormal, newNormal) > Vector3.kEpsilon;
        }

        private void GetConstantVelocity(Vector3 initialDirection)
        {
            var heading = ProjectOntoWallNormalized(initialDirection);
            _constantVelocity = heading * _baseCharacter.maxWalkSpeed;
        }

        private void OnCollisionExit(Collision other)
        {
            if (!_useCollisionPhysics || other.collider != _lastWall || !IsWallRunning)
                return;
            Debug.Log("Stopped wall run due to collision exit");
            StopWallRunning(false);
        }

        private static bool GetSmallestRaycastHitIfValid(RaycastHit[] array, out RaycastHit smallest)
        {
            bool validRaycast = false;
            float minimumDistance = float.MaxValue;
            smallest = new RaycastHit();
            foreach (var hit in array)
            {
                if (hit.collider != null && hit.distance < minimumDistance)
                {
                    validRaycast = true;
                    minimumDistance = hit.distance;
                    smallest = hit;
                }
            }
            return validRaycast;
        }
        
        private static bool ProcessRaycasts(Vector3[] directions, out RaycastHit smallestRaycastResult, Transform origin, int layerMask, float maxDistance)
        {
            RaycastHit[] hits = new RaycastHit[directions.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 localDirection = origin.TransformDirection(directions[i]);
                Physics.Raycast(origin.position, localDirection, out hits[i], maxDistance, layerMask);
                if (hits[i].collider != null)
                {
                    Debug.DrawRay(origin.position, localDirection * hits[i].distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(origin.position, localDirection * maxDistance, Color.red);
                }
            }
            return GetSmallestRaycastHitIfValid(hits, out smallestRaycastResult);

        }
        #endregion
    }
}
