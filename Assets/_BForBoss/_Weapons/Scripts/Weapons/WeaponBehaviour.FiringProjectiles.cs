using Perigon.Utility;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class WeaponBehaviour
    {
        private void FireProjectiles(int numberOfBullets)
        {
            for (int i = 0; i < numberOfBullets; i++)
            {
                var bullet = _bulletSpawner
                    .SpawnBullet(_weapon.TypeOfBullet);
                bullet.OnBulletHitWall = OnBulletHitWall;
                bullet.SetSpawnAndDirection(_firePoint.position, GetDirectionOfShot());
                bullet.OnBulletHitEntity += HandleOnBulletHitEntity;
                bullet.OnBulletDeactivate += HandleOnBulletDeactivate;
            }
        }
        
        private Vector3 GetDirectionOfShot()
        {
            var camRay = MainCamera.ViewportPointToRay(CenterOfCameraPosition);
            Vector3 targetPoint;
            if (Physics.Raycast(camRay, out var hit, Mathf.Infinity, ~TagsAndLayers.Layers.TriggerArea))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = camRay.GetPoint(RAYCAST_DISTANCE_LIMIT);
            }

            return _weapon.GetShootDirection(_firePoint.position, targetPoint, _timeSinceFire);
        }
        
        private void HandleOnBulletHitEntity(IBullet bullet, bool isDead)
        {
            _crosshair.ActivateHitMarker(isDead);
            bullet.OnBulletHitEntity -= HandleOnBulletHitEntity;
            bullet.OnBulletDeactivate -= HandleOnBulletDeactivate;
        }

        private void HandleOnBulletDeactivate(IBullet bullet)
        {
            bullet.OnBulletHitEntity -= HandleOnBulletHitEntity;
            bullet.OnBulletDeactivate -= HandleOnBulletDeactivate;
        }
    }
}
