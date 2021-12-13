using PerigonGames;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public class EquipmentBehaviour : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private WeaponBehaviour[] _weapons = null;
        private WeaponBehaviour _currentWeaponHeld = null;

        private InputAction _reloadInputAction = null;
        private InputAction _fireInputAction = null;
        
        public void Initialize()
        {
            EnableEquipmentPlayerInput();
            foreach (var weapon in _weapons)
            {
                weapon.Initialize(_fireInputAction, _reloadInputAction);
                weapon.enabled = false;
            }

            _currentWeaponHeld = _weapons[0];
            _currentWeaponHeld.enabled = true;
        }
        
        public void EnableEquipmentPlayerInput()
        {
            if (_reloadInputAction != null)
            {
                _reloadInputAction.Enable();
            }

            if (_fireInputAction != null)
            {
                _fireInputAction.Enable();
            }
        }
        
        public void DisableEquipmentPlayerInput()
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
        
        private void SetupPlayerEquipmentInput()
        {
            _reloadInputAction = _inputActions.FindAction("Reload");
            _fireInputAction = _inputActions.FindAction("Fire");
        }
        
        private void Update()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                _currentWeaponHeld.enabled = false;
                _currentWeaponHeld = _weapons[0];
                _currentWeaponHeld.enabled = true;
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                _currentWeaponHeld.enabled = false;
                _currentWeaponHeld = _weapons[1];
                _currentWeaponHeld.enabled = true;
            }
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
    }
}