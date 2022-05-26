using System;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IBullet
    {
        void SetSpawnAndDirection(Vector3 location, Vector3 normalizedDirection);
        Action<Vector3, Vector3> OnBulletHitWall { get; set; }
        event Action<IBullet, bool> OnBulletHitEntity;
        event Action<IBullet> OnBulletDeactivate;
    }
}
