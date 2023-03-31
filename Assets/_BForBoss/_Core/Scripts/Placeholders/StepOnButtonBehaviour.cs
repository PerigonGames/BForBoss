using Perigon.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace BForBoss
{
    public class StepOnButtonBehaviour : MonoBehaviour
    {
        [SerializeField]
        public UnityEvent _executableEvent;
     
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(TagsAndLayers.Tags.Player))
            {
                _executableEvent?.Invoke();
            }
        }
    }
}
