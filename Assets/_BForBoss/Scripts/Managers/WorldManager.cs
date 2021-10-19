using UnityEngine;

namespace BForBoss
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private TimeManager _timeManager = null;
        [SerializeField] private CheckpointManager _checkpointManager = null;
        [SerializeField] private FirstPersonPlayer _player = null;
        private StateManager _stateManager = StateManager.Instance;

        private void CleanUp()
        {
            Debug.Log("Cleaning Up");
        }
        
        private void Reset()
        {
            _timeManager.Reset();
            _checkpointManager.Reset();
            _stateManager.SetState(State.Play);
        }

        private void Awake()
        {
            _stateManager.OnStateChanged += HandleStateChange;
        }

        private void Start()
        {
            _checkpointManager.Initialize();
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
                    _player.transform.position = _checkpointManager.CurrentCheckpoint;
                    _stateManager.SetState(State.Play);
                    break;
                }
            }
        }
        
    }
}
