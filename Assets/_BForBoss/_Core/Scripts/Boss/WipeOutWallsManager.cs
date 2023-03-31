using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace BForBoss
{
    public class WipeOutWallsManager : MonoBehaviour
    {
        private enum WipeOutWall
        {
            North,
            South,
            West,
            East
        }
        
        [SerializeField] private WipeOutWallBehaviour _northPlane;
        [SerializeField] private WipeOutWallBehaviour _southPlane;
        [SerializeField] private WipeOutWallBehaviour _eastPlane;
        [SerializeField] private WipeOutWallBehaviour _westPlane;

        [Title("Properties")] 
        [SerializeField] private float _activateWallDuration = 0.5f;
        [SerializeField] private float _deactivateDuration = 0.5f;

        private WipeOutWallBehaviour[] AllFourWalls => new[] { _northPlane, _southPlane, _eastPlane, _westPlane };
        
        public void Initialize()
        {
            AllFourWalls.ForEach(wall => wall.Initialize(_activateWallDuration, _deactivateDuration));
        }
        
        public void ActivateLongWallClosestTo(Vector3 position)
        {
            var wall = FindClosestWipeOutWallTo(position);
            var parallelWall = MapToParallelDeathArea(wall);
            MapToWipeOutWall(parallelWall).Activate();
            MapToWipeOutWall(wall).Activate();
        }

        public void DeactivateAllShields()
        {
            AllFourWalls.ForEach(wall => wall.Deactivate());
        }

        public void ActivateWallClosestToPlayer(Vector3 position)
        {
            var wall = FindClosestWipeOutWallTo(position);
            MapToWipeOutWall(wall).Activate();
        }

        private WipeOutWall FindClosestWipeOutWallTo(Vector3 position)
        {
            var closestWall = WipeOutWall.North;
            var closestDistance = float.MaxValue;

            foreach (WipeOutWall direction in Enum.GetValues(typeof(WipeOutWall)))
            {
                float distance = Vector3.Distance(position, MapToWipeOutWall(direction).transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWall = direction;
                }
            }

            return closestWall;
        }

        private WipeOutWall MapToParallelDeathArea(WipeOutWall wall)
        {
            switch (wall)
            {
                case WipeOutWall.North:
                    return WipeOutWall.South;
                case WipeOutWall.South:
                    return WipeOutWall.North;
                case WipeOutWall.West:
                    return WipeOutWall.East;
                case WipeOutWall.East:
                    return WipeOutWall.West;
                default:
                    PanicHelper.Panic(new Exception("Unknown Wipe Out Wall Direction Given in BossWipeOutWallBehaviour"));
                    return WipeOutWall.North;
            }
        }

        private WipeOutWallBehaviour MapToWipeOutWall(WipeOutWall wall)
        {
            switch (wall)
            {
                case WipeOutWall.North:
                    return _northPlane;
                case WipeOutWall.South:
                    return _southPlane;
                case WipeOutWall.West:
                    return _westPlane;
                case WipeOutWall.East:
                    return _eastPlane;
                default:
                    PanicHelper.Panic(new Exception("Unknown Wipe Out Wall Direction Given in BossWipeOutWallBehaviour"));
                    break;
            }
            return null;
        }
    }
}
