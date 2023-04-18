using System;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IBullet
    {
        void SetSpawnAndDirection(Vector3 location, Vector3 normalizedDirection);
        Transform HomingTarget { set; }
        Action<Vector3, Vector3> OnBulletHitWall { set; }
        event Action<IBullet, bool> OnBulletHitEntity;
        event Action<IBullet> OnBulletDeactivate;
    }
}
