using System;
using Perigon.Utility;
using Sirenix.Utilities;
using UnityEngine;

namespace Perigon.Weapons
{
    [DefaultExecutionOrder(101)] // Cinemachine uses a modified script execution of 100, this ensures we run after cinemachine updates the camera position
    public class WeaponAnimationController : MonoBehaviour
    {
        [Resolve] [SerializeField] private GameObject _weaponHolder = null;
        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        [SerializeField] private Animator _weaponAnimator = null;
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
            _equipmentBehaviour.Weapons.ForEach(weapon => weapon.OnFireWeapon += OnWeaponFired);
            _mainCam = Camera.main;
        }

        private void OnDestroy()
        {
            _equipmentBehaviour.Weapons.ForEach(weapon => weapon.OnFireWeapon -= OnWeaponFired);
        }

        private void OnWeaponFired(int _)
        {
            _weaponAnimator.SetTrigger("Shoot_Pistol");
            FMODUnity.RuntimeManager.PlayOneShot(_equipmentBehaviour.WeaponShotAudio, transform.position);
        }

        private void Awake()
        {
            if (_equipmentBehaviour == null)
            {
                PanicHelper.Panic(new Exception("Equipment Behaviour missing from WeaponAnimationController"));
            }
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
    }
}
