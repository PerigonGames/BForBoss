using Perigon.Entities;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IApplyDamage
    {
        public void HitObject(Collider col);
    }
    
    public abstract partial class BulletBehaviour : IApplyDamage
    {
        public void HitObject(Collider col)
        {
            if(col.TryGetComponent(out LifeCycleBehaviour lifeCycle))
            {
                lifeCycle.Damage(_properties.Damage);
            }
        }
    }
}
