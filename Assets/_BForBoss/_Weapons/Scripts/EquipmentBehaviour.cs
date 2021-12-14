using System;
using PerigonGames;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public class EquipmentBehaviour : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private WeaponBehaviour[] _weapons = null;
        private int _currentWeaponIndex = 0;

        private InputAction _reloadInputAction = null;
        private InputAction _fireInputAction = null;
        private InputAction _swapWeaponInputAction = null;
        private bool _isMouseScrollEnabled = true;
        
        public void Initialize()
        {
            EnableEquipmentPlayerInput();
            SetupWeapons();
            _swapWeaponInputAction.started += OnControllerSwapWeaponAction;
        }

        private void EnableEquipmentPlayerInput()
        {
            _reloadInputAction?.Enable();
            _fireInputAction?.Enable();
            _swapWeaponInputAction?.Enable();
            _isMouseScrollEnabled = true;
        }
        
        private void DisableEquipmentPlayerInput()
        {
            _reloadInputAction?.Disable();
            _fireInputAction?.Disable();
            _swapWeaponInputAction?.Disable();
            _isMouseScrollEnabled = false;
        }

        private void SetupWeapons()
        {
            foreach (var weapon in _weapons)
            {
                weapon.Initialize(_fireInputAction, _reloadInputAction);
                weapon.enabled = false;
            }

            _weapons[_currentWeaponIndex].enabled = true;
        }
        
        private void SetupPlayerEquipmentInput()
        {
            _reloadInputAction = _inputActions.FindAction("Reload");
            _fireInputAction = _inputActions.FindAction("Fire");
            _swapWeaponInputAction = _inputActions.FindAction("WeaponSwap");
        }

        private void ScrollSwapWeapons(bool isUpwards)
        {
            _weapons[_currentWeaponIndex].enabled = false;
            UpdateCurrentWeaponIndex(isUpwards);
            _weapons[_currentWeaponIndex].enabled = true;
        }
        
        private void UpdateCurrentWeaponIndex(bool isUpwards)
        {
            var indexLength = _weapons.Length - 1;
            _currentWeaponIndex += isUpwards ? 1 : -1;
            
            if (_currentWeaponIndex > indexLength)
            {
                _currentWeaponIndex = 0;
            } 
            else if (_currentWeaponIndex < 0)
            {
                _currentWeaponIndex = indexLength;
            }
        }
        
        private void Update()
        {
            OnMouseSwapWeaponAction();
        }

        private void Start()
        {
            Initialize();
        }

        private void Awake()
        {
            SetupPlayerEquipmentInput();
        }

        private void OnValidate()
        {
            if (_inputActions == null)
            {
                Debug.LogWarning("Input Action Asset is missing from Equipment Behaviour");
            }

            if (_weapons.IsNullOrEmpty())
            {
                Debug.LogWarning("There are currently no weapons equipped within Equipment Behaviour");
            }
        }

        private void OnDestroy()
        {
            _swapWeaponInputAction.started -= OnControllerSwapWeaponAction;
        }

        #region Input 
        private void OnMouseSwapWeaponAction()
        {
            var scrollVector = _isMouseScrollEnabled ? Mouse.current.scroll.ReadValue().normalized : Vector2.zero;
            if (scrollVector.y > 0)
            {
                ScrollSwapWeapons(true);
            }
            else if (scrollVector.y < 0)
            {
                ScrollSwapWeapons(false);
            }
        }
        
        private void OnControllerSwapWeaponAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ScrollSwapWeapons(true);
            }
        }
        #endregion
    }
}