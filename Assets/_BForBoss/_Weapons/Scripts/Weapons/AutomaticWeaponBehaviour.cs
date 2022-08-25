using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public class AutomaticWeaponBehaviour : WeaponBehaviour
    {
        protected override void OnFireInputAction(bool isFiring)
        {
            _isFiring = isFiring;
            
            if (!_isFiring)
            {
                _timeSinceFire = 0;
            }
        }

        protected override void Update()
        {
            if (_isFiring)
            {
                _timeSinceFire += _weapon.ScaledDeltaTime(Time.deltaTime, Time.timeScale);
                _weapon.FireIfPossible();
            }
            _weapon.DecrementElapsedTimeRateOfFire(Time.deltaTime, Time.timeScale);
            _weapon.ReloadWeaponCountDownIfNeeded(Time.deltaTime, Time.timeScale);
        }
    }
}
