using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(WipeOutWallBehaviour))]
    [RequireComponent(typeof(RotationalMovementBehaviour))]
    public class BossWipeOutWallBehaviour : MonoBehaviour
    {
        private WipeOutWallBehaviour _wipeOutWallBehaviour;
        private RotationalMovementBehaviour _rotationalMovementBehaviour;
        private IGetPlayerTransform _getPlayerTransform;


        public void Initialize(IGetPlayerTransform getPlayerTransform)
        {
            _getPlayerTransform = getPlayerTransform;
        }

        public void ActivateClosestWall()
        {
            var playerPosition = _getPlayerTransform.Value.position;
            _wipeOutWallBehaviour.ActivateWallClosestToPlayer(playerPosition);
            var wallPosition = _wipeOutWallBehaviour.FindClosestWipeOutWallPositionTo(playerPosition);
            var rotationState = GetRotationalDirectionBetween(playerPosition, wallPosition);
            _rotationalMovementBehaviour.StartRotation(rotationState);
        }

        private RotationState GetRotationalDirectionBetween(Vector3 playerPosition, Vector3 closestWipeOutWallPosition)
        {
            Vector3 direction = playerPosition - closestWipeOutWallPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (angle < 0)
            {
                angle += 360f;
            }

            return angle > 180f ? RotationState.ClockWise : RotationState.CounterClockWise;
        }

        private void Awake()
        {
            _wipeOutWallBehaviour = GetComponent<WipeOutWallBehaviour>();
            _rotationalMovementBehaviour = GetComponent<RotationalMovementBehaviour>();
        }
    }
}
