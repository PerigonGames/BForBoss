using System;
using ECM2.Characters;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class PlayerRailGrindBehaviour : MonoBehaviour
    {
        [SerializeField] private float _railDetectionArea = 1;
        [SerializeField] private float _heightOffset = 1;
        [SerializeField] private float _grindSpeed = 3;
        private PlayerMovementBehaviour _movementBehaviour;
        private RailGrindData _railGrindData;
        private float _elapsedTime;
        private float _timeForFullSpline;
        private bool IsRailGrinding => _railGrindData != null;
        
        public void Initialize(PlayerMovementBehaviour movementBehaviour)
        {
            _movementBehaviour = movementBehaviour;
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

        private void RailGrind()
        {
            float progress = _elapsedTime / _timeForFullSpline;
            var worldPosition = _railGrindData.NextPosition(progress);
            _movementBehaviour.SetPosition(worldPosition);
            _movementBehaviour.SetVelocity(Vector3.zero);
            _elapsedTime += Time.fixedDeltaTime;
        }

        private void CheckRailGrinding()
        {
            var rails = Physics.OverlapBox(
                center: _movementBehaviour.rootPivot.transform.position,
                halfExtents: Vector3.one * _railDetectionArea / 2,
                orientation: Quaternion.identity,
                layerMask: TagsAndLayers.Mask.RailGrindMask);
            if (rails.Length > 0)
            {
                _movementBehaviour.SetMovementMode(MovementMode.Custom);
                _railGrindData = GetRailGrindData(rails);
                SetInitialRailPosition(_railGrindData);
            }
        }

        private void SetInitialRailPosition(RailGrindData railGrindData)
        {
            _timeForFullSpline = railGrindData.RailLength / _grindSpeed;
            Vector3 splinePoint;
            
            float normalizedTime = railGrindData.CalculateTargetRailPoint(_movementBehaviour.rootPivot.transform.position, out splinePoint);
            _elapsedTime = _timeForFullSpline * normalizedTime;

            var forwardDirection = railGrindData.CalculateForward(normalizedTime);
            railGrindData.CalculateDirection(forwardDirection, transform.forward);
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


        private void OnDrawGizmos()
        {
            if (_movementBehaviour != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_movementBehaviour.rootPivot.transform.position, Vector3.one * _railDetectionArea);
            }
        }
    }
}
