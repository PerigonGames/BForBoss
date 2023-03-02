using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class WeaponBehaviour
    {
        private void FireRayCastBullets()
        {
            var camOrigin = MainCamera.transform.position;
            for (int i = 0; i < _weaponConfigurationData.BulletsPerShot; i++)
            {
                var forwardAngle = _weapon.GetShootDirection(_weaponConfigurationData.GetBulletSpreadRate(_timeSinceFire));
                RayCastBullet(camOrigin, forwardAngle);
            }
        }

        private void RayCastBullet(Vector3 from, Vector3 towards)
        {
            if (Physics.Raycast(from, MainCamera.transform.TransformDirection(towards), out var hit, Mathf.Infinity, ~_rayCastBulletLayerMask.value))
            {
                if (hit.collider.TryGetComponent(out IWeaponHolder weaponHolder))
                {
                    weaponHolder.DamageBy(_weaponConfigurationData.DamagePerRayCast);
                    _crossHairProvider.ActivateHitMarker(!weaponHolder.IsAlive);
                }
                else
                {
                    OnBulletHitWall(hit.point, hit.normal);
                }
            }
        }
    }
}
