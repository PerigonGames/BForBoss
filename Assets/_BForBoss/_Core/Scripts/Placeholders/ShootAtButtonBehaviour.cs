using Perigon.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace BForBoss
{
    public class ShootAtButtonBehaviour : MonoBehaviour, IBulletCollision
    {
        [SerializeField]
        private UnityEvent<Vector3> _executableEvent;
        
        public void OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            _executableEvent?.Invoke(collisionPoint);
        }
    }
}
