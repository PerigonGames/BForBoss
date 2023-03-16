namespace Perigon.Weapons
{
    public class SemiAutomaticWeaponBehaviour : WeaponBehaviour
    {
        protected override void OnFireInputAction(bool isFiring)
        {
            if (isFiring && (_externalShootingCases?.CanShoot ?? true))
            {
                _weapon.TryFire();
            }
        }
    }
}
