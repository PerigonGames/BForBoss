using UnityEngine;

namespace Perigon.Entities
{
    public class HealingBehaviour : MonoBehaviour
    {
        [SerializeField] private float _healAmount = 50f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerLifeCycleBehaviour lifeCycle))
            {
                lifeCycle.HealBy(_healAmount);
            }
        }
    }
}
