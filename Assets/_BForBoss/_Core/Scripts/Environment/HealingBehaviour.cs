using UnityEngine;

namespace Perigon.Entities
{
    public class HealingBehaviour : MonoBehaviour
    {
        [SerializeField] private float _healAmount = 50f;
        
        private void OnTriggerEnter(Collider other)
        {
            var playerLifeCycle = other.transform.GetComponent<PlayerLifeCycleBehaviour>();
            if (playerLifeCycle != null)
            {
                playerLifeCycle.Heal(_healAmount);
            }
            
            gameObject.SetActive(false);
        }
    }
}
