using Perigon.Entities;
using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace BForBoss
{
    public class WeaponSandboxWorldManager : BaseWorldManager
    {
        private const float DEATH_POST_PROCESSING_DURATION = 0.1F;
        private const float DEATH_POST_PROCESSING_START = 0F;
        private const float DEATH_POST_PROCESSING_END = 0.1f;
        
        [SerializeField] 
        private Transform _spawnLocation = null;
        
        [Title("Component")]
        [SerializeField] private LifeCycleManager _lifeCycleManager = null;
        
        [Title("Weapon/Equipment Component")] 
        [SerializeField] private WeaponsManager _weaponsManager = null;
        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        [SerializeField] private AmmunitionCountViewBehaviour _ammunitionCountView = null;
        [SerializeField] private ReloadViewBehaviour _reloadView = null;
        
        [Title("Effects")] 
        [SerializeField] private Volume _deathVolume = null;
        
        private PostProcessingVolumeWeightTool _postProcessingVolumeWeightTool = null;

        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;
        
        protected override void Reset()
        {
            base.Reset();
            _lifeCycleManager.Reset();
        }

        protected override void Awake()
        {
            base.Awake();
            _postProcessingVolumeWeightTool = new PostProcessingVolumeWeightTool(_deathVolume, DEATH_POST_PROCESSING_DURATION, DEATH_POST_PROCESSING_START, DEATH_POST_PROCESSING_END);
        }

        protected override void Start()
        {
            base.Start();
            _weaponsManager.Initialize(new CharacterMovementWrapper(_player));
            _equipmentBehaviour.Initialize();
            _ammunitionCountView.Initialize(_equipmentBehaviour);
            _reloadView.Initialize(_equipmentBehaviour);
        }
        
        protected override void HandleOnDeath()
        {
            _postProcessingVolumeWeightTool.InstantDistortAndRevert();
            _lifeCycleManager.Reset();            
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

            if (_lifeCycleManager == null)
            {
                Debug.LogWarning("Life Cycle Manager missing from World Manager");
            }
        }
    }
}
