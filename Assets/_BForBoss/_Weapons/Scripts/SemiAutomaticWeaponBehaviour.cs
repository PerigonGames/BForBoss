using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public class SemiAutomaticWeaponBehaviour : WeaponBehaviour
    {
        protected override void OnFire(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (CanShoot)
                {
                    Fire();
                }
            }
        }

        protected override void Update()
        {
            if (_elapsedRateOfFire > 0)
            {
                _elapsedRateOfFire -= Time.deltaTime;
            }
        }
    }
}
