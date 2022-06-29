using UnityEngine;

namespace Perigon.Entities
{
    public class PlaceholderObstacleDamageBehaviour : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            var lifeCycle = other.gameObject.GetComponent<LifeCycleBehaviour>();
            if (lifeCycle != null)
            {
                lifeCycle.Damage(50);
            }
        }
    }
}
