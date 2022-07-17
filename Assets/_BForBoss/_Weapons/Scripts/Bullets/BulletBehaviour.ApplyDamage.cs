using UnityEngine;

namespace Perigon.Weapons
{
    public interface ILifeCycleBehaviour
    {
        void Damage(float amount);
        bool IsAlive { get; }
    }
    
    public abstract partial class BulletBehaviour
    {
        protected void HitObject(Collider col, Vector3 hitPosition, Vector3 hitNormal)
        {
            if(col.TryGetComponent(out ILifeCycleBehaviour lifeCycle))
            {
                lifeCycle.Damage(_properties.Damage);
                OnBulletHitEntity?.Invoke(this, !lifeCycle.IsAlive);
            }
            else
            {
                OnBulletHitWall?.Invoke(hitPosition, hitNormal);
            }
        }
    }
}
