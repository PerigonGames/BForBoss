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
        
        public void Initialize()
        {
            EnableEquipmentPlayerInput();
            SetupWeapons();
            SetupSwapWeaponInputBinding();
        }

        private void EnableEquipmentPlayerInput()
        {
            if (_reloadInputAction != null)
            {
                _reloadInputAction.Enable();
            }

            if (_fireInputAction != null)
            {
                _fireInputAction.Enable();
            }

            if (_swapWeaponInputAction != null)
            {
                _swapWeaponInputAction.Enable();
            }
        }
        
        private void DisableEquipmentPlayerInput()
        {
            if (_reloadInputAction != null)
            {
                _reloadInputAction.Disable();
            }

            if (_fireInputAction != null)
            {
                _fireInputAction.Disable();
            }
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
            var weaponScroll = _inputActions.FindAction("WeaponScroll");
            weaponScroll.started += OnWeaponScrollAction;
            weaponScroll.Enable();
        }
        
        private void OnWeaponScrollAction(InputAction.CallbackContext context)
        {
            Debug.Log("Weapon Scroll PC");
            if (context.started)
            {
                ScrollSwapWeapons(true);
            }
        }

        private void SetupSwapWeaponInputBinding()
        {
            _swapWeaponInputAction.started += OnSwapWeaponAction;
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

        private void OnSwapWeaponAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ScrollSwapWeapons(true);
            }
        }

        private void Update()
        {
            SwapWeaponOnMouse();
        }

        private void Start()
        {
            Initialize();
        }

        private void Awake()
        {
            if (_inputActions == null)
            {
                Debug.LogWarning("Input Action Asset is missing from equipment behaviour");
            }

            if (_weapons.IsNullOrEmpty())
            {
                Debug.LogWarning("There are currently no weapons equipped within Equipment Behaviour");
            }
            
            SetupPlayerEquipmentInput();
        }
        
        #region Mouse Input 
        private void SwapWeaponOnMouse()
        {
            var scrollVector = Mouse.current.scroll.ReadValue().normalized;
            if (scrollVector.y > 0)
            {
                ScrollSwapWeapons(true);
            }
            else if (scrollVector.y < 0)
            {
                ScrollSwapWeapons(false);
            }
        }
        #endregion
    }
}