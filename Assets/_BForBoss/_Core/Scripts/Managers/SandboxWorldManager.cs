using System;
using BForBoss.RingSystem;
using Perigon.Utility;
using Sirenix.Utilities;
using UnityEngine;

namespace BForBoss
{
    public class SandboxWorldManager : BaseWorldManager
    {

        [SerializeField] private RingLaborManager _ringLaborManager;
        [SerializeField] private BossWipeOutWallsManager _wipeOutWallsManager;

        private DerekMissileLauncherBehaviour[] _derekMissileLauncherBehaviours;
        private CountdownViewBehaviour _countdownTimer;

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
            _wipeOutWallsManager.Initialize(_playerBehaviour.PlayerMovement);
            _derekMissileLauncherBehaviours.ForEach(launcher => launcher.Initialize(_playerBehaviour.PlayerMovement));
        }

        protected override void Awake()
        {
            base.Awake();
            _derekMissileLauncherBehaviours = FindObjectsOfType<DerekMissileLauncherBehaviour>();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_ringLaborManager == null)
            {
                PanicHelper.Panic(new Exception("_ringLaborManager is missing from Sandbox World Manager"));
            }
        }
    }
}