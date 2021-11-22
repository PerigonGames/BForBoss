using UnityEngine;

namespace BForBoss
{
    public class ResetGameBehaviour : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            StateManager.Instance.SetState(State.PreGame);
        }
    }
}