using System;
using Perigon.Analytics;
using Perigon.Entities;
using Perigon.Leaderboard;
using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace BForBoss
{
    public class WeaponRaceCourseWorldManager: BaseWorldManager
    {
        private const float RACE_COURSE_PENALTY_TIME = 5F;
        private const float MAP_SECONDS_TO_MILLISECONDS = 1000f;
        private const float DEATH_POST_PROCESSING_DURATION = 0.1F;
        private const float DEATH_POST_PROCESSING_START = 0F;
        private const float DEATH_POST_PROCESSING_END = 0.1f;
        
        [Title("Component")] 
        [SerializeField] private TimeManager _timeManager = null;
        [SerializeField] private CheckpointManager _checkpointManager = null;
        [SerializeField] private LifeCycleManager _lifeCycleManager = null;
        [Title("Weapon/Equipment Component")] 
        [SerializeField] private WeaponAnimationController _weaponAnimationController = null;
        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        [SerializeField] private AmmunitionCountViewBehaviour _ammunitionCountView = null;
        [SerializeField] private ReloadViewBehaviour _reloadView = null;
        
        [Title("User Interface")]
        [SerializeField] private TimerViewBehaviour _timerView = null;
        [SerializeField] private ForcedSetUsernameViewBehaviour _forcedUploadView = null;
        [SerializeField] private EntityCounterViewBehaviour _entityCounterView = null;

        [Title("Effects")] 
        [SerializeField] private Volume _deathVolume = null;

        private readonly BForBossAnalytics _analytics = BForBossAnalytics.Instance;
        private PostProcessingVolumeWeightTool _postProcessingVolumeWeightTool = null;
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
            _postProcessingVolumeWeightTool = new PostProcessingVolumeWeightTool(_deathVolume, DEATH_POST_PROCESSING_DURATION, DEATH_POST_PROCESSING_START, DEATH_POST_PROCESSING_END);
            _uploadPlayerScoreDataSource = new UploadPlayerScoreDataSource();
        }

        protected override void Start()
        {
            base.Start();
            _analytics.StartSession(SystemInfo.deviceUniqueIdentifier);
            _checkpointManager.Initialize(_timeManagerViewModel);
            _timeManager.Initialize(_timeManagerViewModel);

            _weaponAnimationController.Initialize(
                () => _playerBehaviour.PlayerMovement.CharacterVelocity,
                () => _playerBehaviour.PlayerMovement.CharacterMaxSpeed,
                () => _playerBehaviour.PlayerMovement.IsWallRunning,
                () => _playerBehaviour.PlayerMovement.IsGrounded,
                () => _playerBehaviour.PlayerMovement.IsSliding,
                () => _playerBehaviour.PlayerMovement.IsDashing);
            _equipmentBehaviour.Initialize(_playerBehaviour.PlayerMovement.RootPivot);
            _ammunitionCountView.Initialize(_equipmentBehaviour);
            _reloadView.Initialize(_equipmentBehaviour);
            
            _timerView.Initialize(_timeManagerViewModel);
            _entityCounterView.Initialize(_lifeCycleManager);
            _forcedUploadView.Initialize(_freezeActionsUtility);
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
            _pauseMenu.ForceOpenLeaderboardWithScore(gameTime);
        }

        protected override void HandleOnDeath()
        {
            _postProcessingVolumeWeightTool.InstantDistortAndRevert();
            base.HandleOnDeath();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_weaponAnimationController == null)
            {
                PanicHelper.Panic(new Exception("Weapons animation controller missing from World Manager"));
            }
            
            if (_equipmentBehaviour == null)
            {
                PanicHelper.Panic(new Exception("Equipment Behaviour missing from World Manager"));
            }
            
            if (_ammunitionCountView == null)
            {
                PanicHelper.Panic(new Exception("Ammunition Count View missing from World Manager"));
            }
            
            if (_reloadView == null)
            {
                PanicHelper.Panic(new Exception("Reload View missing from World Manager"));
            }

            if (_lifeCycleManager == null)
            {
                PanicHelper.Panic(new Exception("Life Cycle Manager missing from World Manager"));
            }
        }
    }
}
