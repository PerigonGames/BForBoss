using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class PlayerRailGrindBehaviour : MonoBehaviour
    {
        [SerializeField] private float _railDetectionArea = 1;
        private PlayerMovementBehaviour _movementBehaviour;
        private RailGrindData _railGrindData;
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
                _railGrindData = GetRailGrindData(rails);
            }
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
