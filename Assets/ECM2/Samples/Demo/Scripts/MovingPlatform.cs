using ECM2.Components;
using UnityEngine;

namespace ECM2.Demo
{
    public class MovingPlatform : PlatformMovement
    {
        public float speed;
        public Vector3 offset;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;

        private static float EaseInOut(float time, float duration)
        {
            return -0.5f * (Mathf.Cos(Mathf.PI * time / duration) - 1.0f);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _startPosition = transform.position;
            _targetPosition = _startPosition + offset;
        }

        protected override void OnMove()
        {
            float moveTime = Vector3.Distance(_startPosition, _targetPosition) / Mathf.Max(speed, 0.0001f);

            float t = EaseInOut(Mathf.PingPong(Time.time, moveTime), moveTime);

            position = Vector3.Lerp(_startPosition, _targetPosition, t);
        }
    }
}
