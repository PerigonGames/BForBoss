using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public class AutomaticWeaponBehaviour : WeaponBehaviour
    {
        protected override void OnFire(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isFiring = true;
            }
            
            if (context.canceled)
            {
                _isFiring = false;
                _elapsedRateOfFire = 0;
            }
        }

        protected override void Update()
        {
            if (!_isFiring)
            {
                return;
            }

            _elapsedRateOfFire -= Time.deltaTime;
            if (CanShoot)
            {
                Fire();
            }
        }
    }
}
