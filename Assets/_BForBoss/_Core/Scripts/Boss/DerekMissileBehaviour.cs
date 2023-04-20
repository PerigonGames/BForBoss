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
            TowardsApex,
            HomingTarget,
            AutoPilot
        }

        [Title("Move Towards Apex Direction")] 
        [SerializeField, InfoBox("Offset of Height to create a fake visual arc")]
        private float _heightOffsetForInitialLaunch;
        [SerializeField, Min(1)]
        private float _towardsApexSpeed = 20;
        [SerializeField, Min(1)] 
        private float _towardsApexRotationalSpeed = 10;
        [SerializeField]
        private AnimationCurve _towardsApexSpeedCurve;
        [SerializeField, Min(1), InfoBox("Missile moves towards the DIRECTION of apex, not to a specific target")] 
        private float _towardsApexTime = 2;
        
        [Title("Homing Towards Target")] 
        [SerializeField, Min(1)]
        private float _homingTargetSpeed = 20;
        [SerializeField, Min(1)] 
        private float _homingTargetRotationalSpeed = 10;
        [SerializeField]
        private AnimationCurve _homingTargetSpeedCurve;
        [SerializeField, Min(1), InfoBox("Distance between missile/player before state change to autopilot")] 
        private float _homingTargetDistanceStateChange = 10;
        [SerializeField, Min(1), InfoBox("Max time before homing missile state changes to autopilot")] 
        private float _homingTargetTimeToLive;

        [Title("AutoDrive Towards Direction")] 
        [SerializeField, Min(1)]
        private float _autoPilotSpeed = 30f;
        [SerializeField, Min(1)] 
        private float _autoPilotTimeToLive = 2f;

        [Title("Shared Configurations")]
        [SerializeField, Min(0.1f)] 
        private float _timeTakenToReachMaxSpeedCurve = 2;
        
        private State _state = State.TowardsApex;
        private Vector3 _lastDirection;
        private float _elapsedAutoPilotTimeToLive;
        private float _elapsedHomingTargetTimeToLive;
        private float _elapsedTowardsApexTime;

        protected override void Update()
        {
            switch (_state)
            {
                case State.TowardsApex:
                    MoveTowardsApex();
                    break;
                case State.HomingTarget:
                    MoveTowardsTarget();
                    break;
                case State.AutoPilot:
                    AutoPilotTowardsDirection();
                    break;
            }
        }

        private void MoveTowardsApex()
        {
            if (_elapsedTowardsApexTime < _towardsApexTime)
            {
                _elapsedTowardsApexTime += Time.deltaTime;
                var position = transform.position;
                var direction = (HomingTarget.position - position);
                var highDirection = new Vector3(direction.x, _startPosition.y + _heightOffsetForInitialLaunch, direction.z).normalized;
                var evaluatedSpeed = _towardsApexSpeedCurve.Evaluate(_elapsedTowardsApexTime / _timeTakenToReachMaxSpeedCurve) * _towardsApexSpeed;
                transform.position = Vector3.MoveTowards(position, position + highDirection, Time.deltaTime * evaluatedSpeed);
                var rotation = Quaternion.LookRotation(highDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * _towardsApexRotationalSpeed);
            }
            else
            {
                Logger.LogString($"Missile [{name}] set to TowardsTarget", color: LoggerColor.Green, key: "DerekBoss");
                _state = State.HomingTarget;
            }
        }

        private void MoveTowardsTarget()
        {
            if (ShouldSetToAutoPilot())
            {
                Logger.LogString($"Missile [{name}] set to Auto Drive", color: LoggerColor.Green, key: "DerekBoss");
                _lastDirection = (HomingTarget.position - transform.position).normalized;
                _state = State.AutoPilot;
                return;
            }
            
            var position = transform.position;
            var direction = (HomingTarget.position - position).normalized;
            var speed = _homingTargetSpeedCurve.Evaluate(_elapsedHomingTargetTimeToLive / _timeTakenToReachMaxSpeedCurve) * _homingTargetSpeed;
            position = Vector3.MoveTowards(position, position + direction, Time.deltaTime * speed);
            transform.position = position;
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * _homingTargetRotationalSpeed);
                
            _elapsedHomingTargetTimeToLive += Time.deltaTime;
        }

        private void AutoPilotTowardsDirection()
        {
            if (_elapsedAutoPilotTimeToLive > _autoPilotTimeToLive)
            {
                Deactivate();
                return;
            }
            _elapsedAutoPilotTimeToLive += Time.deltaTime;
            var position = transform.position;
            transform.position = Vector3.MoveTowards(position, position + _lastDirection, Time.deltaTime * _autoPilotSpeed);;
            transform.rotation = Quaternion.LookRotation(_lastDirection, Vector3.up);
        }

        private bool ShouldSetToAutoPilot()
        {
            _elapsedHomingTargetTimeToLive += Time.deltaTime;
            var withinPlayerDistance = Vector3.Distance(transform.position, HomingTarget.position) <
                                       _homingTargetDistanceStateChange;
            var timeToLiveOver = _elapsedHomingTargetTimeToLive > _homingTargetTimeToLive;
            return withinPlayerDistance || timeToLiveOver;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isActive)
            {
                var contact = collision.GetContact(0);
                HitObject(collision.collider, contact.point, contact.normal);
                Deactivate();
            }
        }

        private void OnEnable()
        {
            _elapsedTowardsApexTime = 0;
            _elapsedHomingTargetTimeToLive = 0;
            _elapsedAutoPilotTimeToLive = 0;
            _state = State.TowardsApex;
        }
    }
}
