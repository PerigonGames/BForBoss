using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public class AutomaticWeaponBehaviour : WeaponBehaviour
    {
        private bool _isFiring = false;
        
        protected override void OnFire(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isFiring = true;
            }
            
            if (context.canceled)
            {
                _isFiring = false;
            }
        }

        protected override void Update()
        {
            if (!_isFiring)
            {
                return;
            }

            _weapon.DecrementElapsedTimeRateOfFire(Time.deltaTime);
            _weapon.FireIfPossible();
        }
    }
}
