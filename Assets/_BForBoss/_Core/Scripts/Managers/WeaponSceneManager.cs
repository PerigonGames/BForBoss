using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WeaponSceneManager : MonoBehaviour
    {
        [Title("Weapon/Equipment Component")] [SerializeField]
        private WeaponAnimationController weaponAnimationController = null;

        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        [SerializeField] private AmmunitionCountViewBehaviour _ammunitionCountView = null;
        [SerializeField] private ReloadViewBehaviour _reloadView = null;

        public void Initialize(PlayerBehaviour playerBehaviour)
        {
            weaponAnimationController.Initialize(
                () => playerBehaviour.PlayerMovement.CharacterVelocity,
                () => playerBehaviour.PlayerMovement.CharacterMaxSpeed,
                () => playerBehaviour.PlayerMovement.IsWallRunning,
                () => playerBehaviour.PlayerMovement.IsGrounded,
                () => playerBehaviour.PlayerMovement.IsSliding,
                () => playerBehaviour.PlayerMovement.IsDashing);
            _equipmentBehaviour.Initialize(playerBehaviour.PlayerMovement.RootPivot);
            _ammunitionCountView.Initialize(_equipmentBehaviour);
            _reloadView.Initialize(_equipmentBehaviour);
        }

        private void OnValidate()
        {
            if (weaponAnimationController == null)
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
