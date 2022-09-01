using System;
using UnityEngine;
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
        private InputAction _dashInputAction;
        private InputAction _slowTimeAction;
        private InputAction _weaponScrollSwapInputAction;
        private InputAction _weaponDirectSwapInputAction;
        private InputAction _pauseInputAction;
        
        public event Action<bool> OnFireAction;
        public event Action OnReloadAction;
        public event Action OnMeleeAction;
        public event Action<bool> OnDashAction;
        public event Action<bool> OnSlowTimeAction;
        public event Action<bool> OnScrollWeaponChangeAction;
        public event Action<int> OnDirectWeaponChangeAction;
        public event Action OnPausePressed;

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
            _dashInputAction = _playerControlsActionMap.FindAction("Dash");
            _slowTimeAction = _playerControlsActionMap.FindAction("SlowTime");
            _weaponScrollSwapInputAction = _playerControlsActionMap.FindAction("WeaponScrollSwap");
            _weaponDirectSwapInputAction = _playerControlsActionMap.FindAction("WeaponDirectSwap");
            _pauseInputAction = _playerControlsActionMap.FindAction("Pause");

            _fireInputAction.started += OnFireInputAction;
            _fireInputAction.canceled += OnFireInputAction;

            _reloadInputAction.performed += OnReloadInputAction;
            
            _meleeWeaponInputAction.performed += OnMeleeInputAction;

            _dashInputAction.started += OnDashInputAction;
            _dashInputAction.canceled += OnDashInputAction;

            _slowTimeAction.started += OnSlowTimeInputAction;
            _slowTimeAction.canceled += OnSlowTimeInputAction;

            _weaponScrollSwapInputAction.performed += OnWeaponScrolledInputAction;
            _weaponDirectSwapInputAction.performed += OnDirectWeaponChangeInputAction;

            _pauseInputAction.performed += OnPauseInputAction;
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

        private void OnDashInputAction(InputAction.CallbackContext context)
        {
            var isDashing = context.ReadValue<float>();
            OnDashAction?.Invoke(isDashing >= 1);
        }

        private void OnSlowTimeInputAction(InputAction.CallbackContext context)
        {
            var isSlowingTime = context.ReadValue<float>();
            OnSlowTimeAction?.Invoke(isSlowingTime >= 1);
        }

        private void OnWeaponScrolledInputAction(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<float>();
            if (direction != 0)
            {   
                OnScrollWeaponChangeAction?.Invoke(direction > 0);
            }

            if (context.started)
            {
                Debug.Log("Started");
            }

            if (context.canceled)
            {
                Debug.Log("cancelled");
            }
        }
        
        private void OnDirectWeaponChangeInputAction(InputAction.CallbackContext context)
        {
            var key = (int)context.ReadValue<Single>();
            OnDirectWeaponChangeAction?.Invoke(key);
        }

        private void OnPauseInputAction(InputAction.CallbackContext context)
        {
            OnPausePressed?.Invoke();
        }
    }
}
