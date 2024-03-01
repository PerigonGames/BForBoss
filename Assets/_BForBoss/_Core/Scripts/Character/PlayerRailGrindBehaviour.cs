using System;
using ECM2.Characters;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public interface IPlayerRailGrindEvents
    {
        void OnGrindStarted();
    }
    public class PlayerRailGrindBehaviour : MonoBehaviour
    {
        [SerializeField] private float _railgrindThrownOffCooldown = 1;
        [SerializeField] private float _railDetectionArea = 1;
        [SerializeField] private float _heightOffset = 1;
        [SerializeField] private float _grindSpeed = 3;
        private Camera _camera;
        public IPlayerRailGrindEvents RailGrindDelegate;
        private PlayerMovementBehaviour _movementBehaviour;
        private RailGrindData _railGrindData;
        private float _elapsedRailProgress;
        private float _timeForFullSpline;

        private Vector3 _currentForwardDirection;
        private float _railGrindThrownOffElapsedCooldownTime;
        public bool IsRailGrinding => _railGrindData != null;
        private bool CanGrind => _railGrindThrownOffElapsedCooldownTime <= 0;
        
        private Camera MainCamera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }

                return _camera;
            }
        }
        
        public void Initialize(PlayerMovementBehaviour movementBehaviour)
        {
            _movementBehaviour = movementBehaviour;
        }

        public void Reset()
        {
            _elapsedRailProgress = 0;
            _timeForFullSpline = 0;
            _railGrindThrownOffElapsedCooldownTime = 0;
            _railGrindData = null;
        }

        public void OnJumped()
        {
            if (IsRailGrinding)
            {
                ThrowOffRail();
            }
        }
        
        private void FixedUpdate()
        {
            if (IsRailGrinding)
            {
                RailGrind();
            }
            else
            {
                CheckRailGrinding();
            }
        }
        
        private void Update()
        {
            if (_railGrindThrownOffElapsedCooldownTime > 0)
            {
                _railGrindThrownOffElapsedCooldownTime -= Time.deltaTime;
            }
        }

        private void RailGrind()
        {
            float progress = _elapsedRailProgress / _timeForFullSpline;
            if (progress <= 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }
            var worldPosition = _railGrindData.CalculateNextPosition(progress);
            _currentForwardDirection = (worldPosition - transform.position).normalized;
            _movementBehaviour.SetPosition(worldPosition);
            _movementBehaviour.SetVelocity(Vector3.zero);
            
            if (_railGrindData.normalDir)
            {
                _elapsedRailProgress += Time.fixedDeltaTime;
            }
            else
            {
                _elapsedRailProgress -= Time.fixedDeltaTime;
            }
        }


        private void CheckRailGrinding()
        {
            var rails = Physics.OverlapBox(
                center: _movementBehaviour.rootPivot.transform.position,
                halfExtents: Vector3.one * _railDetectionArea / 2,
                orientation: Quaternion.identity,
                layerMask: TagsAndLayers.Mask.RailGrindMask);
            if (rails.Length > 0 && CanGrind)
            {
                StartRailGrind(colliders: rails);
            }
        }

        private void StartRailGrind(Collider[] colliders)
        {
            Debug.Log("Start Rail Grinding");
            _movementBehaviour.SetMovementMode(MovementMode.Custom);
            _railGrindData = GetRailGrindData(colliders);
            SetInitialRailPosition(_railGrindData);
            RailGrindDelegate?.OnGrindStarted();
        }

        private void SetInitialRailPosition(RailGrindData railGrindData)
        {
            _timeForFullSpline = railGrindData.RailLength / _grindSpeed;

            var normalizedTime = railGrindData.CalculateTargetRailPoint(_movementBehaviour.rootPivot.transform.position, out var splinePoint);
            _elapsedRailProgress = _timeForFullSpline * normalizedTime;

            var forwardDirection = railGrindData.CalculateForward(normalizedTime);
            railGrindData.CalculateDirection(forwardDirection, MainCamera.transform.forward);
            _movementBehaviour.SetPosition(splinePoint + (transform.up * _heightOffset));
        }

        private RailGrindData GetRailGrindData(Collider[] collider)
        {
            foreach (var col in collider)
            {
                var railGrindData = col.GetComponent<RailGrindData>();
                if (railGrindData != null)
                {
                    return railGrindData;
                }
            }

            PanicHelper.Panic(new Exception("Rail Grind Data not found on gameObject: " + collider));
            return null;
        }

        private void ThrowOffRail()
        {
            Debug.Log("Thrown Off Rails | Disable Grinding");
            _railGrindThrownOffElapsedCooldownTime = _railgrindThrownOffCooldown;
            _railGrindData = null;
            _movementBehaviour.SetMovementMode(MovementMode.Falling);
            _movementBehaviour.LaunchCharacter(_currentForwardDirection * _grindSpeed);
        }
        
        private void OnDrawGizmos()
        {
            if (_movementBehaviour != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, MainCamera.transform.forward * 3);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_movementBehaviour.rootPivot.transform.position, Vector3.one * _railDetectionArea);
            }
        }
    }
}
