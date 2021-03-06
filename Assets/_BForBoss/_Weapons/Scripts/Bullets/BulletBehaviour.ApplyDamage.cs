using UnityEngine;

namespace Perigon.Weapons
{
    public interface IWeaponHolder
    {
        void DamagedBy(float amount);
        bool IsAlive { get; }
    }
    
    public abstract partial class BulletBehaviour
    {
        protected void HitObject(Collider col, Vector3 hitPosition, Vector3 hitNormal)
        {
            if(col.TryGetComponent(out IWeaponHolder weaponHolder))
            {
                weaponHolder.DamagedBy(_properties.Damage);
                OnBulletHitEntity?.Invoke(this, !weaponHolder.IsAlive);
            }
            else
            {
                OnBulletHitWall?.Invoke(hitPosition, hitNormal);
            }
        }
    }
}
