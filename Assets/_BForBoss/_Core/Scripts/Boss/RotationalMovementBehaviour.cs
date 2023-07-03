using UnityEngine;

namespace BForBoss
{
    public enum RotationState
    {
        ClockWise,
        CounterClockWise
    }
    
    [RequireComponent(typeof(Rigidbody))]
    public class RotationalMovementBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float _rotationRate = 30f;
        private int _direction = 0;
        
        private Rigidbody _rigidbody;

        public void Reset()
        {
            _rigidbody.rotation = Quaternion.Euler(0,0,0);
        }
        
        public void StartRotation(RotationState rotation)
        {
            switch (rotation)
            {
                case RotationState.ClockWise:
                    _direction = 1;
                    break;
                case RotationState.CounterClockWise:
                    _direction = -1;
                    break;
            }
        }

        public void StopRotation()
        {
            _direction = 0;
        }
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (_direction != 0)
            {
                var angle = _direction * _rotationRate * Time.fixedDeltaTime;
                _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0, angle, 0));
            }
        }
    }
}
