using System.Linq;
using Perigon.Analytics;
using Perigon.Entities;
using Perigon.Leaderboard;
using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace BForBoss
{
    public class WeaponRaceCourseWorldManager: BaseWorldManager
    {
        [Title("Component")] 
        [SerializeField] private TimeManager _timeManager = null;
        [SerializeField] private CheckpointManager _checkpointManager = null;
        [Title("Weapon/Equipment Component")] 
        [SerializeField] private WeaponsManager _weaponsManager = null;
        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        [SerializeField] private AmmunitionCountViewBehaviour _ammunitionCountView = null;
        [SerializeField] private ReloadViewBehaviour _reloadView = null;
        
        [Title("User Interface")]
        [SerializeField] private TimerViewBehaviour _timerView = null;
        [SerializeField] private ForcedSetUsernameViewBehaviour _forcedUploadView = null;

        [Title("Effects")] 
        [SerializeField] private Volume _deathVolume = null;

        private readonly PerigonAnalytics _perigonAnalytics = PerigonAnalytics.Instance;
        private PostProcessingVolumeWeightTool _postProcessingVolumeWeightTool = null;
        private DetectInput _detectInput = new DetectInput(); //Placeholder, remove this after finishing the timed leader board stuff
        private readonly TimeManagerViewModel _timeManagerViewModel = new TimeManagerViewModel();
        private UploadPlayerScoreDataSource _uploadPlayerScoreDataSource = null;
        private LifeCycleBehaviour[] _lifeCycleBehaviours = null;
        
        protected override Vector3 SpawnLocation => _checkpointManager.CheckpointPosition;
        protected override Quaternion SpawnLookDirection => _checkpointManager.CheckpointRotation;

        protected override void Reset()
        {
            _checkpointManager.Reset();
            base.Reset();
            _detectInput.Reset();
            _timeManager.Reset();
            _timerView.Reset();
            _lifeCycleBehaviours.ForEach(dummy => dummy.Reset());
        }

        protected override void Awake()
        {
            base.Awake();
            _postProcessingVolumeWeightTool = new PostProcessingVolumeWeightTool(_deathVolume, 0.1f, 0f, 0.1f);
            _uploadPlayerScoreDataSource = new UploadPlayerScoreDataSource();
            _lifeCycleBehaviours = FindObjectsOfType<LifeCycleBehaviour>();
        }

        protected override void Start()
        {
            base.Start();
            _perigonAnalytics.StartSession(SystemInfo.deviceUniqueIdentifier);
            _checkpointManager.Initialize(_detectInput, _timeManagerViewModel);
            _timeManager.Initialize(_timeManagerViewModel);
            
            _weaponsManager.Initialize(new CharacterMovementWrapper(_player));
            _equipmentBehaviour.Initialize();
            _ammunitionCountView.Initialize(_equipmentBehaviour);
            _reloadView.Initialize(_equipmentBehaviour);
            
            _timerView.Initialize(_timeManagerViewModel);
            _forcedUploadView.Initialize(_freezeActionsUtility);
        }
        
        private void OnApplicationQuit()
        {
            _perigonAnalytics.EndSession();
        }

        protected override void HandleOnEndOfRace()
        {
            var penaltyTime = 5f;
            _timeManagerViewModel.StopTimer();
            var totalPenaltyTime = _lifeCycleBehaviours.Count(life => life.IsAlive) * penaltyTime * 1000;
            var gameTime = _timeManagerViewModel.CurrentGameTimeMilliSeconds + (int)totalPenaltyTime;
            var input = _detectInput.GetInput(); //Placeholder, remove this after finishing the timed leader board stuff
            _uploadPlayerScoreDataSource.UploadScoreIfPossible(gameTime, input);
            _pauseMenu.ForceOpenLeaderboardWithScore(gameTime, input);
        }

        protected override void HandleOnDeath()
        {
            _postProcessingVolumeWeightTool.InstantDistortAndRevert();
            base.HandleOnDeath();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_weaponsManager == null)
            {
                Debug.LogWarning("Weapons Manager missing from World Manager");
            }
            
            if (_equipmentBehaviour == null)
            {
                Debug.LogWarning("Equipment Behaviour missing from World Manager");
            }
            
            if (_ammunitionCountView == null)
            {
                Debug.LogWarning("Ammunition Count View missing from World Manager");
            }
            
            if (_reloadView == null)
            {
                Debug.LogWarning("Reload View missing from World Manager");
            }
        }
    }
}
