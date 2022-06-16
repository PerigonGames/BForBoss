using System;
using Perigon.Entities;
using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace BForBoss
{
    public class GunRangeWorldManager : BaseWorldManager
    {
        [FormerlySerializedAs("_weaponsManager")]
        [Title("Component")] 
        [SerializeField] private WeaponAnimationController weaponAnimationController = null;
        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        [SerializeField] private AmmunitionCountViewBehaviour _ammunitionCountView = null;
        [SerializeField] private ReloadViewBehaviour _reloadView = null;
        [SerializeField] private MeleeViewBehaviour _meleeView = null;
        [SerializeField] private LifeCycleManager _lifeCycleManager = null;

        protected override Vector3 SpawnLocation => Vector3.zero;
        protected override Quaternion SpawnLookDirection => Quaternion.identity;

        protected override void Start()
        {
            base.Start();
            weaponAnimationController.Initialize(
                () => _player.CharacterVelocity,
                () => _player.CharacterMaxSpeed,
                () => _player.IsWallRunning,
                () => _player.IsGrounded,
                () => _player.IsSliding,
                () => _player.IsDashing);
            _equipmentBehaviour.Initialize();
            _ammunitionCountView.Initialize(_equipmentBehaviour);
            _reloadView.Initialize(_equipmentBehaviour);
            _meleeView.Initialize(_equipmentBehaviour);
        }

        protected override void Reset()
        {
            base.Reset();
            _lifeCycleManager.Reset();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (weaponAnimationController == null)
            {
                PanicHelper.Panic(new Exception("Weapons Manager missing from World Manager"));
            }
            
            if (_equipmentBehaviour == null)
            {
                PanicHelper.Panic(new Exception("Equipment Behaviour missing from World Manager"));
            }
            
            if (_ammunitionCountView == null)
            {
                PanicHelper.Panic(new Exception("Ammunition Count View missing from World Manager"));
            }
            
            if (_reloadView == null)
            {
                PanicHelper.Panic(new Exception("Reload View missing from World Manager"));
            }
            
            if (_meleeView == null)
            {
                PanicHelper.Panic(new Exception("Melee View missing from World Manager"));
            }
            
            if (_lifeCycleManager == null)
            {
                PanicHelper.Panic(new Exception("Life Cycle Manager missing from World Manager"));
            }
        }
    }
}
