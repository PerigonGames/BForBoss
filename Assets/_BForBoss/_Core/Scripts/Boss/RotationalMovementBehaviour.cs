using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Entities
{
    public enum RotationState
    {
        ClockWise,
        CounterClockWise
    }
    public class RotationalMovementBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float rotationRate = 30f;
        private int _direction = 0;
        
        [Button]
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

        [Button]
        public void StopRotation()
        {
            _direction = 0;
        }
        
        private void Update()
        {
            if (_direction != 0)
            {
                transform.Rotate(Vector3.up, _direction * rotationRate * Time.deltaTime);
            }
        }
    }
}
