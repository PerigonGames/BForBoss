using Cinemachine;
using ECM2.Characters;
using ECM2.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM2.Examples.Cinemachine.PathFollowExample
{
    /// <summary>
    /// This example shows how to extend a Character to follow a Cinemachine Path.
    ///
    /// This works similar to the AgentCharacter where the character move towards a given position on the path
    /// </summary>

    public class MyCharacter : Character
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Cinemachine Path")]
        [Tooltip("The path to follow")]
        public CinemachinePathBase path;

        #endregion

        #region FIELDS

        private bool _isPathFollowing;

        private float _pathPosition;

        #endregion

        #region INPUT ACTIONS

        private InputAction interactInputAction { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Has a Path assigned ?
        /// </summary>

        public bool HasPath()
        {
            return path != null;
        }

        /// <summary>
        /// Is Character following a path ?
        /// </summary>

        public bool IsPathFollowing()
        {
            return _isPathFollowing;
        }

        /// <summary>
        /// Starts path following.
        /// Call this from an input event (such as a button 'down' event).
        /// </summary>

        public void FollowPath()
        {
            if (IsPathFollowing())
                return;

            _isPathFollowing = true;

            _pathPosition = path.FindClosestPoint(GetPosition(), 0, -1, 10);
        }

        /// <summary>
        /// Stop the Character from path following.
        /// Call this from an input event (such as a button 'up' event).
        /// </summary>

        public void StopPathFollowing()
        {
            _isPathFollowing = false;
        }

        /// <summary>
        /// Keep updating the destination while desired.
        /// </summary>

        protected virtual void PathFollowing()
        {
            // If Character has no path or not following it, return

            if (!HasPath() || !IsPathFollowing())
                return;

            // Get the world position from our current position on path

            Vector3 targetPosition = path.EvaluatePosition(_pathPosition);

            // Move towards planar target position

            Vector3 toTargetPosition = (targetPosition - GetPosition()).projectedOnPlane(GetUpVector());

            SetMovementDirection(toTargetPosition);

            // If close enough to our target position, update our position on path

            if (toTargetPosition.sqrMagnitude < MathLib.Square(2.0f))
                _pathPosition = path.StandardizePos(_pathPosition + GetMaxSpeed() * Time.deltaTime);
        }

        /// <summary>
        /// Extends HandleInput method to handle path follow.
        /// </summary>

        protected override void HandleInput()
        {
            base.HandleInput();

            PathFollowing();
        }

        /// <summary>
        /// Setup player input actions.
        /// </summary>

        protected override void SetupPlayerInput()
        {
            // Setup base input actions (eg: Movement, Jump, Sprint, Crouch)

            base.SetupPlayerInput();

            // Setup Interact input action handlers

            interactInputAction = actions.FindAction("Interact");
            if (interactInputAction != null)
            {
                interactInputAction.started += context => FollowPath();
                interactInputAction.canceled += context => StopPathFollowing();
            }
        }

        /// <summary>
        /// Initialize this class.
        /// </summary>

        protected override void OnOnEnable()
        {
            // Init Base Class

            base.OnOnEnable();

            // Enable our custom input actions

            interactInputAction?.Enable();
        }

        /// <summary>
        /// De-Initialize this class.
        /// </summary>

        protected override void OnOnDisable()
        {
            // De-Init Base Class

            base.OnOnDisable();

            // Disable our custom input actions

            interactInputAction?.Disable();
        }

        #endregion
    }
}
