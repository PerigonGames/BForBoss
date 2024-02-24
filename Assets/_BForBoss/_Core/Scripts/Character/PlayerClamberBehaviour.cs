using System;
using ECM2.Characters;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class PlayerClamberBehaviour : MonoBehaviour
    {        
        [SerializeField] private float _clamberSpeed = 2;
        private Camera _camera;
        private PlayerMovementBehaviour _movementBehaviour;
        private Func<Vector2> _movementInput;
        private bool _isClambering;
        private Vector3 _target;
        
        private Camera MainCamera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }

                return _camera;
            }
        }
        
        public void Initialize(PlayerMovementBehaviour movementBehaviour, Func<Vector2> getMovementInput)
        {
            _movementBehaviour = movementBehaviour;
            _movementInput = getMovementInput;
        }

        private bool PerformClamberRaycast(out RaycastHit raycast)
        {
            var isFeetFacingClamberWall = Physics.Raycast(
                _movementBehaviour.rootPivot.transform.position,
                MainCamera.transform.forward,
                out RaycastHit raycastFeetHit,
                maxDistance: 1,
                layerMask: TagsAndLayers.Mask.ClamberWallMask,
                queryTriggerInteraction: QueryTriggerInteraction.Collide);
            
            var isHeadFacingClamberWall = Physics.Raycast(
                MainCamera.transform.position,
                MainCamera.transform.forward,
                out RaycastHit raycastHeadHit,
                maxDistance: 1,
                layerMask: TagsAndLayers.Mask.ClamberWallMask,
                queryTriggerInteraction: QueryTriggerInteraction.Collide);
            
            
            if (isFeetFacingClamberWall)
            {
                raycast = raycastFeetHit;
            }
            else if (isHeadFacingClamberWall)
            {
                raycast = raycastHeadHit;
            }
            else
            {
                raycast = new RaycastHit();
                return false;
            }
            return true;
        }
        
        private void FixedUpdate()
        {
            var isMovingForward = _movementInput().y > 0;
            var isFacingClamberWall = PerformClamberRaycast(out RaycastHit raycastHit);

            Debug.Log($"Facing Clamber Wall: {isFacingClamberWall}\nMoving Forward: {isMovingForward}\nNot Clambering:{_isClambering}");
            if (isFacingClamberWall && isMovingForward && !_isClambering)
            {
                // Is character below the clamber wall
                if (transform.position.y < raycastHit.collider.bounds.max.y)
                {
                    _isClambering = true;
                    var yPosition = raycastHit.collider.bounds.max.y + 1f;
                    _target = new Vector3(raycastHit.point.x, yPosition, raycastHit.point.z);
                    Debug.Log("Raycasting Clamber Section");
                }
            }

            if (_isClambering)
            {
                Debug.Log("Is Clambering");
                MoveUpwards(target: _target);
                if (Math.Abs(Vector3.Distance(transform.position, _target)) < 0.1)
                {
                    Debug.Log("Stop Clambering");
                    _isClambering = false;
                    _movementBehaviour.SetMovementMode(MovementMode.Falling);
                }
            }
        }
        
        private void MoveUpwards(Vector3 target)
        {
            _movementBehaviour.SetMovementMode(MovementMode.None);
            var lerpedPosition = Vector3.Lerp(transform.position, target, Time.deltaTime * _clamberSpeed);
            _movementBehaviour.SetPosition(lerpedPosition);
        }

        private void OnDrawGizmosSelected()
        {
            if (_movementBehaviour != null)
            {
                Gizmos.color = Color.red;
                Vector3 direction =
                    _movementBehaviour.rootPivot.transform.TransformDirection(Vector3.forward);
                Gizmos.DrawRay(_movementBehaviour.rootPivot.transform.position, direction * 5);
                
                Gizmos.color = Color.blue;
                Vector3 camDirection =
                    MainCamera.transform.TransformDirection(Vector3.forward);
                Gizmos.DrawRay(MainCamera.transform.position, camDirection * 5);
            }
        }
    }
}
