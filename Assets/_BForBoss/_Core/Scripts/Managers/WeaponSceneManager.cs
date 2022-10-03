using System;
using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WeaponSceneManager : MonoBehaviour
    {
        [Title("Weapon/Equipment Component")] [SerializeField]
        private WeaponAnimationController _weaponAnimationController = null;

        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        [SerializeField] private AmmunitionCountViewBehaviour _ammunitionCountView = null;
        [SerializeField] private ReloadViewBehaviour _reloadView = null;
        [SerializeField] private CrossHairBehaviour _crossHairBehaviour;
        
        public void Initialize(PlayerBehaviour playerBehaviour, PGInputSystem inputSystem)
        {
            _weaponAnimationController.Initialize(
                () => playerBehaviour.PlayerMovement.SpeedMagnitude,
                () => playerBehaviour.PlayerMovement.IsWallRunning,
                () => playerBehaviour.PlayerMovement.IsGrounded,
                () => playerBehaviour.PlayerMovement.IsSliding,
                () => playerBehaviour.PlayerMovement.IsDashing);
            _equipmentBehaviour.Initialize(playerBehaviour.PlayerMovement.RootPivot, inputSystem, _weaponAnimationController, _crossHairBehaviour);
            _ammunitionCountView.Initialize(_equipmentBehaviour);
            _reloadView.Initialize(_equipmentBehaviour);
            StateManager.Instance.OnStateChanged += OnStateChanged;
        }

        private void OnStateChanged(State state)
        {
            var shouldShowWeaponHUD = state != State.EndGame;
            _reloadView.gameObject.SetActive(shouldShowWeaponHUD);
            _ammunitionCountView.gameObject.SetActive(shouldShowWeaponHUD);
            _weaponAnimationController.gameObject.SetActive(shouldShowWeaponHUD);
            _crossHairBehaviour.gameObject.SetActive(shouldShowWeaponHUD);
        }

        private void OnValidate()
        {
            if (_weaponAnimationController == null)
            {
                PanicHelper.Panic(new Exception("Weapons Manager missing from World Manager"));
            }
            
            if (_equipmentBehaviour == null)
            {
                PanicHelper.Panic(new Exception("Equipment Behaviour missing from World Manager"));
            }
            
            if (_ammunitionCountView == null)
            {
                PanicHelper.Panic(new Exception("Ammunition Count View missing from World Manager"));
            }
            
            if (_reloadView == null)
            {
                PanicHelper.Panic(new Exception("Reload View missing from World Manager"));
            }
        }

        private void OnDestroy()
        {
            StateManager.Instance.OnStateChanged -= OnStateChanged;
        }
    }
}
