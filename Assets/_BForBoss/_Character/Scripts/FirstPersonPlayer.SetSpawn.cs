using UnityEngine;

namespace BForBoss
{
    public interface ICharacterSpawn
    {
        void SpawnAt(Vector3 position, Quaternion facing);
    }

    public partial class FirstPersonPlayer: ICharacterSpawn
    {
        public void SpawnAt(Vector3 position, Quaternion facing)
        {
            SetVelocity(Vector3.zero);
            SetPosition(position);
            // TODO rotation of camera??
        }
    }
}