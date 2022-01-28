using Perigon.Entities;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class BulletBehaviour
    {
        protected void HitObject(Collider col, Vector3 hitPosition, Vector3 hitNormal)
        {
            if(col.TryGetComponent(out LifeCycleBehaviour lifeCycle))
            {
                lifeCycle.Damage(_properties.Damage);
                var isDead = !lifeCycle.IsAlive;
                OnBulletHitEntity?.Invoke(this, isDead);
            }
            else
            {
                SpawnWallHitPrefab(hitPosition, hitNormal);
            }
        }
    }
}
