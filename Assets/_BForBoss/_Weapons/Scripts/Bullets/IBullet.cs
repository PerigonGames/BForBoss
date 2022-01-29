using UnityEngine;

namespace Perigon.Weapons
{
    public interface IBullet
    {
        void SetSpawnAndDirection(Vector3 location, Vector3 normalizedDirection);
    }
}
