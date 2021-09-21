using UnityEngine;

namespace ECM2.Components
{
    /// <summary>
    /// PlatformMovement Component.
    ///
    /// The PlatformMovement is the responsible to perform the platform movement and compute its current velocities.
    /// The big advantage of this, is to allow the Character to access the platform current state (eg: position, rotation, velocity, angularVelocity).
    /// </summary>

    [DefaultExecutionOrder(-1000)]
    public abstract class PlatformMovement : MonoBehaviour
    {
        #region FIELDS

        private Transform _transform;
        private Rigidbody _rigidbody;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The platform transform.
        /// </summary>

        public new Transform transform
        {
            get
            {
                if (_transform == null)
                    _transform = GetComponent<Transform>();

                return _transform;
            }
        }

        /// <summary>
        /// The platform rigidbody.
        /// </summary>
        
        public new Rigidbody rigidbody
        {
            get
            {
                if (_rigidbody == null)
                    _rigidbody = GetComponent<Rigidbody>();

                return _rigidbody;
            }
        }

        /// <summary>
        /// The platform current position.
        /// </summary>

        public Vector3 position { get; protected set; }

        /// <summary>
        /// The platform current rotation.
        /// </summary>

        public Quaternion rotation { get; protected set; } = Quaternion.identity;

        /// <summary>
        /// The platform current velocity.
        /// </summary>

        public Vector3 velocity
        {
            get => rigidbody.velocity;
            private set => rigidbody.velocity = value;
        }

        /// <summary>
        /// The platform current angular velocity.
        /// </summary>

        public Vector3 angularVelocity
        {
            get => rigidbody.angularVelocity;
            private set => rigidbody.angularVelocity = value;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Apply platform movement (update position and / or rotation here).
        /// </summary>

        protected abstract void OnMove();

        /// <summary>
        /// Move platform and update its current state.
        /// Exclusively called by PlatformManager.
        /// </summary>

        protected virtual void Move()
        {
            // Save last position and rotation

            _lastPosition = position;
            _lastRotation = rotation;

            // Apply platform movement (updates position and / or rotation)
            
            OnMove();

            // Compute current velocities

            Vector3 deltaPosition = position - _lastPosition;
            Quaternion deltaRotation = rotation * Quaternion.Inverse(_lastRotation);

            float deltaTime = Time.deltaTime;

            velocity = deltaTime > 0.0f ? deltaPosition / deltaTime : Vector3.zero;
            angularVelocity = deltaTime > 0.0f ? deltaRotation.eulerAngles * Mathf.Deg2Rad / deltaTime : Vector3.zero;

            // Update rigidbody position and rotation

            rigidbody.MovePosition(position);
            rigidbody.MoveRotation(rotation);
        }

        protected virtual void OnStart()
        {
            // Init position and rotation

            position = transform.position;
            rotation = transform.rotation;
        }

        protected virtual void OnFixedUpdate()
        {
            Move();
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Start()
        {
            OnStart();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        #endregion
    }
}