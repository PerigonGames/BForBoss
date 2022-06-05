using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BForBoss
{
    public class DebugFreeRoamCamera : MonoBehaviour
    {
        [SerializeField, Resolve] private Camera _freeRoamCamera;

        [Title("Modifiers")] 
        [SerializeField] private float _movementSpeed = 10.0f;
        [SerializeField, Range(0, 20)] private float _speedBoostModifier = 5.0f;
        [SerializeField, Range(0, 10)] private float _freeLookSensitivity = 3.0f;
        [SerializeField, Range(5, 20)] private float _zoomSensitivity = 10.0f;
        [SerializeField, Range(0,10f)] private float _zoomBoostModifier = 5.0f;

        private bool _isFreeLooking = false;
        
        private bool _canRoam = false;
        
        public void Initialize()
        {
            _canRoam = true;
        }

        private void Update()
        {
            if (!_canRoam)
            {
                return;
            }

            bool isSpeedBoosted = Keyboard.current[Key.LeftShift].wasPressedThisFrame ||
                                  Keyboard.current[Key.RightShift].wasPressedThisFrame;
            float movementSpeed = _movementSpeed * (isSpeedBoosted ? _speedBoostModifier : 1);

            if (Keyboard.current[Key.A].wasPressedThisFrame || Keyboard.current[Key.LeftArrow].wasPressedThisFrame)
            {
                Debug.Log("A was pressed");
                transform.position = transform.position + (-transform.right * (movementSpeed * Time.deltaTime));
            }
            
            if (Keyboard.current[Key.D].wasPressedThisFrame || Keyboard.current[Key.RightArrow].wasPressedThisFrame)
            {
                Debug.Log("D was pressed");
                transform.position = transform.position + (transform.right * (movementSpeed * Time.deltaTime));
            }
            
            if (Keyboard.current[Key.W].wasPressedThisFrame || Keyboard.current[Key.UpArrow].wasPressedThisFrame)
            {
                Debug.Log("W was pressed");
                transform.position = transform.position + (transform.forward * (movementSpeed * Time.deltaTime));
            }
            
            if (Keyboard.current[Key.S].wasPressedThisFrame || Keyboard.current[Key.DownArrow].wasPressedThisFrame)
            {
                Debug.Log("S was pressed");
                transform.position = transform.position + (-transform.forward * (movementSpeed * Time.deltaTime));
            }

            if (Keyboard.current[Key.Q].wasPressedThisFrame)
            {
                transform.position = transform.position + (transform.up * (movementSpeed * Time.deltaTime));
            }

            if (Keyboard.current[Key.E].wasPressedThisFrame)
            {
                transform.position = transform.position + (-transform.up * (movementSpeed * Time.deltaTime));
            }
            
            if (Keyboard.current[Key.R].wasPressedThisFrame || Keyboard.current[Key.PageUp].wasPressedThisFrame)
            {
                transform.position = transform.position + (Vector3.up * (movementSpeed * Time.deltaTime));
            }

            if (Keyboard.current[Key.F].wasPressedThisFrame || Keyboard.current[Key.PageDown].wasPressedThisFrame)
            {
                transform.position = transform.position + (-Vector3.up * (movementSpeed * Time.deltaTime));
            }

            if (_isFreeLooking)
            {
                float newRotationX = transform.localEulerAngles.y + Mouse.current.position.x.ReadValue() * _freeLookSensitivity; 
                float newRotationY = transform.localEulerAngles.x + Mouse.current.position.y.ReadValue() * _freeLookSensitivity;
                transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
            }

            float scrollWheelAxis = Mouse.current.scroll.EvaluateMagnitude();
            if (scrollWheelAxis != 0.0f)
            {
                float zoomSensitivity = _zoomSensitivity * (isSpeedBoosted ? _zoomBoostModifier : 1);
                transform.position = transform.position * (scrollWheelAxis * zoomSensitivity);
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                StartLooking();
            }
            else if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                StopLooking();
            }
        }

        private void StartLooking()
        {
            _isFreeLooking = true;
        }

        private void StopLooking()
        {
            _isFreeLooking = false;
        }
    }
}
