using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public static class LineOfSight
    {
        public static bool IsBlocked(Func<Vector3> destination, Func<Vector3> shootFromPosition)
        {
            var direction = destination() - shootFromPosition(); 
            if (Physics.Raycast(shootFromPosition(), direction.normalized, out var hitInfo))
            {
                Debug.DrawRay(shootFromPosition(), direction.normalized, Color.red);
                if (hitInfo.collider.CompareTag(TagsAndLayers.Tags.Player))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
