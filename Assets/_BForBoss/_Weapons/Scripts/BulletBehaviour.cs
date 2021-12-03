using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract class BulletBehaviour : MonoBehaviour, IBullet
    {
        public void SetSpawnAndDirection(Vector3 location, Vector3 normalizedDirection)
        {
            throw new System.NotImplementedException();
        }
    }
}
