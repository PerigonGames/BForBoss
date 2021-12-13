using PerigonGames;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public partial class EquipmentBehaviour : MonoBehaviour
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
        }

        private void SetupSwapWeaponInputBinding()
        {
            _swapWeaponInputAction.started += SwapWeaponInputAction;
        }

        private void SwapWeaponInputAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                print("started :" + context.ReadValue<Vector2>());
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
    }
}