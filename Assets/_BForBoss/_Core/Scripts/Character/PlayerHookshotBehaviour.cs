using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class PlayerHookshotBehaviour : MonoBehaviour
    {
        private const float LineRendererExpansionWithinDistanceOfTarget = 0.1f;
        [Title("Visual Component")]
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _lineRenderExpansionSpeed = 10f;
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
        private bool _isStartingHookShot = false;

        private bool CanHookShot => _canHookshot && _cooldownElapsedTime <= 0;
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
        
        public void Initialize(
            PlayerMovementBehaviour playerMovement)
        {
            _playerMovement = playerMovement;
        }

        public void Reset()
        {
            _canHookshot = false;
            _target = Vector3.zero;
            _isHookshotting = false;
            _cooldownElapsedTime = 0;
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.zero);
            _lineRenderer.enabled = false;
            _isStartingHookShot = false;
        }
        
        public void HookShot(bool didPress)
        {
            if (CanHookShot && didPress)
            {
                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, transform.position);
                _isStartingHookShot = true;
                _lineRenderer.enabled = true;
                Perigon.Utility.Logger.LogString("Hookshot - Start", key: "hookshot");
            }
        }


        private void Update()
        {
            if (_cooldownElapsedTime > 0)
            {
                _cooldownElapsedTime -= Time.deltaTime;
            }

            if (_isStartingHookShot)
            {
                var linePosition = _lineRenderer.GetPosition(1);
                var lerpedPosition = Vector3.Lerp(linePosition, _target, Time.deltaTime * _lineRenderExpansionSpeed);
                _lineRenderer.SetPosition(1, lerpedPosition);
                _lineRenderer.SetPosition(0, transform.position);
                if (Vector3.Distance(lerpedPosition, _target) < LineRendererExpansionWithinDistanceOfTarget) 
                {
                    _isStartingHookShot = false;
                    LaunchCharacter();
                }
            }
        }
        
        private void LaunchCharacter()
        {
            _playerMovement.LaunchCharacter(target: _target, force: _force);
            _canHookshot = false;
            _isHookshotting = true;
            _cooldownElapsedTime = _cooldown;
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, _target) < _distanceBeforeSlowdown && _isHookshotting)
            {
                Perigon.Utility.Logger.LogString("Hookshot - Slow Down within slowdown distance", key: "hookshot");
                _isHookshotting = false;
                _target = Vector3.zero;
                _lineRenderer.enabled = false;
                _playerMovement.SetVelocity(_playerMovement.GetVelocity() * _slowdownMultiplier);
                return;
            }

            if (Physics.Raycast(
                    MainCamera.transform.position, 
                    MainCamera.transform.forward,
                    out RaycastHit raycastHit, 
                    maxDistance: _range, 
                    layerMask: TagsAndLayers.Mask.HookshotTargetMask, 
                    queryTriggerInteraction: QueryTriggerInteraction.Collide))
            {
                _target = raycastHit.transform.position;
                _canHookshot = true;
                return;
            }

            _canHookshot = false;

            if (_isHookshotting && _playerMovement.IsOnGround())
            {
                Perigon.Utility.Logger.LogString("Hookshot - Stopped Grounded", key: "hookshot");
                _lineRenderer.enabled = false;
                 _target = Vector3.zero;
                 _lineRenderer.enabled = false;
                _isHookshotting = false;
            }

            SetLineRendererIfHookShotting();
        }

        private void SetLineRendererIfHookShotting()
        {
            if (_isHookshotting)
            {
                var lerpedPosition = Vector3.Lerp(transform.position, _target, Time.deltaTime * _lineRenderExpansionSpeed);
                _lineRenderer.SetPosition(0, lerpedPosition);
            }
        }
    }
}
