using UnityEngine;
using System;
using ECM2.Common;
using Sirenix.OdinInspector;

namespace Perigon.Character
{
    public class PlayerWallRunBehaviour : MonoBehaviour
    {
        private const string PARKOUR_WALL_LAYER = "ParkourWall";

        #region SERIALIZED_FIELDS
        [FoldoutGroup("Wall Run Movement properties")]
        [SerializeField] 
        private float _speedMultiplier = 1f;
        [FoldoutGroup("Wall Run Movement properties")]
        [SerializeField]
        private float _maxWallRunAcceleration = 20f;
        [FoldoutGroup("Wall Run Movement properties")]
        [SerializeField]
        private float _wallGravityDownForce = 0f;

        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField, Tooltip("Stop a wall run if speed dips below this")]
        private float _minSpeed = 3f;
        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField, Tooltip("Don't allow a wall run if the player is too close to the ground")] 
        private float _minHeight = 1f;
        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField]
        private float _wallMaxDistance = 1f;
        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField, Range(0f, 3f), Tooltip("Only allow for a wall run if jump is longer than this")]
        private float _minJumpDuration = 0.3f;

        [FoldoutGroup("Timers")]
        [SerializeField, Tooltip("Gravity won't apply until after this many seconds")]
        private float _gravityTimerDuration = 1f;
        [FoldoutGroup("Timers")]
        [SerializeField, Tooltip("Wall runs on the same wall are only allowed after this long")]
        private float _wallResetTimer = 2f;

        [FoldoutGroup("Wall Run jump properties")]
        [SerializeField]
        private float _wallBounciness = 6f;
        [FoldoutGroup("Wall Run jump properties")]
        [SerializeField]
        private float _jumpHeightMultiplier = 1f;
        [FoldoutGroup("Wall Run jump properties")]
        [SerializeField, Range(0f, 2f)]
        private float _jumpForwardVelocityMultiplier = .75f;

        [FoldoutGroup("Camera Settings")]
        [SerializeField]
        private float _maxCameraAngleRoll = 30f;
        [FoldoutGroup("Camera Settings")]
        [SerializeField]
        private float _cameraRotateDuration = 1f;
        [FoldoutGroup("Camera Settings")]
        [SerializeField] 
        private float _lookAlongWallRotationSpeed = 3f;
        [FoldoutGroup("Camera Settings")]
        [SerializeField] 
        private float _minLookAlongWallStabilizationAngle = 5f;
        [SerializeField]
        private bool _shouldPrintDebugLogs = false;
        #endregion

        #region PRIVATE_FIELDS
        /// <summary>
        /// If we are in a wall run, check for walls all around the player
        /// </summary>
        private readonly Vector3[] _directionsCurrentlyWallRunning = 
        {
            Vector3.right, 
            Vector3.right + Vector3.forward,
            Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left,
            Vector3.left + Vector3.back,
            Vector3.back + Vector3.right,
            Vector3.back
        };
        
        /// <summary>
        /// If not in a wall run, only check for walls ahead of the player
        /// </summary>
        private readonly Vector3[] _startWallRunDirections =
        {
            Vector3.right, 
            Vector3.right + Vector3.forward,
            Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left
        };
        
        private ECM2.Characters.Character _baseCharacter = null;
        private PlayerMovementBehaviour _fpsCharacter = null;
        private LayerMask _parkourWallMask;
        private Vector3 _lastWallRunNormal;
        private Vector3 _lastPlayerWallRunDirection;
        private Collider _lastWall;
        private float _baseMaxSpeed;
        private Func<Vector2> _movementInput;
        private Action<int> _OnWallRunFinished;

        private bool _isCameraStabilizeNeeded = true;
        private float _currentJumpDuration = 0f;
        private float _timeSinceWallAttach = 0f;
        private float _timeSinceWallDetach = 0f;
