using UnityEngine;

namespace BForBoss
{
    public enum TutorialState
    {
        Basic,
        Energy
    }
    
    public class TutorialViewsManager : MonoBehaviour
    {
        public static TutorialViewsManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void Show(TutorialState state)
        {
            
        }
    }
}
