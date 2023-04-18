using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = Perigon.Utility.Logger;

namespace BForBoss
{
    public class DerekMissileBehaviour : BulletBehaviour
    {
        private enum State
        {
            MoveTowardsApex,
            MoveTowardsTarget,
            AutoPilot
        }

        [Title("Move Towards Highest Point Direction")] 
        [SerializeField, InfoBox("Offset of Height to create a fake visual arc")]
        private float _heightOffsetForInitialLaunch;
        [SerializeField]
        private float _speedToReachApex = 20;
        [SerializeField] 
        private float _rotationalSpeedWhenReachingApex = 10;
        [SerializeField]
        private AnimationCurve _speedCurveToReachApex;
        [SerializeField, InfoBox("Missile moves towards the DIRECTION of apex, not to a specific target")] 
        private float _timeTakenToMoveTowardsApex = 2;
        private float _elapsedTimeToMoveTowardsApex = 0;
        
        [Title("Reaching Target")] 
        [SerializeField]
        private float _speedToReachTarget = 20;
        [SerializeField] 
        private float _rotationalSpeedWhenReachingTarget = 10;
        [SerializeField]
        private AnimationCurve _speedCurveToReachTarget;
        [SerializeField] 
        private float _distanceBeforeSettingAutoDrive = 10;
        private float _elapsedTimeToMoveTowardsTarget;

        [Title("AutoDrive Towards Direction")] 
        [SerializeField]
        private float _autoDriveSpeed = 30f;
        [SerializeField] 
        private float _timeToLive = 2f;
        private float _elapsedTimeToLive = 0;

        [Title("Shared Configurations")]
        [SerializeField] 
        private float _timeTakenToReachMaxSpeedCurve = 2;
        
        private IGetPlayerTransform _playerTarget;
        private State _state = State.MoveTowardsApex;
        private Vector3 _lastDirection;

        public void Initialize(IGetPlayerTransform playerTransform)
        {
            _playerTarget = playerTransform;
        }

        protected override void Update()
        {
            switch (_state)
            {
                case State.MoveTowardsApex:
                    MoveTowardsApex();
                    break;
                case State.MoveTowardsTarget:
                    MoveTowardsTarget();
                    break;
                case State.AutoPilot:
                    AutoPilotTowardsDirection();
                    break;
            }
        }

        private void MoveTowardsApex()
        {
            if (_elapsedTimeToMoveTowardsApex < _timeTakenToMoveTowardsApex)
            {
                _elapsedTimeToMoveTowardsApex += Time.deltaTime;
                var position = transform.position;
                var direction = (_playerTarget.Value.position - position);
                var highDirection = new Vector3(direction.x, _startPosition.y + _heightOffsetForInitialLaunch, direction.z).normalized;
                var evaluatedSpeed = _speedCurveToReachApex.Evaluate(_elapsedTimeToMoveTowardsApex / _timeTakenToReachMaxSpeedCurve) * _speedToReachApex;
                transform.position = Vector3.MoveTowards(position, position + highDirection, Time.deltaTime * evaluatedSpeed);
                var rotation = Quaternion.LookRotation(highDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * _rotationalSpeedWhenReachingApex);
            }
            else
            {
                Logger.LogString($"Missile [{name}] set to TowardsTarget", color: LoggerColor.Green, key: "DerekBoss");
                _state = State.MoveTowardsTarget;
            }
        }

        private void MoveTowardsTarget()
        {
            if (ShouldSetToAutoPilot())
            {
                Logger.LogString($"Missile [{name}] set to Auto Drive", color: LoggerColor.Green, key: "DerekBoss");
                _lastDirection = (_playerTarget.Value.position - transform.position).normalized;
                _state = State.AutoPilot;
                return;
            }
            
            var position = transform.position;
            var direction = (_playerTarget.Value.position - position).normalized;
            var speed = _speedCurveToReachTarget.Evaluate(_elapsedTimeToMoveTowardsTarget / _timeTakenToReachMaxSpeedCurve) * _speedToReachTarget;
            position = Vector3.MoveTowards(position, position + direction, Time.deltaTime * speed);
            transform.position = position;
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * _rotationalSpeedWhenReachingTarget);
                
            _elapsedTimeToMoveTowardsTarget += Time.deltaTime;
        }

        private void AutoPilotTowardsDirection()
        {
            if (_elapsedTimeToLive > _timeToLive)
            {
                Deactivate();
                return;
            }
            _elapsedTimeToLive += Time.deltaTime;
            var position = transform.position;
            transform.position = Vector3.MoveTowards(position, position + _lastDirection, Time.deltaTime * _autoDriveSpeed);;
            transform.rotation = Quaternion.LookRotation(_lastDirection, Vector3.up);
        }

        private bool ShouldSetToAutoPilot()
        {
            return Vector3.Distance(transform.position, _playerTarget.Value.position) < _distanceBeforeSettingAutoDrive;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var contact = collision.GetContact(0);
            HitObject(collision.collider, contact.point, contact.normal);
            Deactivate();
        }

        private void OnEnable()
        {
            _elapsedTimeToMoveTowardsApex = 0;
            _elapsedTimeToMoveTowardsTarget = 0;
            _elapsedTimeToLive = 0;
            _state = State.MoveTowardsApex;
        }
    }
}
