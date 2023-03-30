using UnityEngine;

namespace BForBoss
{
    public enum RotationState
    {
        ClockWise,
        CounterClockWise
    }
    public class RotationalMovementBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float _rotationRate = 30f;
        private int _direction = 0;

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

        private void FixedUpdate()
        {
            if (_direction != 0)
            {
                transform.Rotate(Vector3.up, _direction * _rotationRate * Time.fixedDeltaTime);
            }
        }
    }
}
