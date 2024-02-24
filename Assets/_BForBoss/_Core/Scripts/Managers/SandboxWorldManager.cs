using BForBoss.RingSystem;
using Perigon.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace BForBoss
{
    public class SandboxWorldManager : BaseWorldManager
    {
        [SerializeField] private RingLaborManager _ringLaborManager;
        [SerializeField] private BossWipeOutWallsManager _wipeOutWallsManager;
        [SerializeField] private RingGrouping _ringGroupings;
        [SerializeField] private SimonSaysSystem _simonSaysSystem;
        
        private DerekMissileLauncherBehaviour[] _derekMissileLauncherBehaviours;
        private CountdownViewBehaviour _countdownTimerView;

        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;

        protected override void Reset()
        {
            base.Reset();
            _ringLaborManager.Reset();
        }

        protected override void Start()
        {
            base.Start();
            _ringLaborManager.Initialize();
            _ringLaborManager.SetRings(_ringGroupings);
            _wipeOutWallsManager.Initialize(_playerBehaviour.PlayerMovement);
            _derekMissileLauncherBehaviours.ForEach(launcher => launcher.Initialize(_playerBehaviour.PlayerMovement));
            _simonSaysSystem.Initialize();
            _stateManager.SetState(State.PreGame);
        }

        protected override void Awake()
        {
            base.Awake();
            _derekMissileLauncherBehaviours = FindObjectsOfType<DerekMissileLauncherBehaviour>();
            this.PanicIfNullObject(_ringGroupings, nameof(_ringGroupings));
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.PanicIfNullObject(_ringLaborManager, nameof(_ringLaborManager));
        }

        [Button]
        public void SetGameOver()
        {
            StateManager.Instance.SetState(State.EndGame);
        }

        public void RandomizeRingGroupToActivate()
        {
            _ringLaborManager.Reset();
            _ringLaborManager.SetRings(_ringGroupings);
            _ringLaborManager.ActivateSystem();
        }
    }
}