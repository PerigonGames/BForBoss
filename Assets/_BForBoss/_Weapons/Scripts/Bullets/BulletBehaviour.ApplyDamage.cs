using UnityEngine;

namespace Perigon.Weapons
{
    public interface IWeaponHolder
    {
        void DamageBy(float amount);
        bool IsAlive { get; }
    }

    public interface IBulletCollision
    {
        void OnCollided(Vector3 collisionPoint, Vector3 collisionNormal);
    }
    
    public abstract partial class BulletBehaviour
    {
        protected void HitObject(Collider col, Vector3 hitPosition, Vector3 hitNormal)
        {
            if(col.TryGetComponent(out IWeaponHolder weaponHolder))
            {
                weaponHolder.DamageBy(_properties.Damage);
                OnBulletHitEntity?.Invoke(this, !weaponHolder.IsAlive);
            } 
            else if (col.TryGetComponent(out IBulletCollision bulletCollision))
            {
                bulletCollision.OnCollided(hitPosition, hitNormal);    
            }
            else if (col.attachedRigidbody != null && col.attachedRigidbody.TryGetComponent(out IBulletCollision bulletRbCollision))
            {
                bulletRbCollision.OnCollided(hitPosition, hitNormal);  
            }
            else
            {
                OnBulletHitWall?.Invoke(hitPosition, hitNormal);
            }
        }
    }
}
