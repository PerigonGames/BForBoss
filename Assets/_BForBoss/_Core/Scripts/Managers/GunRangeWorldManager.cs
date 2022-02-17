using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class GunRangeWorldManager : BaseWorldManager
    {
        [Title("Component")] 
        [SerializeField] private WeaponsManager _weaponsManager = null;
        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        [SerializeField] private AmmunitionCountViewBehaviour _ammunitionCountView = null;
        [SerializeField] private ReloadViewBehaviour _reloadView = null;

        protected override Vector3 SpawnLocation => Vector3.zero;
        protected override Quaternion SpawnLookDirection => Quaternion.identity;
        
        protected override void Start()
        {
            base.Start();
            _weaponsManager.Initialize(new CharacterMovementWrapper(_player));
            _equipmentBehaviour.Initialize();
            _ammunitionCountView.Initialize(_equipmentBehaviour);
            _reloadView.Initialize(_equipmentBehaviour);
        }
        
        protected override void SetupAnalytics()
        {
            // Not needed
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
