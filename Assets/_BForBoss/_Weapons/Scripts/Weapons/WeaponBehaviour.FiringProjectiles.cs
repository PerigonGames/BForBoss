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
                bullet.SetSpawnAndDirection(_firePoint.position, GetDirectionOfShot());
                bullet.OnBulletHitEntity += HandleOnBulletHitEntity;
                bullet.OnBulletDeactivate += HandleOnBulletDeactivate;
            }
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
