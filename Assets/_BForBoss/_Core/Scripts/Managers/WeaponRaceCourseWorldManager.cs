using System;
using Perigon.Analytics;
using Perigon.Leaderboard;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace BForBoss
{
    public class WeaponRaceCourseWorldManager: BaseWorldManager
    {
        private const float RACE_COURSE_PENALTY_TIME = 5F;
        private const float MAP_SECONDS_TO_MILLISECONDS = 1000f;
        
        [Title("Component")] 
        [SerializeField] private TimeManager _timeManager = null;
        [SerializeField] private CheckpointManager _checkpointManager = null;
        [SerializeField] private LifeCycleManager _lifeCycleManager = null;

        [Title("User Interface")]
        [SerializeField] private TimerViewBehaviour _timerView = null;
        [SerializeField] private EntityCounterViewBehaviour _entityCounterView = null;

        private readonly BForBossAnalytics _analytics = BForBossAnalytics.Instance;
        private readonly TimeManagerViewModel _timeManagerViewModel = new TimeManagerViewModel();
        private UploadPlayerScoreDataSource _uploadPlayerScoreDataSource = null;

        protected override Vector3 SpawnLocation => _checkpointManager.CheckpointPosition;
        protected override Quaternion SpawnLookDirection => _checkpointManager.CheckpointRotation;

        protected override void Reset()
        {
            _checkpointManager.Reset();
            base.Reset();
            _timeManager.Reset();
            _timerView.Reset();
            _lifeCycleManager.Reset();
            _entityCounterView.Reset();
        }

        protected override void Awake()
        {
            base.Awake();
            _uploadPlayerScoreDataSource = new UploadPlayerScoreDataSource();
        }

        protected override void Start()
        {
            base.Start();
            _analytics.StartSession(SystemInfo.deviceUniqueIdentifier);
            _checkpointManager.Initialize(_timeManagerViewModel);
            _timeManager.Initialize(_timeManagerViewModel);

            _timerView.Initialize(_timeManagerViewModel);
            _entityCounterView.Initialize(_lifeCycleManager);
        }
        
        private void OnApplicationQuit()
        {
            _analytics.EndSession();
        }

        protected override void HandleOnEndOfRace()
        {
            _timeManagerViewModel.StopTimer();
            var totalPenaltyTime = _lifeCycleManager.LivingEntities * RACE_COURSE_PENALTY_TIME * MAP_SECONDS_TO_MILLISECONDS;
            var gameTime = _timeManagerViewModel.CurrentGameTimeMilliSeconds + (int)totalPenaltyTime;
            _uploadPlayerScoreDataSource.UploadScoreIfPossible(gameTime);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_lifeCycleManager == null)
            {
                PanicHelper.Panic(new Exception("Life Cycle Manager missing from World Manager"));
            }
        }
    }
}
