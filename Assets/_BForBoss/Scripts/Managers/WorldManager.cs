using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace BForBoss
{
    public class WorldManager : MonoBehaviour
    {
        [Title("Component")] [SerializeField] private TimeManager _timeManager = null;
        [SerializeField] private CheckpointManager _checkpointManager = null;
        [SerializeField] private FirstPersonPlayer _player = null;

        [Title("User Interface")] [SerializeField]
        private TimerViewBehaviour _timerView = null;

        [SerializeField] private InputSettingsViewBehaviour _inputSettingsView = null;
        [SerializeField] private InputUsernameViewBehaviour _uploadView = null;

        [Title("Effects")] [SerializeField] private Volume _deathVolume = null;

        // This probably best placed inside its own utility section
        private StateManager _stateManager = StateManager.Instance;

        // This is probably best kept within its own utility section
        private PostProcessingVolumeWeightTool _postProcessingVolumeWeightTool = null;

        private readonly TimeManagerViewModel _timeManagerViewModel = new TimeManagerViewModel();
        private InputSettingsViewModel _inputSettingsViewModel = null;
        private ICharacterSpawn _character = null;
        private readonly UploadPlayerScores _uploadPlayerScores = new UploadPlayerScores();

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

            _inputSettingsViewModel = new InputSettingsViewModel(_player);
        }

        private void Start()
        {
            _player.Initialize();
            _checkpointManager.Initialize();
            _uploadView.Initialize(new InputUsernameViewModel(new LockMouseUtility(_player)));
            _timeManager.Initialize(_timeManagerViewModel);
            _timerView.Initialize(_timeManagerViewModel);
            _stateManager.SetState(State.PreGame);
            _inputSettingsView.Initialize(_inputSettingsViewModel);
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
                    _uploadPlayerScores.SetTime(_timeManagerViewModel.CurrentGameTime);
                    _uploadPlayerScores.SetInput("Controller");
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
