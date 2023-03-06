using UnityEngine;

namespace BForBoss
{
    public class SandboxWorldManager : BaseWorldManager
    {
        [SerializeField] private EnergySystemBehaviour _energySystemBehaviour;

        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;

        protected override void Start()
        {
            _playerBehaviour.EnergySystem = _energySystemBehaviour;
            base.Start();
        }

        protected override void Reset()
        {
            _energySystemBehaviour.Reset();
            base.Reset();
        }
    }
}
