using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace BForBoss
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private TimeManager _timeManager = null;
        [SerializeField] private CheckpointManager _checkpointManager = null;
        [SerializeField] private FirstPersonPlayer _player = null;
        [SerializeField] private TimerViewBehaviour _timerView = null;

        [Title("Effects")] 
        [SerializeField] private Volume _deathVolume = null;
        
        private StateManager _stateManager = StateManager.Instance;
        private TimeManagerViewModel _timeManagerViewModel = new TimeManagerViewModel();
        private ICharacterSpawn _character = null;
        private PostProcessingVolumeWeightTool _postProcessingVolumeWeightTool = null;
        
        private void CleanUp()
        {
            Debug.Log("Cleaning Up");
        }
        
        private void Reset()
        {
            _timeManager.Reset();
            _checkpointManager.Reset();
            _stateManager.SetState(State.Play);
            _timerView.Reset();
            _character.SpawnAt(_checkpointManager.CheckpointPosition, _checkpointManager.CheckpointRotation);
        }

        private void Awake()
        {
            _stateManager.OnStateChanged += HandleStateChange;
            _character = _player;
            _postProcessingVolumeWeightTool = new PostProcessingVolumeWeightTool(_deathVolume, 0.1f, 0f, 0.1f);
        }

        private void Start()
        {
            _checkpointManager.Initialize();
            _timeManager.Initialize(_timeManagerViewModel);
            _timerView.Initialize(_timeManagerViewModel);
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
                    break;
                }
                case State.Pause:
                {
                    break;
                }
                case State.EndRace:
                {
                    _timeManagerViewModel.StopTimer();
                    break;
                }
                case State.Death:
                {
                    _postProcessingVolumeWeightTool.InstantDistortAndRevert();
                    _character.SpawnAt(_checkpointManager.CheckpointPosition, _checkpointManager.CheckpointRotation);
                    _stateManager.SetState(State.Play);
                    break;
                }
            }
        }
        
    }
}
