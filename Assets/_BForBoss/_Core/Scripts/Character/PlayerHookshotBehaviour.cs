using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class PlayerHookshotBehaviour : MonoBehaviour
    {
        private const float MIN_DISTANCE_LIMIT = 3;
        [SerializeField] private float _range = 20f;
        [SerializeField] private float _force = 10f;
        private Camera _camera;

        private PlayerMovementBehaviour _playerMovement;
        private bool _canHookShot;
        private Vector3 _target;
        private bool _isHookShotting = false;

        public void Initialize(
            PGInputSystem input,
            PlayerMovementBehaviour playerMovement)
        {
            input.OnInteractAction += HookShot;
            _playerMovement = playerMovement;
        }

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void HookShot(bool didPress)
        {
            if (_canHookShot)
            {
                _playerMovement.LaunchCharacter(target: _target, force: _force);
                _target = Vector3.zero;
                _canHookShot = false;
                _isHookShotting = true;
                Debug.Log("Start Hookshotting");
            }
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, _target) < MIN_DISTANCE_LIMIT && _isHookShotting)
            {
                Debug.Log("Stop HookShotting and slow down");
                _isHookShotting = false;
                var velocity = _playerMovement.GetVelocity();
                _playerMovement.SetVelocity(velocity * 0.4f);
            }

            if (Physics.Raycast(
                    _camera.transform.position, 
                    _camera.transform.forward,
                    out RaycastHit raycastHit, 
                    maxDistance: _range, 
                    layerMask: TagsAndLayers.Mask.HookshotTargetMask, 
                    queryTriggerInteraction: QueryTriggerInteraction.Collide))
            {
                _target = raycastHit.transform.position;
                Debug.Log("Target Set: "+_target);
                _canHookShot = true;
            }
            else
            {
                _canHookShot = false;
            }

            if (_isHookShotting && _playerMovement.IsOnGround())
            {
                Debug.Log("Stop HookShotting");
                _isHookShotting = false;
            }
        }
    }
}
