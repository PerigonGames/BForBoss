using ECM2.Components;
using UnityEngine;

namespace ECM2.Examples.Platforms.ScriptedPlatformExample
{
    /// <summary>
    /// Example of how animate a platform using scripted animation.
    /// 
    /// This extends PlatformMovement and override its OnMove method to update its position (and rotation if needed).
    /// </summary>

    public sealed class MyPlatform : PlatformMovement
    {
        #region EDITOR EXPOSED FIELDS

        [SerializeField]
        private float _moveTime = 3.0f;

        [SerializeField]
        private Vector3 _offset;

        #endregion

        #region FIELDS

        private Vector3 _startPosition;
        private Vector3 _endPosition;

        #endregion

        #region PROPERTIES

        public float moveTime
        {
            get => _moveTime;
            set => _moveTime = Mathf.Max(0.0001f, value);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Ease function.
        /// </summary>

        public static float EaseInOut(float time, float duration)
        {
            return -0.5f * (Mathf.Cos(Mathf.PI * time / duration) - 1.0f);
        }

        /// <summary>
        /// Apply platforms movement.
        /// Eg: Update its position and/or rotation here.
        /// </summary>

        protected override void OnMove()
        {
            float t = EaseInOut(Mathf.PingPong(Time.time, moveTime), moveTime);

            position = Vector3.Lerp(_startPosition, _endPosition, t);
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Reset()
        {
            _moveTime = 3.0f;
            _offset = Vector3.zero;
        }

        private void OnValidate()
        {
            moveTime = _moveTime;
        }

        public void Awake()
        {
            rigidbody.isKinematic = true;

            _startPosition = transform.position;
            _endPosition = _startPosition + _offset;
        }

        #endregion
    }
}
