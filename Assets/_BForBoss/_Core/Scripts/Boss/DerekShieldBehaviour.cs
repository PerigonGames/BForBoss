using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Collider))]
    public class DerekShieldBehaviour : MonoBehaviour
    {
        public void ToggleShield(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}
