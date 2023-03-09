using UnityEngine;
using UnityEngine.SceneManagement;

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

        protected override void OnAdditiveSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            switch (scene.name)
            {
                case ADDITIVE_WEAPON_SCENE_NAME:
                    WeaponSceneManager.EnergySystem = _energySystemBehaviour;
                    break;
                case ADDITIVE_HUD_SCENE_NAME:
                    HUDManager.EnergyDataSubject = _energySystemBehaviour;
                    break;
            }
            base.OnAdditiveSceneLoaded(scene, loadSceneMode);
        }
    }
}
