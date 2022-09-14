using System;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Weapons
{
    public enum WeaponAnimationType
    {
        Pistol,
        Rifle
    }
    public interface IWeaponAnimationProvider
    {
        void MeleeAttack(WeaponAnimationType type);
        void WeaponFire(WeaponAnimationType type);
        void SwapWeapon(bool isUpwards);
        void ReloadingWeapon(bool isReloading);
    }
    
    [DefaultExecutionOrder(101)] // Cinemachine uses a modified script execution of 100, this ensures we run after cinemachine updates the camera position
    public class WeaponAnimationController : MonoBehaviour, IWeaponAnimationProvider
    {
        private const string MELEE_PISTOL_PARAM = "Melee_Pistol";
        private const string MELEE_RIFLE_PARAM = "Melee_Rifle";
        
        private const string SHOOT_PISTOL_PARAM = "Shoot_Pistol";
        private const string SHOOT_RIFLE_PARAM = "Shoot_Rifle";
        
        private const string SWAP_WEAPON_DOWN_PARAM = "Swap_Weapon_Down";
        private const string SWAP_WEAPON_UP_PARAM = "Swap_Weapon_Up";

        private const string RELOADING_WEAPON_PARAM = "Reloading_Weapon";
        private const string WALKING_WEAPON_PARAM = "Walking_Velocity";
        
        [Resolve][SerializeField] private GameObject _weaponHolder = null;
        [Resolve][SerializeField] private Animator _weaponAnimator = null;
        private Camera _mainCam;
        
        private Func<float> _characterVelocity = null;
        private Func<bool> _isWallRunning = null;
        private Func<bool> _isSliding = null;
        private Func<bool> _isDashing = null;
        private Func<bool> _isGrounded = null;

        public void Initialize(
            Func<float> characterVelocity, 
            Func<bool> isWallRunning,
            Func<bool> isGrounded,
            Func<bool> isSliding,
            Func<bool> isDashing)
        {
            _characterVelocity = characterVelocity;
            _isGrounded = isGrounded;
            _isSliding = isSliding;
            _isDashing = isDashing;
            _isWallRunning = isWallRunning;
        }
        
        public void MeleeAttack(WeaponAnimationType type) 
        {
            switch (type)
            {
                case WeaponAnimationType.Pistol:
                    _weaponAnimator.SetTrigger(MELEE_PISTOL_PARAM);
                    break;
                case WeaponAnimationType.Rifle:
                    _weaponAnimator.SetTrigger(MELEE_RIFLE_PARAM);
                    break;
            }
        }
        
        public void WeaponFire(WeaponAnimationType type)
        {
            switch (type)
            {
                case WeaponAnimationType.Pistol:
                    _weaponAnimator.SetTrigger(SHOOT_PISTOL_PARAM);
                    break;
                case WeaponAnimationType.Rifle:
                    _weaponAnimator.SetTrigger(SHOOT_RIFLE_PARAM);
                    break;
            }
        }
        
        public void SwapWeapon(bool isUpwards)
        {
            var param = isUpwards ? SWAP_WEAPON_UP_PARAM : SWAP_WEAPON_DOWN_PARAM;
            _weaponAnimator.SetTrigger(param);
        }
        
        public void ReloadingWeapon(bool isReloading)
        {
            _weaponAnimator.SetBool(RELOADING_WEAPON_PARAM, isReloading);
        }

        private void Start()
        {
            _mainCam = Camera.main;
        }

        private void LateUpdate()
        {
            _weaponAnimator.SetFloat(WALKING_WEAPON_PARAM, _characterVelocity() * (CanBobWeapon() ? 1 : 0));
            
            var camTransform = _mainCam.transform;
            _weaponHolder.transform.SetPositionAndRotation(
                camTransform.position,
                camTransform.rotation);
        }

        private bool CanBobWeapon()
        {
            return (_isWallRunning() || _isGrounded()) 
                   && !_isDashing()
                   && !_isSliding()
                   && _characterVelocity() > 0;
        }
    }
}
