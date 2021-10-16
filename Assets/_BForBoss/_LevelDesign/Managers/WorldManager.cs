using UnityEngine;

namespace BForBoss
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private TimeManager _timeManager;
        private StateManager _stateManager = StateManager.Instance;

        public void CleanUp()
        {
            Debug.Log("Cleaning Up");
        }
        
        public void Reset()
        {
            Debug.Log("Resetting");
            _timeManager.Reset();
        }
        
        private void Awake()
        {
            _stateManager.OnStateChanged += HandleStateChange;
            _stateManager.SetState(State.PreGame);
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
                    Reset();
                    _stateManager.SetState(State.Play);
                    break;
                }
                case State.Play:
                {
                    _timeManager.StartTimer();
                    break;
                }
                case State.Pause:
                {
                    break;
                }
                case State.EndRace:
                {
                    _timeManager.StopTimer();
                    Debug.Log($"The time taken was {_timeManager.CurrentGameTime} seconds");
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
