using UnityEngine;
using System;
using System.Collections.Generic;
using ECM2.Common;
using Perigon.Utility;
using Sirenix.OdinInspector;

namespace BForBoss
{
    public class PlayerWallRunBehaviour : MonoBehaviour
    {
        #region SERIALIZED_FIELDS
        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField]
        private float _wallResetTimer = 2;
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
        [FoldoutGroup("Wall Run Conditions")]
        [SerializeField, Tooltip("Stop wall running next angled wall is obtuse to this angle")]
        private float _obtuseWallAngle = 70f;
        [SerializeField]
        private bool _shouldPrintDebugLogs = false;
        #endregion

        #region PRIVATE_FIELDS

        private WallRunData _wallRunData = default;
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

        private Vector3 LastLookTowardsWall => (_lastPlayerWallRunDirection - _lastWallRunNormal).normalized;
        public bool IsWallRunning { get; private set; }

        #endregion

        #region PUBLIC_METHODS
        public void Initialize(ECM2.Characters.Character baseCharacter, Func<Vector2> getMovementInput, Action<int> onWallRunFinished)
        {
            _baseCharacter = baseCharacter;
            _fpsCharacter = baseCharacter as PlayerMovementBehaviour;
            _movementInput = getMovementInput;
            _OnWallRunFinished = onWallRunFinished;
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
                StopWallRunning(jumpedOutOfWallRun: true);
            }
        }

        public void OnLanded()
        {
            _currentJumpDuration = 0f;
            _lastWall = null;
            if (IsWallRunning)
            {
                PrintWallRunLogs("Stopped wall run due to landed on ground");
                StopWallRunning(jumpedOutOfWallRun: false);
            }
        }

        public bool CanJump()
        {
            return IsWallRunning; // can always jump out of a wall run
        }

        public void OnWallRunning()
        {
            var lastLookTowardsWall = LastLookTowardsWall;
            if (!IsWallRunning)
            {
                ResetLastWallIfNeeded();
                return;
            }

            if (DidForwardInputStop())
            {
                PrintWallRunLogs("Stopped wall run due to no forward input");
                StopWallRunning(jumpedOutOfWallRun: false);
                return;
            }

            if (DidPlayerStopMoving())
            {
                PrintWallRunLogs("Stopped Wall run due to low velocity");
                StopWallRunning(jumpedOutOfWallRun: false);
                return;
            }

            if (IsTooFarFromWall())
            {
                PrintWallRunLogs("Stopped wall run since too far away from wall");
                StopWallRunning(jumpedOutOfWallRun: false);
                return;
            }

            _timeSinceWallAttach += Time.fixedDeltaTime;
            _lastPlayerWallRunDirection = ProjectOntoWallNormalized(_lastPlayerWallRunDirection);
            var lookTowardsWall = LastLookTowardsWall;
            var constantVelocity = (_lastPlayerWallRunDirection + lookTowardsWall).normalized * _baseCharacter.maxWalkSpeed;
            if (IsNextWallAngleObtuse(currentLookTowardsWall: lookTowardsWall, lastLookTowardsWall))
            {
                PrintWallRunLogs("Stopped wall run since too obtuse from wall");
                StopWallRunning(jumpedOutOfWallRun: false);
                return;
            }

            StabilizeCameraIfNeeded(ChildTransform.forward, _lastPlayerWallRunDirection);
            _baseCharacter.SetVelocity(constantVelocity + DownwardForceIfNeeded());
        }

        public Vector3 CalcJumpVelocity()
        {
            var velocity = _baseCharacter.GetVelocity() * _wallRunData.JumpForwardVelocityMultiplier;
            if (IsWallRunning)
            {
                velocity += _lastWallRunNormal * _wallRunData.WallBounciness +
                            Vector3.up * (_baseCharacter.jumpImpulse * _wallRunData.JumpHeightMultiplier);
            }
            return velocity;
        }


        public float GetMaxAcceleration()
        {
            return _wallRunData.MaxWallRunAcceleration;
        }

        public void OnLateUpdate()
        {
            if (_fpsCharacter != null)
                _fpsCharacter.cmWalkingCamera.m_Lens.Dutch = GetCameraRoll();
        }

        private float CalculateWallSideRelativeToPlayer()
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
                                                                 ChildTransform, TagsAndLayers.Layers.ParkourWallMask, _wallMaxDistance)
                                                             && hit.collider != _lastWall;
            var isRaycastingLastWallFromEyePosition = ProcessRaycasts(_startWallRunDirections, out hit,
                                                          _fpsCharacter.eyePivot, TagsAndLayers.Layers.ParkourWallMask, _wallMaxDistance)
                                                      && hit.collider != _lastWall;
            var shouldStartWallRun = isRaycastingLastWallFromCenterBodyPosition 
                                     && isRaycastingLastWallFromEyePosition
                                     && IsClearOfGround();
            if (shouldStartWallRun && hit.transform.TryGetComponent(out WallRunDataContainer wallRunDataContainer))
            {
                _wallRunData = wallRunDataContainer.GetData;
                return true;
            }
            return false;
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
            _baseCharacter.maxWalkSpeed *= _wallRunData.SpeedMultiplier;
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
            if (ProcessRaycasts(_directionsCurrentlyWallRunning, out var hit, ChildTransform, TagsAndLayers.Layers.ParkourWallMask, _wallMaxDistance))
            {
                PrintWallRunLogs("Distance away from Wall: " + Vector3.Distance(ChildTransform.position, hit.point));
                _lastWallRunNormal = hit.normal;
                return false;
            }
            
            return true;
        }

        private bool IsNextWallAngleObtuse(Vector3 currentLookTowardsWall, Vector3 lastLookTowardsWall)
        {
            return Vector3.Angle(currentLookTowardsWall, lastLookTowardsWall) > _obtuseWallAngle;
        }
                
        private void StabilizeCameraIfNeeded(Vector3 characterForward, Vector3 heading)
        {
            if (!_isCameraStabilizeNeeded)
                return;
            
            var angleDifference = Vector3.SignedAngle(characterForward, heading, Vector3.up);
            if (Mathf.Abs(angleDifference) > _wallRunData.MinLookAlongWallStabilizationAngle)
            {
                _baseCharacter.AddYawInput(angleDifference * Time.deltaTime * _wallRunData.LookAlongWallRotationSpeed);
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
            _wallRunData = default;
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
            return _timeSinceWallAttach >= _wallRunData.GravityTimerDuration ? 
                Vector3.down * (_wallRunData.WallGravityDownForce * Time.fixedDeltaTime)
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
                targetAngle = Mathf.Sign(wallDirection) * heading * _wallRunData.MaxCameraAngleRoll;
            }
            return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(_timeSinceWallAttach, _timeSinceWallDetach) / Mathf.Max(_wallRunData.CameraRotateDuration, 0.01f));
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
            
            return GetSmallestRaycastHitIfValid(FilterRayCastedParkourWalls(hits), out smallestRaycastResult);
        }

        private RaycastHit[] FilterRayCastedParkourWalls(RaycastHit[] hits)
        {
            var filteredHits = new List<RaycastHit>();
            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.TryGetComponent(out WallRunDataContainer _))
                {
                    filteredHits.Add(hit);
                }
            }

            return filteredHits.ToArray();
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
