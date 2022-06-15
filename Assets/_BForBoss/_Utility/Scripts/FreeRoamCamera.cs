using System;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace BForBoss
{
    public class FreeRoamCamera : MonoBehaviour
    {
        internal class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

                x += rotatedTranslation.x;
                y += rotatedTranslation.y;
                z += rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
            }
        }

        private const float MOUSE_SENSITIVITY_MULTIPLIER = 0.01f;

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel."), Range(2.0f, 6.0f)]
        [SerializeField] private float _boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField] private float _positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("Multiplier for the sensitivity of the rotation.")] 
        [SerializeField] private float _mouseSensitivity = 60.0f;

        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        [SerializeField]private AnimationCurve _mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField] private float _rotationLerpTime = 0.01f;

        private CameraState _targetCameraState = new CameraState();
        private CameraState _interpolatingCameraState = new CameraState();
        private Action _onExitCamera = null;
        private InputAction _movementAction;
        private InputAction _verticalMovementAction;
        private InputAction _lookAction;
        private InputAction _boostFactorAction;
        private InputActionMap _actionMap;
        private Transform _startingTransform;
        private bool _mouseRightButtonPressed;
        
        public void Initialize(Transform playerTransform, Action onExit)
        {
            if (_actionMap == null)
            {
                InitializeActionMap();
            }
            else
            {
                _actionMap.Enable();
            }

            _startingTransform = playerTransform;
            transform.SetPositionAndRotation(playerTransform.position, playerTransform.rotation);
            _onExitCamera = onExit;
            _targetCameraState.SetFromTransform(transform);
            _interpolatingCameraState.SetFromTransform(transform);
        }

        private void InitializeActionMap()
        {
            _actionMap = new InputActionMap("Free Roam Camera Controller");

            _lookAction = _actionMap.AddAction("look", binding: "<Mouse>/delta");
            _movementAction = _actionMap.AddAction("move", binding: "<Gamepad>/leftStick");
            _verticalMovementAction = _actionMap.AddAction("Vertical Movement");
            _boostFactorAction = _actionMap.AddAction("Boost Factor", binding: "<Mouse>/scroll");

            _lookAction.AddBinding("<Gamepad>/rightStick").WithProcessor("scaleVector2(x=15, y=15)");
            _movementAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");
            _verticalMovementAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/pageUp")
                .With("Down", "<Keyboard>/pageDown")
                .With("Up", "<Keyboard>/e")
                .With("Down", "<Keyboard>/q")
                .With("Up", "<Gamepad>/rightshoulder")
                .With("Down", "<Gamepad>/leftshoulder");
            _boostFactorAction.AddBinding("<Gamepad>/Dpad").WithProcessor("scaleVector2(x=1, y=4)");

            _movementAction.Enable();
            _lookAction.Enable();
            _verticalMovementAction.Enable();
            _boostFactorAction.Enable();
        }
        
        private Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = Vector3.zero;
            var moveDelta = _movementAction.ReadValue<Vector2>();
            direction.x = moveDelta.x;
            direction.z = moveDelta.y;
            direction.y = _verticalMovementAction.ReadValue<Vector2>().y;
            return direction;
        }

        private void Update()
        {
            if (IsBackQuotePressed())
            {
                _actionMap.Disable();
                _onExitCamera?.Invoke();
            }

            // Hide and lock cursor when right mouse button pressed
            if (IsRightMouseButtonDown())
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Unlock and show cursor when right mouse button released
            if (IsRightMouseButtonUp())
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            
            // Return to starting position when Reset button is Pressed
            if (IsResetButtonPressed())
            {
                _targetCameraState.SetFromTransform(_startingTransform);
                _interpolatingCameraState.SetFromTransform(_startingTransform);
            }

            // Rotation
            if (IsCameraRotationAllowed())
            {
                var mouseMovement = GetInputLookRotation() * (MOUSE_SENSITIVITY_MULTIPLIER * _mouseSensitivity);
                var mouseSensitivityFactor = _mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                _targetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                _targetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }
            
            // Translation
            var translation = GetInputTranslationDirection() * Time.unscaledDeltaTime;

            // Speed up movement when shift key held
            if (IsBoostPressed())
            {
                translation *= 10.0f;
            }
            
            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
            _boost += GetBoostFactor();
            _boost = Mathf.Clamp(_boost, 2.0f, 6.0f);
            translation *= Mathf.Pow(2.0f, _boost);

            _targetCameraState.Translate(translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / _positionLerpTime) * Time.unscaledDeltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / _rotationLerpTime) * Time.unscaledDeltaTime);
            _interpolatingCameraState.LerpTowards(_targetCameraState, positionLerpPct, rotationLerpPct);

            _interpolatingCameraState.UpdateTransform(transform);
        }

        private float GetBoostFactor()
        {
            return _boostFactorAction.ReadValue<Vector2>().y * 0.01f;
        }

        private Vector2 GetInputLookRotation()
        {
            // try to compensate the diff between the two input systems by multiplying with empirical values
            var delta = _lookAction.ReadValue<Vector2>();
            delta *= 0.5f; // Account for scaling applied directly in Windows code by old input system.
            delta *= 0.1f; // Account for sensitivity setting on old Mouse X and Y axes.
            return delta;
        }

        private bool IsBoostPressed()
        {
            bool boost = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed; 
            boost |= Gamepad.current != null && Gamepad.current.xButton.isPressed;
            return boost;
        }

        private bool IsBackQuotePressed()
        {
            return Keyboard.current != null && Keyboard.current[Key.Backquote].isPressed;
        }

        private bool IsCameraRotationAllowed()
        {
            bool canRotate = Mouse.current != null && Mouse.current.rightButton.isPressed;
            canRotate |= Gamepad.current != null && Gamepad.current.rightStick.ReadValue().magnitude > 0;
            return canRotate;
        }

        private bool IsRightMouseButtonDown()
        {
            return Mouse.current != null && Mouse.current.rightButton.isPressed;
        }

        private bool IsRightMouseButtonUp()
        {
            return Mouse.current != null && !Mouse.current.rightButton.isPressed;
        }

        private bool IsResetButtonPressed()
        {
            bool isResetButtonPressed = Keyboard.current != null && Keyboard.current[Key.Space].isPressed;
            isResetButtonPressed |= Gamepad.current != null && Gamepad.current[GamepadButton.South].isPressed;
            return isResetButtonPressed;
        }
    }
}