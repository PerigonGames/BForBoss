using Perigon.Weapons;
using Sirenix.OdinInspector;
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

        private class PlayerPosition : IGetPlayerTransform
        {
            public Transform Value => FindObjectOfType<PlayerBehaviour>().transform;
        }


        [Button]
        public void Initialize(IGetPlayerTransform getPlayerTransform)
        {
            _getPlayerTransform = new PlayerPosition();
        }

        [Button]
        public void ActivateClosestAndRotateWall()
        {
            var playerPosition = _getPlayerTransform.Value.position;
            _wipeOutWallBehaviour.ActivateWallClosestToPlayer(playerPosition);
            var rotationState = GetRotationalDirectionFrom(playerPosition);
            _rotationalMovementBehaviour.StartRotation(rotationState);
        }

        [Button]
        public void DeactivateWallAndRotation()
        {
            _wipeOutWallBehaviour.DeactivateAllShields();
            _rotationalMovementBehaviour.StopRotation();
        }

        private RotationState GetRotationalDirectionFrom(Vector3 playerPosition)
        {
            var centerPointRotation = transform.rotation;
            var localPlayerPosition = Quaternion.Inverse(centerPointRotation) * (playerPosition - transform.position);
            var angle = Mathf.Atan2(localPlayerPosition.z, localPlayerPosition.x);
            angle *= Mathf.Rad2Deg;

            if (angle > 180)
            {
                angle -= 360;
            }

            if (angle < 0)
            {
                return RotationState.CounterClockWise;
            }
            else
            {
                return RotationState.ClockWise;
            }        
        }

        private void Awake()
        {
            _wipeOutWallBehaviour = GetComponent<WipeOutWallBehaviour>();
            _rotationalMovementBehaviour = GetComponent<RotationalMovementBehaviour>();
        }
    }
}
