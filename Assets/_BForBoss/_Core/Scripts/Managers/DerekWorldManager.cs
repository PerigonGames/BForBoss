using BForBoss.RingSystem;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class DerekWorldManager : BaseWorldManager
    {
        [SerializeField] private RingLaborManager _ringLaborManager;
        [SerializeField] private DerekContextManager _derekContextManager;
        private CountdownViewBehaviour _countdownTimer;

        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;

        protected override void Reset()
        {
            base.Reset();
            _ringLaborManager.Reset();
            _derekContextManager.Reset();
        }

        protected override void Start()
        {
            base.Start();
            _ringLaborManager.Initialize();
            _derekContextManager.Initialize(_ringLaborManager, _playerBehaviour.PlayerMovement);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.PanicIfNullObject(_ringLaborManager, nameof(_ringLaborManager));
            this.PanicIfNullObject(_derekContextManager, nameof(_derekContextManager));
        }
    }
}
