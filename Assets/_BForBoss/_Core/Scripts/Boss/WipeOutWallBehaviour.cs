using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class WipeOutWallBehaviour : MonoBehaviour
    {
        private enum WipeOutWall
        {
            North,
            South,
            West,
            East
        }
        
        [SerializeField] private DeathAreaBehaviour _northPlane;
        [SerializeField] private DeathAreaBehaviour _southPlane;
        [SerializeField] private DeathAreaBehaviour _eastPlane;
        [SerializeField] private DeathAreaBehaviour _westPlane;

        public void ActivateLongWallClosestTo(Vector3 position)
        {
            var wall = FindClosestWipeOutWallTo(position);
            var parallelWall = MapToParallelDeathArea(wall);
            MapToDeathArea(parallelWall).gameObject.SetActive(true);
            MapToDeathArea(wall).gameObject.SetActive(true);
        }

        public void DeactivateAllShields()
        {
            _northPlane.gameObject.SetActive(false);
            _southPlane.gameObject.SetActive(false);
            _eastPlane.gameObject.SetActive(false);
            _westPlane.gameObject.SetActive(false);
        }

        public void ActivateWallClosestToPlayer(Vector3 position)
        {
            var wall = FindClosestWipeOutWallTo(position);
            MapToDeathArea(wall).gameObject.SetActive(true);
        }

        private WipeOutWall FindClosestWipeOutWallTo(Vector3 position)
        {
            var closestWall = WipeOutWall.North;
            var closestDistance = float.MaxValue;

            foreach (WipeOutWall direction in Enum.GetValues(typeof(WipeOutWall)))
            {
                float distance = Vector3.Distance(position, MapToDeathArea(direction).transform.position);
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

        private DeathAreaBehaviour MapToDeathArea(WipeOutWall wall)
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
