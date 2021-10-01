using UnityEngine;

namespace BForBoss
{
    public class WorldManager : MonoBehaviour
    {
        private StateManager _stateManager = StateManager.Instance;

        public void CleanUp()
        {
            Debug.Log("Cleaning Up");
        }
        
        public void Reset()
        {
            Debug.Log("Resetting");
        }
        
        private void Awake()
        {
            _stateManager.OnStateChanged += HandleStateChange;
        }
        
        private void OnDestroy()
        {
            _stateManager.OnStateChanged -= HandleStateChange;
        }


        private void HandleStateChange(State newState)
        {
            switch (newState)
            {
                case State.PreGame:
                {
                    CleanUp();
                    _stateManager.SetState(State.Play);
                    break;
                }
                case State.Play:
                {
                    Reset();
                    break;
                }
                case State.Pause:
                {
                    break;
                }
                case State.Death:
                {
                    //Handle Death
                    _stateManager.SetState(State.PreGame);
                    break;
                }
            }
        }
        
    }
}
