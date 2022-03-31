using UnityEngine;
using System;
using ECM2.Common;

namespace Perigon.Character
{
    public class PlayerWallRunBehaviour : MonoBehaviour
    {
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

        [SerializeField] private float _minLookAlongWallStabilizationAngle = 5f;
        #endregion

        #region PRIVATE_FIELDS
        private readonly Vector3[] _directions = new Vector3[]
        {
            Vector3.right, 
            Vector3.right + Vector3.forward,
            Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left
        };

        private bool _isWallRunning = false;
        private ECM2.Characters.Character _baseCharacter = null;
        private FirstPersonPlayer _fpsCharacter = null;
        private LayerMask _mask;
        private Vector3 _lastWallRunPosition;
        private Vector3 _lastWallRunNormal;
        private Collider _lastWall;
        private float _baseMaxSpeed;
        private Func<Vector2> _movementInput;
        private Action<int> _OnWallRunFinished;

        private bool _hasCameraStabilized = false;

        private float _currentJumpDuration = 0f;
        private float _timeSinceWallAttach = 0f;
        private float _timeSinceWallDetach = 0f;
#endregion

        #region PROPERTIES
        private Transform ChildTransform => _fpsCharacter != null ? _fpsCharacter.rootPivot : transform;

        public bool IsWallRunning => _isWallRunning;
        #endregion

        #region PUBLIC_METHODS
        public void Initialize(ECM2.Characters.Character baseCharacter, Func<Vector2> getMovementInput, Action<int> OnWallRunFinished)
        {
            _baseCharacter = baseCharacter;
            _fpsCharacter = baseCharacter as FirstPersonPlayer;
            _movementInput = getMovementInput;
            _OnWallRunFinished = OnWallRunFinished;
            _mask = LayerMask.GetMask("ParkourWall");
        }

        public void Falling(Vector3 _)
        {
            if (!CanWallRun()) 
                return;
            RaycastHit[] hits = new RaycastHit[_directions.Length];
            for (int i = 0; i < _directions.Length; i++)
            {
                Vector3 playerFacingDirection = ChildTransform.TransformDirection(_directions[i]);
                Physics.Raycast(ChildTransform.position, playerFacingDirection, out hits[i], _wallMaxDistance, _mask);
                if (hits[i].collider != null)
                {
                    Debug.DrawRay(ChildTransform.position, playerFacingDirection * hits[i].distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(ChildTransform.position, playerFacingDirection * _wallMaxDistance, Color.red);
                }
            }

            if (GetSmallestRaycastHitIfValid(hits, out RaycastHit hit) && IsClearOfGround())
            {
                if(_isWallRunning || hit.collider != _lastWall) 
                    WallRun(hit);
            }
        }

        public void OnJumped()
        {
            _currentJumpDuration = 0f;
            if (_isWallRunning)
            {
                StopWallRunning(true);
            }
        }

        public void OnLanded()
        {
            _currentJumpDuration = 0f;
            _lastWall = null;
            if (_isWallRunning)
            {
                StopWallRunning(false);
            }
        }

        public bool CanJump()
        {
            return _isWallRunning; // can always jump out of a wall run
        }

        public void OnWallRunning()
        {
            var deltaTime = Time.fixedDeltaTime;
            if (!_isWallRunning)
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
                StopWallRunning(false);
                return;
            }
            
            var heading = CalculateWallHeadingDirection();
            
            var velocity = _baseCharacter.GetVelocity();
            velocity = velocity.dot(heading) * heading;
            
            Debug.DrawRay(transform.position, velocity);
            
            if (velocity.sqrMagnitude < _minSpeed * _minSpeed)
            {
                StopWallRunning(false);
                return;
            }
            
            LookAlongWall(ChildTransform.forward, heading);

            var downwardForce = _timeSinceWallAttach >= _gravityTimerDuration ? 
                Vector3.down * (_wallGravityDownForce * deltaTime)
                : Vector3.zero;
            _timeSinceWallAttach += deltaTime;
            _baseCharacter.SetVelocity(velocity + downwardForce);
        }

        public Vector3 CalcJumpVelocity()
        {
            var velocity = _baseCharacter.GetVelocity() * _jumpForwardVelocityMultiplier;
            if (_isWallRunning)
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
            if (_isWallRunning)
            {
                Vector3 heading = _lastWallRunPosition - transform.position;
                Vector3 perpendicular = Vector3.Cross(ChildTransform.forward, heading);
                float wallDirection = Vector3.Dot(perpendicular, ChildTransform.up);
                return wallDirection;
            }
            return 0;
        }
        #endregion

        #region PRIVATE_METHODS
        private void WallRun(RaycastHit wall)
        {
            _lastWall = wall.collider;
            _lastWallRunNormal = wall.normal;
            _lastWallRunPosition = wall.point;
            if (!_isWallRunning)
            {
                _isWallRunning = true;
                _hasCameraStabilized = false;
                _baseMaxSpeed = _baseCharacter.maxWalkSpeed;
                _baseCharacter.maxWalkSpeed *= _speedMultiplier;
                _timeSinceWallAttach = 0f;
                _timeSinceWallDetach = 0f;
            }
        }

        private bool CanWallRun()
        {
            if (_baseCharacter.IsOnGround()) return false;

            if (_baseCharacter.IsJumping())
            {
                _currentJumpDuration += Time.fixedDeltaTime;
                if (_currentJumpDuration < _minJumpDuration) 
                    return false;
            }
            return _movementInput().y > 0 && IsClearOfGround();
        }

        private void StopWallRunning(bool jumpedOutOfWallRun)
        {
            if (!_isWallRunning) 
                return;
            _isWallRunning = false;
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
            var heading = CalculateWallHeadingDirection().dot(ChildTransform.forward);
            float cameraAngle = _fpsCharacter.cmWalkingCamera.m_Lens.Dutch;
            float targetAngle = 0;
            if (wallDirection != 0)
            {
                targetAngle = Mathf.Sign(wallDirection) * heading * _maxCameraAngleRoll;
            }
            return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(_timeSinceWallAttach, _timeSinceWallDetach) / _cameraRotateDuration);
        }
        
        private Vector3 CalculateWallHeadingDirection()
        {
            return (Vector3.Cross(ChildTransform.up, _lastWallRunNormal) * CalculateWallSideRelativeToPlayer()).normalized;
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
        #endregion
    }
}
