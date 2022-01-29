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
                lifeCycle.Damage(BulletProperties.Damage);
                _onBulletHitEntity?.Invoke(this, !lifeCycle.IsAlive);
            }
            else
            {
                SpawnWallHitPrefab(hitPosition, hitNormal);
            }
        }
    }
}
