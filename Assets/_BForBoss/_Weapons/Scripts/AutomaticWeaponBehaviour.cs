using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public class AutomaticWeaponBehaviour : WeaponBehaviour
    {
        private bool _isFiring;
        
        protected override void OnFireInputAction(InputAction.CallbackContext context)
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
            if (_isFiring)
            {
                _weapon.FireIfPossible();
            }
            _weapon.DecrementElapsedTimeRateOfFire(Time.deltaTime);
            _weapon.ReloadWeaponCountDownIfNeeded(Time.deltaTime);
        }
    }
}
