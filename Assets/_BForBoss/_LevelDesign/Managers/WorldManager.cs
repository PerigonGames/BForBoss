using UnityEngine;
using UnityEngine.InputSystem;

namespace BForBoss
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private TimeManager _timeManager;
        [SerializeField] private CheckpointManager _checkpointManager;
        private StateManager _stateManager = StateManager.Instance;

        public void CleanUp()
        {
            Debug.Log("Cleaning Up");
        }
        
        public void Reset()
        {
            _timeManager.Reset();
            _checkpointManager.Reset();
        }
        
        private void Awake()
        {
            _checkpointManager.Initialize();
            _stateManager.OnStateChanged += HandleStateChange;
            _stateManager.SetState(State.PreGame);
        }
        
        //Temporary way to check if Checkpoint system works - Todo: Remove when death plane is in place
        private void Update()
        {
            if (Keyboard.current[Key.R].wasPressedThisFrame)
            {
                _stateManager.SetState(State.Death);
            }
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
                    if (!_timeManager.IsTimerTracking)
                    {
                        _timeManager.StartTimer();
                    }
                    
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
                    GameObject player = GameObject.FindWithTag(Tags.Player);
                    player.transform.position = _checkpointManager.CurrentCheckpoint;
                    _stateManager.SetState(State.Play);
                    break;
                }
            }
        }
        
    }
}
