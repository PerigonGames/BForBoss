using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public class AutomaticWeaponBehaviour : WeaponBehaviour
    {
        protected override void OnFireInputAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isFiring = true;
            }
            
            if (context.canceled)
            {
                _isFiring = false;
                _timeSinceFire = 0;
            }
        }

        protected override void Update()
        {
            if (_isFiring)
            {
                _timeSinceFire += Time.deltaTime;
                _weapon.FireIfPossible();
            }
            _weapon.DecrementElapsedTimeRateOfFire(Time.deltaTime);
            _weapon.ReloadWeaponCountDownIfNeeded(Time.deltaTime);
        }
    }
}
