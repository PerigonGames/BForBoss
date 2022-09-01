using System;
using UnityEngine.InputSystem;

namespace Perigon.Utility
{
    public class PGInputSystem
    {
        private readonly InputActionAsset _actionAsset;

        private InputActionMap _playerControlsActionMap;
        private InputActionMap _UIControlsActionMap;
        
        private InputAction _reloadInputAction;
        private InputAction _fireInputAction;
        private InputAction _meleeWeaponInputAction;
        private InputAction _weaponScrollSwapInputAction;
        private InputAction _weaponDirectSwapInputAction;
        
        public event Action<bool> OnFireAction;
        public event Action OnReloadAction;
        public event Action OnMeleeAction;
        public event Action<bool> OnScrollWeaponChangeAction;
        public event Action<int> OnDirectWeaponChangeAction;

        public PGInputSystem(InputActionAsset asset)
        {
            _actionAsset = asset;
            SetupActionMapInput();
            SetupPlayerActions();
        }

        public void SetToUIControls()
        {
            _UIControlsActionMap.Enable();
            _playerControlsActionMap.Disable();
            LockMouseUtility.Instance.UnlockMouse();
        }
        
        public void SetToPlayerControls()
        {
            _UIControlsActionMap.Disable();
            _playerControlsActionMap.Enable();
            LockMouseUtility.Instance.LockMouse();
        }
        
        private void SetupActionMapInput()
        {
            _playerControlsActionMap = _actionAsset.FindActionMap("Player Controls");
            _UIControlsActionMap = _actionAsset.FindActionMap("UI");
        }

        private void SetupPlayerActions()
        {
            _fireInputAction = _playerControlsActionMap.FindAction("Fire");
            _reloadInputAction = _playerControlsActionMap.FindAction("Reload");
            _meleeWeaponInputAction = _playerControlsActionMap.FindAction("Melee");
            _weaponScrollSwapInputAction = _playerControlsActionMap.FindAction("WeaponScrollSwap");
            _weaponDirectSwapInputAction = _playerControlsActionMap.FindAction("WeaponDirectSwap");

            _fireInputAction.started += OnFireInputAction;
            _fireInputAction.canceled += OnFireInputAction;

            _reloadInputAction.performed += OnReloadInputAction;
            
            _meleeWeaponInputAction.performed += OnMeleeInputAction;

            _weaponScrollSwapInputAction.performed += OnWeaponScrolledInputAction;
            _weaponDirectSwapInputAction.performed += OnDirectWeaponChangeInputAction;
        }

        private void OnFireInputAction(InputAction.CallbackContext context)
        {
            var isFiring = context.ReadValue<float>();
            OnFireAction?.Invoke(isFiring >= 1);
        }
        
        private void OnReloadInputAction(InputAction.CallbackContext context)
        {
            OnReloadAction?.Invoke();
        }

        private void OnMeleeInputAction(InputAction.CallbackContext context)
        {
            OnMeleeAction?.Invoke();
        }

        private void OnWeaponScrolledInputAction(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<float>();
            OnScrollWeaponChangeAction?.Invoke(direction > 0);
        }
        
        private void OnDirectWeaponChangeInputAction(InputAction.CallbackContext context)
        {
            var key = (int)context.ReadValue<Single>();
            OnDirectWeaponChangeAction?.Invoke(key);
        }
    }
}
