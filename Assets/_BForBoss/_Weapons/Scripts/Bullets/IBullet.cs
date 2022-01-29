using System;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IBullet
    {
        void SetSpawnAndDirection(Vector3 location, Vector3 normalizedDirection);
        event Action OnBulletSpawn;
        event Action<IBullet, bool> OnBulletHitEntity;
        event Action<IBullet> OnBulletDeactivate;
    }
}
