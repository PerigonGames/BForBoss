using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace BForBoss
{
    public class WorldManager : MonoBehaviour
    {
        [Title("Component")] 
        [SerializeField] private TimeManager _timeManager = null;
        [SerializeField] private CheckpointManager _checkpointManager = null;
        [SerializeField] private FirstPersonPlayer _player = null;

        [Title("User Interface")]
        [SerializeField] private PauseMenu _pauseMenu;
        [SerializeField] private TimerViewBehaviour _timerView = null;
        [SerializeField] private SettingsViewBehaviour _settingsViewBehaviour = null;
        [SerializeField] private ForcedInputUsernameViewBehaviour _forcedUploadView = null;
        [SerializeField] private LeaderboardPanelBehaviour _leaderboardPanel = null;

        [Title("Effects")] 
        [SerializeField] private Volume _deathVolume = null;
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [Title("Debug")]
        [SerializeField] private GameObject _debugCanvas;
#endif

        private const Key PauseKey = Key.Escape;
        // This probably best placed inside its own utility section
        private readonly StateManager _stateManager = StateManager.Instance;
        private readonly PerigonAnalytics _perigonAnalytics = PerigonAnalytics.Instance;
        // This is probably best kept within its own utility section
        private PostProcessingVolumeWeightTool _postProcessingVolumeWeightTool = null;
        private DetectInput _detectInput = new DetectInput(); //Placeholder, remove this after finishing the timed leader board stuff

        private readonly TimeManagerViewModel _timeManagerViewModel = new TimeManagerViewModel();
        private ICharacterSpawn _character = null;
        private UploadPlayerScoreDataSource _uploadPlayerScoreDataSource = null;

        private void CleanUp()
        {
            Debug.Log("Cleaning Up");
        }

        private void Reset()
        {
            _detectInput.Reset();
            _timeManager.Reset();
            _checkpointManager.Reset();
            _stateManager.SetState(State.Play);
            _timerView.Reset();
            _character.SpawnAt(_checkpointManager.CheckpointPosition, _checkpointManager.CheckpointRotation);
        }

        private void Awake()
        {
            _stateManager.OnStateChanged += HandleStateChange;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            if (FindObjectOfType<DebugWindow>() == null)
            {
                DebugWindow debugWindow = Instantiate(_debugCanvas).gameObject.GetComponent<DebugWindow>();
            }
#endif
            
            
            _character = _player;
            _pauseMenu.Initialize(_player);
            _postProcessingVolumeWeightTool = new PostProcessingVolumeWeightTool(_deathVolume, 0.1f, 0f, 0.1f);
            _uploadPlayerScoreDataSource = new UploadPlayerScoreDataSource();
        }

        private void Start()
        {
            _perigonAnalytics.StartSession(SystemInfo.deviceUniqueIdentifier);
            _player.Initialize();
            _checkpointManager.Initialize(_detectInput, _timeManagerViewModel);
            _timeManager.Initialize(_timeManagerViewModel);
            _timerView.Initialize(_timeManagerViewModel);
            _settingsViewBehaviour.Initialize(_player);
            SetupLeaderboardViews();
            
            _stateManager.SetState(State.PreGame);
        }

        private void SetupLeaderboardViews()
        {
            _leaderboardPanel.Initialize(LockMouseUtility.Instance);
            _forcedUploadView.Initialize();
        }

        private void Update()
        {
            if (Keyboard.current[PauseKey].wasPressedThisFrame)
            {
                if (_pauseMenu.gameObject.activeSelf)
                {
                    _pauseMenu.ClosePanel();
                }
                else
                {
                    _pauseMenu.OpenPanel();
                }
            }
        }

        private void OnApplicationQuit()
        {
            _perigonAnalytics.EndSession();
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
                    Time.timeScale = 1.0f;
                    break;
                }
                case State.Pause:
                {
                    Time.timeScale = 0.0f;
                    break;
                }
                case State.EndRace:
                {
                    HandleOnEndOfRace();
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

        private void HandleOnEndOfRace()
        {
            _timeManagerViewModel.StopTimer();
            var gameTime = _timeManagerViewModel.CurrentGameTimeMilliSeconds;
            var input = _detectInput.GetInput(); //Placeholder, remove this after finishing the timed leader board stuff
            _uploadPlayerScoreDataSource.UploadScoreIfPossible(gameTime, input);
            _leaderboardPanel.SetUserTime(gameTime, input);
            _leaderboardPanel.ShowPanel();
        }
    }
}
