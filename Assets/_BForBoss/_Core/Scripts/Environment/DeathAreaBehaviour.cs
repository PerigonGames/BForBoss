using UnityEngine;

namespace BForBoss
{
    public class DeathAreaBehaviour : MonoBehaviour
    {
        [SerializeField] private string _deathAreaName = "deathArea";
        
        private void OnCollisionEnter(Collision other)
        {
            StateManager.Instance.SetState(State.Death);
        }
    }
}
