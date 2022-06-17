using System;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    [RequireComponent(typeof(BulletSpawner))]
    [RequireComponent(typeof(WallHitVFXSpawner))]
    public partial class EquipmentBehaviour : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private WeaponBehaviour[] _weaponBehaviours = null;
        private BulletSpawner _bulletSpawner;
        private WallHitVFXSpawner _wallHitVFXSpawner;
        private MeleeWeaponBehaviour _meleeBehaviour = null;
        private Weapon[] _weapons = null;
        private int _currentWeaponIndex = 0;

        private InputAction _reloadInputAction = null;
        private InputAction _fireInputAction = null;
        private InputAction _swapWeaponInputAction = null;
        private InputAction _meleeWeaponInputAction = null;
        private bool _isMouseScrollEnabled = true;

        private Transform _playerPivotTransform;
        
        public void Initialize(Transform playerPivotTransform)
        {
            _playerPivotTransform = playerPivotTransform;
            EnableEquipmentPlayerInput();
            _meleeBehaviour.Initialize(_meleeWeaponInputAction, () => _playerPivotTransform, onSuccessfulAttack: OnMeleeAttack);
        }

        private void EnableEquipmentPlayerInput()
        {
            _reloadInputAction?.Enable();
            _fireInputAction?.Enable();
            _swapWeaponInputAction?.Enable();
            _meleeWeaponInputAction?.Enable();
            _isMouseScrollEnabled = true;
        }
        
        private void DisableEquipmentPlayerInput()
        {
            _reloadInputAction?.Disable();
            _fireInputAction?.Disable();
            _swapWeaponInputAction?.Disable();
            _meleeWeaponInputAction?.Disable();
            _isMouseScrollEnabled = false;
        }

        private void SetupWeapons()
        {
            _weapons = new Weapon[_weaponBehaviours.Length];
            for(int i = 0; i < _weaponBehaviours.Length; i++)
            {
                _weaponBehaviours[i].Initialize(_fireInputAction, _reloadInputAction, _bulletSpawner, _wallHitVFXSpawner);
                _weapons[i] = _weaponBehaviours[i].WeaponViewModel;
                _weapons[i].ActivateWeapon = false;
            }

            _weapons[_currentWeaponIndex].ActivateWeapon = true;
        }
        
        private void SetupPlayerEquipmentInput()
        {
            _reloadInputAction = _inputActions.FindAction("Reload");
            _fireInputAction = _inputActions.FindAction("Fire");
            _swapWeaponInputAction = _inputActions.FindAction("WeaponSwap");
            _meleeWeaponInputAction = _inputActions.FindAction("Melee");
        }

        private void ScrollSwapWeapons(bool isUpwards)
        {
            _weapons[_currentWeaponIndex].ActivateWeapon = false;
            UpdateCurrentWeaponIndex(isUpwards);
            _weapons[_currentWeaponIndex].ActivateWeapon = true;
        }
        
        private void UpdateCurrentWeaponIndex(bool isUpwards)
        {
            var indexLength = _weaponBehaviours.Length - 1;
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

        private void OnMeleeAttack()
        {
            _weaponBehaviours[_currentWeaponIndex].OnMeleeAttack();
        }

        private void Awake()
        {
            SetupPlayerEquipmentInput();
            _swapWeaponInputAction.started += OnControllerSwapWeaponAction;
            _bulletSpawner = GetComponent<BulletSpawner>();
            _wallHitVFXSpawner = GetComponent<WallHitVFXSpawner>();
            _meleeBehaviour = GetComponent<MeleeWeaponBehaviour>();
            if (_weaponBehaviours.IsNullOrEmpty())
            {
                PanicHelper.Panic(new Exception("There are currently no WeaponBehaviour within the child of EquipmentBehaviour"));
            }
            SetupWeapons();
        }

        private void OnValidate()
        {
            if (_inputActions == null)
            {
                PanicHelper.Panic(new Exception("Input Action Asset is missing from Equipment Behaviour"));
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