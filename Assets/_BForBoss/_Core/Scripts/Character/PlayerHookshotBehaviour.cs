using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    /*
     * TODO - Change Camera -> Player eye pivot
     * TODO Debug.Log -> Using Logger
     */
    public class PlayerHookshotBehaviour : MonoBehaviour
    {

        [Title("Hookshot Configuration")]
        [InfoBox("Distance between player and hookshot target")]
        [SerializeField] private float _range = 20f;
        [SerializeField] private float _force = 10f;
        [InfoBox("Cooldown intervals between hookshots")]
        [SerializeField] private float _cooldown = 1f;
        [Title("Hookshot slowdown")]
        [InfoBox("Slowdown speed")]
        [SerializeField] private float _slowdownMultiplier = 0.4f;
        [InfoBox("Distance between player and target before slowing down player during hooktshot")]
        [SerializeField] private float _distanceBeforeSlowdown = 3;
        private Camera _camera;

        private PlayerMovementBehaviour _playerMovement;

        private bool _canHookshot = false;
        private Vector3 _target;
        private bool _isHookshotting = false;
        private float _cooldownElapsedTime = 0;
        
        private bool CanHookShot => _canHookshot && _cooldownElapsedTime <= 0;
        
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
            if (CanHookShot && didPress)
            {
                _playerMovement.LaunchCharacter(target: _target, force: _force);
                _target = Vector3.zero;
                _canHookshot = false;
                _isHookshotting = true;
                _cooldownElapsedTime = _cooldown;
                Debug.Log("Start Hookshot");
            }
        }

        private void Update()
        {
            if (_cooldownElapsedTime > 0)
            {
                _cooldownElapsedTime -= Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, _target) < _distanceBeforeSlowdown && _isHookshotting)
            {
                Debug.Log("Stop Hookshot Slow Down - within slowdown distance");
                _isHookshotting = false;
                _playerMovement.SetVelocity(_playerMovement.GetVelocity() * _slowdownMultiplier);
                return;
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
                //Debug.Log("Target Set: "+_target);
                _canHookshot = true;
                return;
            }

            _canHookshot = false;

            if (_isHookshotting && _playerMovement.IsOnGround())
            {
                Debug.Log("Stop Hookshot - Grounded");
                _isHookshotting = false;
            }
        }
    }
}
