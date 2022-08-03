using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Perigon.Weapons
{
    public class WeaponAnimationController : MonoBehaviour
    {
        private const float ACCUMULATED_RECOIL_PERCENTAGE = 0.99F;

        [Resolve] [SerializeField] private GameObject _weaponHolder = null;
        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        
        private Func<Vector3> _characterVelocity = null;
        private Func<float> _characterMaxSpeed = null;
        private Func<bool> _isWallRunning = null;
        private Func<bool> _isSliding = null;
        private Func<bool> _isDashing = null;
        private Func<bool> _isGrounded = null;

        [Title("Weapon Bob Properties")] 
        [SerializeField]
        private float _hipFireBobAmount = 0.05f;
        [SerializeField] 
        private float _wallRunningBobMultiplier = 0.5f;
        [InfoBox("Frequency at which weapon will move around screen when moving")]
        [SerializeField]
        private float _weaponBobFrequency = 10f;
        [InfoBox("How fast the weapon bob is applied, bigger value is faster")]
        [SerializeField] 
        private float _weaponBobSharpness = 10f;

        [Title("Recoil Properties")]
        [SerializeField] 
        private float _maxRecoilDistance = 0.5f;
        [InfoBox("This will affect how fast the recoil moves the weapon, the bigger the value, the fastest")]
        [SerializeField]
        private float _recoilSharpness = 50f;
        [InfoBox("How fast the weapon goes back to it's original position after the recoil is finished")]
        [SerializeField]
        private float _recoilRestitutionSharpness = 10f;

        private float _weaponBobFactor = 0;
        private Vector3 _weaponBobLocalPosition;
        private Vector3 _accumulatedRecoil;
        private Vector3 _weaponRecoilLocalPosition;

        public void Initialize(
            Func<Vector3> characterVelocity, 
            Func<float> characterMaxSpeed, 
            Func<bool> isWallRunning,
            Func<bool> isGrounded,
            Func<bool> isSliding,
            Func<bool> isDashing)
        {
            _characterVelocity = characterVelocity;
            _characterMaxSpeed = characterMaxSpeed;
            _isGrounded = isGrounded;
            _isSliding = isSliding;
            _isDashing = isDashing;
            _isWallRunning = isWallRunning;
        }

        private void Start()
        {
            _equipmentBehaviour.Weapons.ForEach(weapon => weapon.OnFireWeapon += OnWeaponFired);
        }

        private void OnDestroy()
        {
            _equipmentBehaviour.Weapons.ForEach(weapon => weapon.OnFireWeapon -= OnWeaponFired);
        }

        private void OnWeaponFired(int _)
        {
            _accumulatedRecoil += Vector3.back * _equipmentBehaviour.CurrentWeapon.VisualRecoilForce;
            _accumulatedRecoil = Vector3.ClampMagnitude(_accumulatedRecoil, _maxRecoilDistance);
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
            UpdateBobbingWeaponOnMovement();
            UpdateWeaponRecoil();
            
            var camTransform = Camera.main.transform;
            _weaponHolder.transform.SetPositionAndRotation(
                camTransform.TransformPoint(_weaponBobLocalPosition + _weaponRecoilLocalPosition),
                camTransform.rotation);
        }

        private void UpdateBobbingWeaponOnMovement()
        {
            var characterVelocity = _characterVelocity();
            var characterMovementFactor = 0f;
            if (CanBobWeapon())
            {
                characterMovementFactor =
                    Mathf.Clamp01(characterVelocity.magnitude / _characterMaxSpeed());
            }
            
            _weaponBobFactor = Mathf.Lerp(_weaponBobFactor, characterMovementFactor, _weaponBobSharpness * Time.deltaTime);
            var bobAmount = _hipFireBobAmount * (_isWallRunning() ? _wallRunningBobMultiplier : 1);
            
            var hBobValue = Mathf.Sin(Time.time * _weaponBobFrequency) * bobAmount * _weaponBobFactor;
            // Trignometric Graph Tranformation: y = A * Sin(b * x - c) + d
            var vBobValue = (Mathf.Sin(Time.time * _weaponBobFrequency * 2f) * 0.5f + 0.5f) * _weaponBobFactor * bobAmount;
            var xPosition = hBobValue;
            var yPosition = Mathf.Abs(vBobValue);

            _weaponBobLocalPosition = new Vector3(xPosition, yPosition, 0);
        }
        
        private bool CanBobWeapon()
        {
            return (_isWallRunning() || _isGrounded()) 
                   && !_isDashing()
                   && !_isSliding();
        }

        private void UpdateWeaponRecoil()
        {
            if (_weaponRecoilLocalPosition.z >= _accumulatedRecoil.z * ACCUMULATED_RECOIL_PERCENTAGE)
            {
                _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, _accumulatedRecoil,
                    _recoilSharpness * Time.deltaTime);
            }
            else
            {
                _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, Vector3.zero,
                    _recoilRestitutionSharpness * Time.deltaTime);
                _accumulatedRecoil = _weaponRecoilLocalPosition;
            }
        }
    }
}
