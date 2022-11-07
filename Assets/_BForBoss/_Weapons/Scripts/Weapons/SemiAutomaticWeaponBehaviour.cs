using UnityEngine;

namespace Perigon.Weapons
{
    public class SemiAutomaticWeaponBehaviour : WeaponBehaviour
    {
        protected override void OnFireInputAction(bool isFiring)
        {
            if (isFiring)
            {
                _weapon.TryFire();
            }
        }

        protected override void Update()
        {
            _weapon.DecrementElapsedTimeRateOfFire(Time.deltaTime, Time.timeScale);
            _weapon.ReloadWeaponCountDownIfNeeded(Time.deltaTime, Time.timeScale);
        }
    }
}
