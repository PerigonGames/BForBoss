using ECM2.Characters;
using ECM2.Components;
using ECM2.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

namespace BForBoss
{
    public class PlayerWallRunBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private float _speedMultiplier = 1f;
        [SerializeField, Tooltip("Don't allow a wall run if the player is too close to the ground")] 
        private float _minHeight = 1f;
        [SerializeField]
        private float _wallMaxDistance;
        [SerializeField]
        private float _wallGravityDownForce = 0f;
        [SerializeField, Range(0f, 3f)]
        private float _minJumpDuration = 0.3f;

        [SerializeField]
        private float _maxCameraAngleRoll;
        [SerializeField]
        private float _cameraRotateDuration;

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
        private Transform _childTransform;
        private RaycastHit[] _hits;
        private LayerMask _mask;
        private Vector3 _lastWallRunPosition;
        private Vector3 _lastWallRunNormal;
        private float _baseMaxSpeed;

        private float _currentJumpDuration = 0f;
        private float _timeSinceWallAttach = 0f;
        private float _timeSinceWallDetach = 0f;

        public void Initialize(Character baseCharacter)
        {
            _baseCharacter = baseCharacter;
            _fpsCharacter = baseCharacter as FirstPersonCharacter;
            _childTransform = _baseCharacter.transform.GetChild(0);
            _mask = LayerMask.GetMask("ParkourWall");
        }

        public void Falling(Vector3 _)
        {
            if (!CanWallRun()) return;
            DoRaycasts();
            
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
                _timeSinceWallDetach += Time.deltaTime;
                return;
            }

            _timeSinceWallAttach += Time.deltaTime;
            var velocity = _baseCharacter.GetVelocity();
            var alongWall = _childTransform.TransformDirection(Vector3.forward).normalized;
            velocity = velocity.dot(alongWall) * alongWall;
            velocity += Vector3.down * _wallGravityDownForce * Time.deltaTime;
            _baseCharacter.SetVelocity(velocity);
        }

        private void HandleEyePivotRotation()
        {
            var rotation = _fpsCharacter.eyePivot.localEulerAngles;
            rotation.z = GetCameraRoll();
            _fpsCharacter.eyePivot.localEulerAngles = rotation;
        }

        private void LateUpdate()
        {
            if(_fpsCharacter != null) HandleEyePivotRotation();
        }

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
                _currentJumpDuration += Time.deltaTime;
                if (_currentJumpDuration < _minJumpDuration) return false;
            }

            return !Physics.Raycast(_childTransform.position, Vector3.down, _minHeight);
        }

        private void StopWallRunning()
        {
            if (!_isWallRunning) return;
            _isWallRunning = false;
            _baseCharacter.maxWalkSpeed = _baseMaxSpeed;
            _timeSinceWallAttach = 0f;
            _timeSinceWallDetach = 0f;
        }

        public bool CalcJumpVelocity(out Vector3 velocity)
        {
            velocity = Vector3.zero;
            if (_isWallRunning)
            {
                velocity = _lastWallRunNormal * _baseCharacter.jumpImpulse + Vector3.up;
            }
            return _isWallRunning;
        }

        float CalculateSide()
        {
            if (_isWallRunning)
            {
                Vector3 heading = _lastWallRunPosition - transform.position;
                Vector3 perp = Vector3.Cross(transform.forward, heading);
                float dir = Vector3.Dot(perp, transform.up);
                return dir;
            }
            return 0;
        }

        private float GetCameraRoll()
        {
            float dir = CalculateSide();
            float cameraAngle = _baseCharacter.cameraTransform.eulerAngles.z;
            float targetAngle = 0;
            if (dir != 0)
            {
                targetAngle = Mathf.Sign(dir) * _maxCameraAngleRoll;
            }
            return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(_timeSinceWallAttach, _timeSinceWallDetach) / _cameraRotateDuration);
        }

        private void DoRaycasts()
        {
            _hits = new RaycastHit[directions.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 dir = _childTransform.TransformDirection(directions[i]); //convert directions to relative to player
                Physics.Raycast(_childTransform.position, dir, out _hits[i], _wallMaxDistance, _mask);
                if (_hits[i].collider != null) Debug.DrawRay(_childTransform.position, dir * _hits[i].distance, Color.green);
                else Debug.DrawRay(_childTransform.position, dir * _wallMaxDistance, Color.red);
            }

            if (GetSmallestRaycastHit(_hits, out RaycastHit hit))
            {
                WallRun(hit);
            }
            else if (_isWallRunning) StopWallRunning();
        }

        private static bool GetSmallestRaycastHit(RaycastHit[] array, out RaycastHit smallest)
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
    }
}
