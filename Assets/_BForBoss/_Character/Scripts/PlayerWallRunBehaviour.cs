using ECM2.Characters;
using ECM2.Common;
using UnityEngine;
using System;

namespace BForBoss
{
    public class PlayerWallRunBehaviour : MonoBehaviour
    {
        #region SERIALIZED_FIELDS
        [SerializeField] 
        private float _speedMultiplier = 1f;
        [SerializeField]
        private float _maxWallRunAcceleration = 20f;
        [SerializeField, Tooltip("Don't allow a wall run if the player is too close to the ground")] 
        private float _minHeight = 1f;
        [SerializeField]
        private float _wallMaxDistance = 1f;
        [SerializeField]
        private float _wallGravityDownForce = 0f;
        [SerializeField, Range(0f, 3f), Tooltip("Only allow for a wall run if jump is longer than this")]
        private float _minJumpDuration = 0.3f;
        [SerializeField, Tooltip("Gravity won't apply until after this many seconds")]
        private float _gravityTimerDuration = 1f;
        [SerializeField]
        private float _wallBounciness = 6f;
        [SerializeField]
        private float _jumpHeightMultiplier = 1f;

        [SerializeField]
        private float _maxCameraAngleRoll = 30f;
        [SerializeField]
        private float _cameraRotateDuration = 1f;
        #endregion

        #region PRIVATE_FIELDS
        private readonly Vector3[] directions = new Vector3[]
        {
            Vector3.right, 
            Vector3.right + Vector3.forward,
            Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left
        };

        private bool _isWallRunning = false;
        private Character _baseCharacter = null;
        private FirstPersonCharacter _fpsCharacter = null;
        private LayerMask _mask;
        private Vector3 _lastWallRunPosition;
        private Vector3 _lastWallRunNormal;
        private float _baseMaxSpeed;
        private Func<Vector2> _movementInput;

        private float _currentJumpDuration = 0f;
        private float _timeSinceWallAttach = 0f;
        private float _timeSinceWallDetach = 0f;
#endregion

        #region PROPERTIES
        private Transform ChildTransform
        {
            get => _fpsCharacter != null ? _fpsCharacter.rootPivot : transform;
        }

        public bool IsWallRunning => _isWallRunning;
        #endregion

        #region PUBLIC_METHODS
        public void Initialize(Character baseCharacter, Func<Vector2> getMovementInput)
        {
            _baseCharacter = baseCharacter;
            _fpsCharacter = baseCharacter as FirstPersonCharacter;
            _movementInput = getMovementInput;
            _mask = LayerMask.GetMask("ParkourWall");
        }

        public void Falling(Vector3 _)
        {
            if (!CanWallRun()) return;
            RaycastHit[] _hits = new RaycastHit[directions.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 playerFacingDirection = ChildTransform.TransformDirection(directions[i]);
                Physics.Raycast(ChildTransform.position, playerFacingDirection, out _hits[i], _wallMaxDistance, _mask);
                if (_hits[i].collider != null)
                {
                    Debug.DrawRay(ChildTransform.position, playerFacingDirection * _hits[i].distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(ChildTransform.position, playerFacingDirection * _wallMaxDistance, Color.red);
                }
            }

            if (GetSmallestRaycastHitIfValid(_hits, out RaycastHit hit))
            {
                WallRun(hit);
            }
            else if (_isWallRunning) StopWallRunning();
        }

        public void OnJumped(ref int _jumpCount)
        {
            _currentJumpDuration = 0f;
            if (_isWallRunning)
            {
                StopWallRunning();
                _jumpCount = 0;
            }
        }

        public void OnLanded()
        {
            _currentJumpDuration = 0f;
            if (_isWallRunning)
            {
                StopWallRunning();
            }
        }

        public bool CanJump()
        {
            return _isWallRunning; // can always jump out of a wall run
        }

        public void OnWallRunning()
        {
            if (!_isWallRunning)
            {
                _timeSinceWallDetach += Time.fixedDeltaTime;
                return;
            }

            if (_movementInput().sqrMagnitude <= 0)
            {
                return;
            }
            _timeSinceWallAttach += Time.fixedDeltaTime;
            var velocity = _baseCharacter.GetVelocity();
            var alongWall = ChildTransform.TransformDirection(Vector3.forward).normalized;
            velocity = velocity.dot(alongWall) * alongWall;
            var downwardForce = _timeSinceWallAttach >= _gravityTimerDuration ? 
                Vector3.down * _wallGravityDownForce * Time.fixedDeltaTime
                : Vector3.zero;
            _baseCharacter.SetVelocity(velocity + downwardForce);
        }

        public Vector3 CalcJumpVelocity()
        {
            var velocity = _baseCharacter.GetVelocity();
            if (_isWallRunning)
            {
                velocity += _lastWallRunNormal * _wallBounciness + Vector3.up * _baseCharacter.jumpImpulse * _jumpHeightMultiplier;
            }
            return velocity;
        }

        public float GetMaxAcceleration()
        {
            return _maxWallRunAcceleration;
        }

        public void OnLateUpdate()
        {
            if (_fpsCharacter != null) HandleEyePivotRotation();
        }

#endregion

        #region PRIVATE_METHODS
        private void WallRun(RaycastHit wall)
        {
            _lastWallRunNormal = wall.normal;
            _lastWallRunPosition = wall.point;
            if (!_isWallRunning)
            {
                _isWallRunning = true;
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
                if (_currentJumpDuration < _minJumpDuration) return false;
            }

            if (_movementInput().y <= 0) return false;

            return !Physics.Raycast(ChildTransform.position, Vector3.down, _minHeight);
        }

        private void StopWallRunning()
        {
            if (!_isWallRunning) return;
            _isWallRunning = false;
            _baseCharacter.maxWalkSpeed = _baseMaxSpeed;
            _timeSinceWallAttach = 0f;
            _timeSinceWallDetach = 0f;
        }

        #region CAMERA_ROLL
        private float CalculateWallSideRelativeToPlayer()
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

        private void HandleEyePivotRotation()
        {
            var rotation = _fpsCharacter.eyePivot.localEulerAngles;
            rotation.z = GetCameraRoll();
            _fpsCharacter.eyePivot.localEulerAngles = rotation;
        }

        private float GetCameraRoll()
        {
            float wallDirection = CalculateWallSideRelativeToPlayer();
            float cameraAngle = _baseCharacter.cameraTransform.eulerAngles.z;
            float targetAngle = 0;
            if (wallDirection != 0)
            {
                targetAngle = Mathf.Sign(wallDirection) * _maxCameraAngleRoll;
            }
            return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(_timeSinceWallAttach, _timeSinceWallDetach) / _cameraRotateDuration);
        }
        #endregion

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
