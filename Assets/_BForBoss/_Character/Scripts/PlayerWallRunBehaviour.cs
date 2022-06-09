using UnityEngine;
using System;
using ECM2.Common;

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

        [Header("Wall Run Conditions")]
        [SerializeField, Tooltip("Stop a wall run if speed dips below this")]
        private float _minSpeed = 3f;
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
        private FirstPersonPlayer _fpsCharacter = null;
        private LayerMask _mask;
        private Vector3 _lastWallRunNormal;
        private Vector3 _lastPlayerWallRunDirection;
        private Collider _lastWall;
        private float _baseMaxSpeed;
        private Func<Vector2> _movementInput;
        private Action<int> _OnWallRunFinished;

        private bool isCameraStablizeNeeded = true;
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
            _fpsCharacter = baseCharacter as FirstPersonPlayer;
            _movementInput = getMovementInput;
            _OnWallRunFinished = onWallRunFinished;
            _mask = LayerMask.GetMask(PARKOUR_WALL_LAYER);
        }

        public void Falling(Vector3 _)
        {
            if (!CanWallRun()) 
                return;

            var shouldStartWallRun = ProcessRaycasts(_startWallRunDirections, out var hit, ChildTransform, _mask, _wallMaxDistance)
                                     && IsClearOfGround()
                                     && hit.collider != _lastWall;
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
                Debug.Log("Stopped wall run due to jump");
                _lastPlayerWallRunDirection = Vector3.zero;
                isCameraStablizeNeeded = true;
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
                _lastPlayerWallRunDirection = Vector3.zero;
                isCameraStablizeNeeded = true;
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
                Debug.Log("Stopped wall run due to no forward input");
                _lastPlayerWallRunDirection = Vector3.zero;
                isCameraStablizeNeeded = true;
                StopWallRunning(false);
                return;
            }

            if (DidPlayerStopMoving())
            {
                Debug.Log("Stopped MOVING");
                StopWallRunning(false);
                return;
            }
            
            //Far from wall - stop running
            if (ProcessRaycasts(_directionsCurrentlyWallRunning, out var hit, ChildTransform, _mask, _wallMaxDistance))
            {
                Debug.Log("Distance away from Char: " + Vector3.Distance(ChildTransform.position, hit.point));
                _lastWallRunNormal = hit.normal;
            }
            else
            {
                Debug.Log("Stopped since too far away from wall");
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
                Debug.Log("Reset Wall");
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

        private Vector3 LookAwayFromWallAngle
        {
            get
            {
                var angle = 2f;
                if (_wallDirectionRelativeToPlayer == Vector3.left)
                {
                    return Vector3.back + Vector3.right * angle;
                } 
                
                if (_wallDirectionRelativeToPlayer == Vector3.right)
                {
                    return Vector3.back + Vector3.left * angle;
                }

                return Vector3.zero;
            }
        }

        private Vector3 _wallDirectionRelativeToPlayer = Vector3.zero;

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
        
        private void StabilizeCameraIfNeeded(Vector3 characterForward, Vector3 heading)
        {
            if (!isCameraStablizeNeeded)
                return;
            
            var angleDifference = Vector3.SignedAngle(characterForward, heading, Vector3.up);
            if (Mathf.Abs(angleDifference) > _minLookAlongWallStabilizationAngle)
            {
                _baseCharacter.AddYawInput(angleDifference * Time.deltaTime * _lookAlongWallRotationSpeed);
            }
            else
            {
                isCameraStablizeNeeded = false;
            }
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
        #endregion

        #region PRIVATE_METHODS

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
            _wallDirectionRelativeToPlayer = Vector3.right * CalculateWallSideRelativeToPlayer();
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
