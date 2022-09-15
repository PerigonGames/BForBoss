using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class WeaponBehaviour
    {
        private void FireRayCastBullets(int numberOfBullets)
        {
            var camOrigin = MainCamera.transform.position;
            for (int i = 0; i < numberOfBullets; i++)
            {
                var forwardAngle = _weapon.GetShootDirection(_timeSinceFire);
                RayCastBullet(camOrigin, forwardAngle);
            }
        }

        private void RayCastBullet(Vector3 from, Vector3 towards)
        {
            if (Physics.Raycast(from, MainCamera.transform.TransformDirection(towards), out var hit, Mathf.Infinity, ~_rayCastBulletLayerMask.value))
            {
                if (hit.collider.TryGetComponent(out IWeaponHolder weaponHolder))
                {
                    weaponHolder.DamagedBy(_weapon.DamagePerRayCast);
                    _crosshair.ActivateHitMarker(!weaponHolder.IsAlive);
                }
                else
                {
                    OnBulletHitWall(hit.point, hit.normal);
                }
            }
        }
    }
}
