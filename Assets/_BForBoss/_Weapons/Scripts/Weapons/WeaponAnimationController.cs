using System;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IWeaponAnimationProvider
    {
        void MeleeAttack();
        void WeaponFire();
        void SwapWeapon(bool isUpwards);
    }
    
    [DefaultExecutionOrder(101)] // Cinemachine uses a modified script execution of 100, this ensures we run after cinemachine updates the camera position
    public class WeaponAnimationController : MonoBehaviour, IWeaponAnimationProvider
    {
        private const string MELEE_PARAM = "Melee";
        private const string SHOOT_PISTOL_PARAM = "Shoot_Pistol";
        private const string SWAP_WEAPON_DOWN_PARAM = "Swap_Weapon_Down";
        private const string SWAP_WEAPON_UP_PARAM = "Swap_Weapon_Up";
        
        [Resolve][SerializeField] private GameObject _weaponHolder = null;
        [Resolve][SerializeField] private Animator _weaponAnimator = null;
        private Camera _mainCam;
        
        private Func<Vector3> _characterVelocity = null;
        private Func<bool> _isWallRunning = null;
        private Func<bool> _isSliding = null;
        private Func<bool> _isDashing = null;
        private Func<bool> _isGrounded = null;

        public void Initialize(
            Func<Vector3> characterVelocity, 
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

        private void Start()
        {
            _mainCam = Camera.main;
        }

        private void LateUpdate()
        {
            var camTransform = _mainCam.transform;
            _weaponHolder.transform.SetPositionAndRotation(
                camTransform.position,
                camTransform.rotation);
        }
        
        private bool CanBobWeapon()
        {
            return (_isWallRunning() || _isGrounded()) 
                   && !_isDashing()
                   && !_isSliding();
        }

        public void MeleeAttack()
        {
            _weaponAnimator.SetTrigger(MELEE_PARAM);
        }

        public void WeaponFire()
        {
            _weaponAnimator.SetTrigger(SHOOT_PISTOL_PARAM);
        }

        public void SwapWeapon(bool isUpwards)
        {
            var param = isUpwards ? SWAP_WEAPON_UP_PARAM : SWAP_WEAPON_DOWN_PARAM;
            _weaponAnimator.SetTrigger(param);
        }
    }
}
