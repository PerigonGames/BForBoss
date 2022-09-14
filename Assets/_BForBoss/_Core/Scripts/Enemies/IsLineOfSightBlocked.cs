using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class IsLineOfSightBlocked
    {
        private Func<Vector3> _shootingFromPosition;
        private Func<Vector3> _destination;
        public IsLineOfSightBlocked(Func<Vector3> destination, Func<Vector3> shootFromPosition)
        {
            _destination = destination;
            _shootingFromPosition = shootFromPosition;
        }
        public bool Execute()
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
