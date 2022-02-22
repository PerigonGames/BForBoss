using Perigon.Analytics;
using Perigon.Leaderboard;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace BForBoss
{
    public class RaceCourseWorldManager : BaseWorldManager
    {
        [Title("Component")] 
        [SerializeField] private TimeManager _timeManager = null;
        [SerializeField] private CheckpointManager _checkpointManager = null;

        [Title("User Interface")]
        [SerializeField] private TimerViewBehaviour _timerView = null;
        [SerializeField] private ForcedSetUsernameViewBehaviour _forcedUploadView = null;

        [Title("Effects")] 
        [SerializeField] private Volume _deathVolume = null;

        private readonly BForBossAnalytics _analytics = BForBossAnalytics.Instance;
        private PostProcessingVolumeWeightTool _postProcessingVolumeWeightTool = null;
        private DetectInput _detectInput = new DetectInput(); //Placeholder, remove this after finishing the timed leader board stuff
        private readonly TimeManagerViewModel _timeManagerViewModel = new TimeManagerViewModel();
        private UploadPlayerScoreDataSource _uploadPlayerScoreDataSource = null;

        protected override Vector3 SpawnLocation => _checkpointManager.CheckpointPosition;
        protected override Quaternion SpawnLookDirection => _checkpointManager.CheckpointRotation;

        protected override void Reset()
        {
            _checkpointManager.Reset();
            base.Reset();
            _detectInput.Reset();
            _timeManager.Reset();
            _timerView.Reset();
        }

        protected override void Awake()
        {
            base.Awake();
            _postProcessingVolumeWeightTool = new PostProcessingVolumeWeightTool(_deathVolume, 0.1f, 0f, 0.1f);
            _uploadPlayerScoreDataSource = new UploadPlayerScoreDataSource();
        }

        protected override void Start()
        {
            base.Start();
            _analytics.StartSession(SystemInfo.deviceUniqueIdentifier);
            _checkpointManager.Initialize(_detectInput, _timeManagerViewModel);
            _timeManager.Initialize(_timeManagerViewModel);
            _timerView.Initialize(_timeManagerViewModel);
            _forcedUploadView.Initialize(_freezeActionsUtility);
        }

        private void OnApplicationQuit()
        {
            _analytics.EndSession();
        }

        protected override void HandleOnEndOfRace()
        {
            _timeManagerViewModel.StopTimer();
            var gameTime = _timeManagerViewModel.CurrentGameTimeMilliSeconds;
            var input = _detectInput.GetInput(); //Placeholder, remove this after finishing the timed leader board stuff
            _uploadPlayerScoreDataSource.UploadScoreIfPossible(gameTime, input);
            _pauseMenu.ForceOpenLeaderboardWithScore(gameTime, input);
        }

        protected override void HandleOnDeath()
        {
            _postProcessingVolumeWeightTool.InstantDistortAndRevert();
            base.HandleOnDeath();
        }
    }
}
