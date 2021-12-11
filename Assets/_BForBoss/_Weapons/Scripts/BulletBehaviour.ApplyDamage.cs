using Perigon.Entities;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class BulletBehaviour
    {
        protected void HitObject(Collider col)
        {
            if(col.TryGetComponent(out LifeCycleBehaviour lifeCycle))
            {
                lifeCycle.Damage(_properties.Damage);
            }
        }
    }
}
