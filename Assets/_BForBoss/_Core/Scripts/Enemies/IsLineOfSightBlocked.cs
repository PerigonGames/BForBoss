using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class LineOfSight
    {
        private Func<Vector3> _shootingFromPosition;
        private Func<Vector3> _destination;
        public LineOfSight(Func<Vector3> destination, Func<Vector3> shootFromPosition)
        {
            _destination = destination;
            _shootingFromPosition = shootFromPosition;
        }
        public bool IsBlocked()
        {
            var direction = _destination() - _shootingFromPosition(); 
            if (Physics.Raycast(_shootingFromPosition(), direction.normalized, out var hitInfo))
            {
                Debug.DrawRay(_shootingFromPosition(), direction.normalized, Color.red);
                if (hitInfo.collider.CompareTag(TagsAndLayers.Tags.Player))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
