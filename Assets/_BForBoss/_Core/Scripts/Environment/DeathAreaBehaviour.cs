using FMODUnity;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class DeathAreaBehaviour : MonoBehaviour
    {
        [SerializeField]
        private EventReference _onDeathSFX;
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(TagsAndLayers.Tags.Player))
            {
                if(!_onDeathSFX.IsNull)
                    RuntimeManager.PlayOneShot(_onDeathSFX, other.gameObject.transform.position);
                StateManager.Instance.SetState(State.PreGame);
            }
        }
    }
}
