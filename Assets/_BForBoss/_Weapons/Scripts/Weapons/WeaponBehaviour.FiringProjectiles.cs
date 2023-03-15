namespace Perigon.Weapons
{
    public abstract partial class WeaponBehaviour
    {
        private void FireProjectiles()
        {
            var spreadAngle =
                _weapon.GetSpreadDirection(_weaponConfigurationData.GetBulletSpreadRate(_timeSinceFire));
            var direction = MainCamera.transform.TransformDirection(spreadAngle);

            for (int i = 0; i < _weaponConfigurationData.BulletsPerShot; i++)
            {
                var bullet = _bulletSpawner
                    .SpawnBullet(_weaponConfigurationData.BulletType);
                bullet.OnBulletHitWall = OnBulletHitWall;
                bullet.SetSpawnAndDirection(_firePoint.position, direction);
                bullet.OnBulletHitEntity += HandleOnBulletHitEntity;
                bullet.OnBulletDeactivate += HandleOnBulletDeactivate;
            }
        }
        
        private void HandleOnBulletHitEntity(IBullet bullet, bool isDead)
        {
            _crossHairBehaviour.ActivateHitMarker(isDead);
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
