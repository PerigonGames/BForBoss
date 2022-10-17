using UnityEngine;

namespace Perigon.Entities
{
    public class PlaceholderObstacleDamageBehaviour : MonoBehaviour
    {
        [SerializeField] private float _damage = 50;
        private void OnCollisionEnter(Collision other)
        {
            var lifeCycle = other.gameObject.GetComponent<LifeCycleBehaviour>();
            if (lifeCycle != null)
            {
                lifeCycle.DamageBy(_damage);
            }
        }
    }
}
