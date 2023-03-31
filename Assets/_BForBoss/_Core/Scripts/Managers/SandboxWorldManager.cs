using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class SandboxWorldManager : BaseWorldManager
    {
        
        [SerializeField] private RingLaborManager _ringLaborManager;
        [SerializeField] private BossWipeOutWallBehaviour _wipeOutWallBehaviour;
        
        
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
            _wipeOutWallBehaviour.Initialize(_playerBehaviour.PlayerMovement);
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
