using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface ICharacterMovement
    {
        Vector3 CharacterVelocity { get; }
        float CharacterMaxSpeed { get; }
        bool IsGrounded { get; }
        bool IsDashing { get; }
        bool IsSliding { get; }
    }
    
    public class WeaponsManager : MonoBehaviour
    {
        [Resolve] [SerializeField] private GameObject _weaponHolder = null;
        private ICharacterMovement _characterMovement = null;
        
        [Title("Properties")] 
        [SerializeField]
        private float _hipFireBobAmount = 0.05f;
        // Frequency at which weapon will move around screen when moving
        [SerializeField]
        private float _weaponBobFrequency = 10f;
        // How fast the weapon bob is applied, bigger value is faster
        [SerializeField] 
        private float _weaponBobSharpness = 10f;

        private float _weaponBobFactor = 0;

        public void Initialize(ICharacterMovement characterMovement)
        {
            _characterMovement = characterMovement;
        }
        
        private void LateUpdate()
        {
            var characterVelocity = _characterMovement.CharacterVelocity;
            var characterMovementFactor = 0f;
            if (CanBobWeapon())
            {
                characterMovementFactor =
                    Mathf.Clamp01(characterVelocity.magnitude / _characterMovement.CharacterMaxSpeed);
            }
            
            _weaponBobFactor = Mathf.Lerp(_weaponBobFactor, characterMovementFactor, _weaponBobSharpness * Time.deltaTime);
            
            var hBobValue = Mathf.Sin(Time.time * _weaponBobFrequency) * _hipFireBobAmount * _weaponBobFactor;
            var vBobValue = (Mathf.Sin(Time.time * _weaponBobFrequency * 2f) * 0.5f + 0.5f) * _weaponBobFactor * _hipFireBobAmount;
            var xPosition = hBobValue;
            var yPosition = Mathf.Abs(vBobValue);

            _weaponHolder.transform.localPosition = new Vector3(xPosition, yPosition, 0);
        }

        private bool CanBobWeapon()
        {
            return _characterMovement.IsGrounded && !_characterMovement.IsDashing && !_characterMovement.IsSliding;
        }
    }
}
