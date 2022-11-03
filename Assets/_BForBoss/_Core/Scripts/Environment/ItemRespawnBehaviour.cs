using UnityEngine;

namespace BForBoss
{
    public abstract class ItemRespawnBehaviour : MonoBehaviour
    {
        protected bool _canRespawn = false;

        public virtual bool CanRespawn => _canRespawn;

        public virtual void Reset()
        {
            _canRespawn = false;
        }

    }
}
