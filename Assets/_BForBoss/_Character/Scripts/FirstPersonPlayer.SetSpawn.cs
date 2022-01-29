using UnityEngine;

namespace Perigon.Character
{
    public interface ICharacterSpawn
    {
        void SpawnAt(Vector3 position, Quaternion facing);
    }

    public partial class FirstPersonPlayer: ICharacterSpawn
    {
        void ICharacterSpawn.SpawnAt(Vector3 position, Quaternion facing)
        {
            SetVelocity(Vector3.zero);
            SetPosition(position);
            rootPivot.rotation = facing;
            eyePivot.rotation = facing;
        }
    }
}
