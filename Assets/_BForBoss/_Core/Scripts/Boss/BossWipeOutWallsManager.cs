using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(WipeOutWallsManager))]
    [RequireComponent(typeof(RotationalMovementBehaviour))]
    public class BossWipeOutWallsManager : MonoBehaviour
    {
        private WipeOutWallsManager _wipeOutWallsManager;
        private RotationalMovementBehaviour _rotationalMovementBehaviour;
        private IGetPlayerTransform _getPlayerTransform;

        public void Reset()
        {
            _wipeOutWallsManager.DeactivateAllShields();
            _rotationalMovementBehaviour.Reset();
        }
        
        public void Initialize(IGetPlayerTransform getPlayerTransform)
        {
            _getPlayerTransform = getPlayerTransform;
            _wipeOutWallsManager.Initialize();
            _wipeOutWallsManager.DeactivateAllShields();
        }
        
        public void ActivateClosestLongWallAndRotate()
        {
            var playerPosition = _getPlayerTransform.Value.position;
            _wipeOutWallsManager.ActivateLongWallClosestTo(playerPosition);
            var rotationState = GetRotationalDirectionFrom(playerPosition);
            _rotationalMovementBehaviour.StartRotation(rotationState);
        }

        public void ActivateClosestAndRotateWall()
        {
            var playerPosition = _getPlayerTransform.Value.position;
            _wipeOutWallsManager.ActivateWallClosestToPlayer(playerPosition);
            var rotationState = GetRotationalDirectionFrom(playerPosition);
            _rotationalMovementBehaviour.StartRotation(rotationState);
        }

        public void DeactivateWallAndRotation()
        {
            _wipeOutWallsManager.DeactivateAllShields();
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

            return angle < 0 ? RotationState.CounterClockWise : RotationState.ClockWise;        
        }

        private void Awake()
        {
            _wipeOutWallsManager = GetComponent<WipeOutWallsManager>();
            _rotationalMovementBehaviour = GetComponent<RotationalMovementBehaviour>();
            
            this.PanicIfNullObject(_wipeOutWallsManager, nameof(_wipeOutWallsManager));
            this.PanicIfNullObject(_rotationalMovementBehaviour, nameof(_rotationalMovementBehaviour));
        }
    }
}