#endregion

        #region PROPERTIES
        private Transform ChildTransform => _fpsCharacter != null ? _fpsCharacter.rootPivot : transform;

        public bool IsWallRunning { get; private set; }

        #endregion

        #region PUBLIC_METHODS
        public void Initialize(ECM2.Characters.Character baseCharacter, Func<Vector2> getMovementInput, Action<int> onWallRunFinished)
        {
            _baseCharacter = baseCharacter;
            _fpsCharacter = baseCharacter as PlayerMovementBehaviour;
            _movementInput = getMovementInput;
            _OnWallRunFinished = onWallRunFinished;
            _parkourWallMask = LayerMask.GetMask(PARKOUR_WALL_LAYER);
        }

        public void Falling(Vector3 _)
        {
            if (!CanWallRun()) 
                return;

            var shouldStartWallRun = ShouldStartWallRun(out var hit);
            if (shouldStartWallRun)
            {
                StartWallRun(hit.collider, hit.normal);
            }
        }

        public void OnJumped()
        {
            _currentJumpDuration = 0f;
            if (IsWallRunning)
            {
                PrintWallRunLogs("Stopped wall run due to jump");
                StopWallRunning(true);
            }
        }

        public void OnLanded()
        {
            _currentJumpDuration = 0f;
            _lastWall = null;
            if (IsWallRunning)
            {
                PrintWallRunLogs("Stopped wall run due to landed on ground");
                StopWallRunning(false);
            }
        }

        public bool CanJump()
        {
            return IsWallRunning; // can always jump out of a wall run
        }

        public void OnWallRunning()
        {
            if (!IsWallRunning)
            {
                ResetLastWallIfNeeded();
                return;
            }
            
            if (DidForwardInputStop())
            {
                PrintWallRunLogs("Stopped wall run due to no forward input");
                StopWallRunning(false);
                return;
            }

            if (DidPlayerStopMoving())
            {
                PrintWallRunLogs("Stopped Wall run due to low velocity");
                StopWallRunning(false);
                return;
            }
            
            if (IsTooFarFromWall())
            {
                PrintWallRunLogs("Stopped wall run since too far away from wall");
                StopWallRunning(false);
                return;
            }

            _lastPlayerWallRunDirection = ProjectOntoWallNormalized(_lastPlayerWallRunDirection);
            StabilizeCameraIfNeeded(ChildTransform.forward, _lastPlayerWallRunDirection);

            _timeSinceWallAttach += Time.fixedDeltaTime;
            var lookTowardsWall = (_lastPlayerWallRunDirection - _lastWallRunNormal).normalized;
            var constantVelocity = (_lastPlayerWallRunDirection + lookTowardsWall).normalized * _baseCharacter.maxWalkSpeed;
            _baseCharacter.SetVelocity(constantVelocity + DownwardForceIfNeeded());
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
            if (IsWallRunning)
            {
                Vector3 heading = ProjectOntoWallNormalized(ChildTransform.forward);
                Vector3 perpendicular = Vector3.Cross(_lastWallRunNormal, heading);
                float wallDirection = Vector3.Dot(perpendicular, ChildTransform.up);
                return wallDirection;
            }

            return 0;
        }
        #endregion

        #region PRIVATE_METHODS
        
        
        private bool ShouldStartWallRun(out RaycastHit hit)
        {
            var isRaycastingLastWallFromCenterBodyPosition = ProcessRaycasts(_startWallRunDirections, out hit,
                                                                 ChildTransform, _parkourWallMask, _wallMaxDistance)
                                                             && hit.collider != _lastWall;
            var isRaycastingLastWallFromEyePosition = ProcessRaycasts(_startWallRunDirections, out hit,
                                                          _fpsCharacter.eyePivot, _parkourWallMask, _wallMaxDistance)
                                                      && hit.collider != _lastWall;
            var shouldStartWallRun = isRaycastingLastWallFromCenterBodyPosition 
                                     && isRaycastingLastWallFromEyePosition
                                     && IsClearOfGround();
                
            return shouldStartWallRun;
        }

        private void StartWallRun(Collider wallCollider, Vector3 normal)
        {
            _lastWall = wallCollider;
            _lastWallRunNormal = normal;
            var forwardMovementDirection = _lastPlayerWallRunDirection == Vector3.zero
                ? ChildTransform.forward
                : _lastPlayerWallRunDirection;
            _lastPlayerWallRunDirection = ProjectOntoWallNormalized(forwardMovementDirection);
            
            if (!IsWallRunning)
            {
                StartWallRun();
            }
        }

        private void StartWallRun()
        {
            IsWallRunning = true;
            _baseMaxSpeed = _baseCharacter.maxWalkSpeed;
            _baseCharacter.maxWalkSpeed *= _speedMultiplier;
            _timeSinceWallAttach = 0f;
            _timeSinceWallDetach = 0f;
        }

        private bool CanWallRun()
        {
            if (_baseCharacter.IsOnGround()) 
                return false;

            if (!_baseCharacter.IsJumping()) 
                return _movementInput().y > 0 && IsClearOfGround();
            
            _currentJumpDuration += Time.fixedDeltaTime;
            if (_currentJumpDuration < _minJumpDuration) 
                return false;
            return _movementInput().y > 0 && IsClearOfGround();
        }
        
        private bool IsTooFarFromWall()
        {
            if (ProcessRaycasts(_directionsCurrentlyWallRunning, out var hit, ChildTransform, _parkourWallMask, _wallMaxDistance))
            {
                PrintWallRunLogs("Distance away from Wall: " + Vector3.Distance(ChildTransform.position, hit.point));
                _lastWallRunNormal = hit.normal;
                return false;
            }
            
            return true;
        }
                
        private void StabilizeCameraIfNeeded(Vector3 characterForward, Vector3 heading)
        {
            if (!_isCameraStabilizeNeeded)
                return;
            
            var angleDifference = Vector3.SignedAngle(characterForward, heading, Vector3.up);
            if (Mathf.Abs(angleDifference) > _minLookAlongWallStabilizationAngle)
            {
                _baseCharacter.AddYawInput(angleDifference * Time.deltaTime * _lookAlongWallRotationSpeed);
            }
            else
            {
                _isCameraStabilizeNeeded = false;
            }
        }

        private void StopWallRunning(bool jumpedOutOfWallRun)
        {
            if (!IsWallRunning) 
                return;
            IsWallRunning = false;
            _baseCharacter.maxWalkSpeed = _baseMaxSpeed;
            _timeSinceWallAttach = 0f;
            _timeSinceWallDetach = 0f;
            _lastPlayerWallRunDirection = Vector3.zero;
            _isCameraStabilizeNeeded = true;
            _OnWallRunFinished?.Invoke(jumpedOutOfWallRun ? 1 : 0);
        }
        
        private bool DidPlayerStopMoving()
        {
            return _baseCharacter.GetVelocity().magnitude < _minSpeed;
        }
        
        private void ResetLastWallIfNeeded()
        {
            _timeSinceWallDetach += Time.deltaTime;
            if(_timeSinceWallDetach > _wallResetTimer)
            {
                _lastWall = null;
                PrintWallRunLogs("Reset Last Wall");
            }
        }

        private Vector3 DownwardForceIfNeeded()
        {
            return _timeSinceWallAttach >= _gravityTimerDuration ? 
                Vector3.down * (_wallGravityDownForce * Time.fixedDeltaTime)
                : Vector3.zero;
        }

        private bool DidForwardInputStop()
        {
            return _movementInput().sqrMagnitude <= 0 || _movementInput().y < 0;
        }

        private bool IsClearOfGround()
        {
            var downwardHit = Physics.Raycast(ChildTransform.position, Vector3.down, out RaycastHit hit, _minHeight);
#if Unity_Editor || Development_Build
            if (downwardHit)
            {
                Debug.DrawRay(ChildTransform.position, Vector3.down * hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(ChildTransform.position, Vector3.down * _minHeight, Color.green);
            }
#endif
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

        private bool GetSmallestRaycastHitIfValid(RaycastHit[] array, out RaycastHit smallest)
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
        
        private bool ProcessRaycasts(Vector3[] directions, out RaycastHit smallestRaycastResult, Transform origin, int layerMask, float maxDistance)
        {
            RaycastHit[] hits = new RaycastHit[directions.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 localDirection = origin.TransformDirection(directions[i]);
                Physics.Raycast(origin.position, localDirection, out hits[i], maxDistance, layerMask);
#if Unity_Editor || Development_Build
                if (hits[i].collider != null)
                {
                    Debug.DrawRay(origin.position, localDirection * hits[i].distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(origin.position, localDirection * maxDistance, Color.red);
                }
#endif
            }
            return GetSmallestRaycastHitIfValid(hits, out smallestRaycastResult);

        }

        private void PrintWallRunLogs(string log)
        {
            if (_shouldPrintDebugLogs)
            {
                Debug.Log(log);
            }
        }
        #endregion
    }
}
