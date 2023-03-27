using System;
using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class BossWipeOutWallBehaviour : MonoBehaviour
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
        private IGetPlayerTransform _getPlayerTransform;
        
        [Button]
        public void Initialize(IGetPlayerTransform getPlayerTransform)
        {
            _getPlayerTransform = getPlayerTransform;
        }

        [Button]
        public void ActivateLongWallClosestToPlayer()
        {
            var wall = FindClosestDeathPlane();
            var parallelWall = MapToParallelDeathArea(wall);
            MapToDeathArea(parallelWall).gameObject.SetActive(true);
            MapToDeathArea(wall).gameObject.SetActive(true);
        }

        [Button]
        public void DeactivateAllShields()
        {
            _northPlane.gameObject.SetActive(false);
            _southPlane.gameObject.SetActive(false);
            _eastPlane.gameObject.SetActive(false);
            _westPlane.gameObject.SetActive(false);
        }

        [Button]
        public void ActivateWallClosestToPlayer()
        {
            var wall = FindClosestDeathPlane();
            MapToDeathArea(wall).gameObject.SetActive(true);
        }
        
        private WipeOutWall FindClosestDeathPlane()
        {
            var closestWall = WipeOutWall.North;
            var closestDistance = float.MaxValue;

            foreach (WipeOutWall direction in Enum.GetValues(typeof(WipeOutWall)))
            {
                float distance = Vector3.Distance(_getPlayerTransform.Value.position, MapToDeathArea(direction).transform.position);
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
