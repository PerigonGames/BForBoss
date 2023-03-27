using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class DeathAreaBehaviour : MonoBehaviour
    {        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(TagsAndLayers.Tags.Player))
            {
                StateManager.Instance.SetState(State.PreGame);
            }
        }
    }
}
