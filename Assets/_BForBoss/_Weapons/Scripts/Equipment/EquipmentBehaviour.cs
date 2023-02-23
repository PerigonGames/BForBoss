using System;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IGetPlayerTransform
    {
        Transform Value { get; }
    }
    
    [RequireComponent(typeof(BulletSpawner))]
    [RequireComponent(typeof(WallHitVFXSpawner))]
    public partial class EquipmentBehaviour : MonoBehaviour
    {
        [SerializeField] private WeaponBehaviour[] _weaponBehaviours = null;
        private BulletSpawner _bulletSpawner;
        private WallHitVFXSpawner _wallHitVFXSpawner;
        private MeleeWeaponBehaviour _meleeBehaviour = null;
        private IWeaponAnimationProvider _weaponAnimationProvider;
        private PGInputSystem _inputSystem;
        private Weapon[] _weapons = null;

        private int _currentWeaponIndex = 0;
        
        public void Initialize(IGetPlayerTransform getPlayerTransform, PGInputSystem inputSystem, IWeaponAnimationProvider weaponAnimationProvider, ICrossHairProvider crossHairProvider)
        {
            _inputSystem = inputSystem;
            _weaponAnimationProvider = weaponAnimationProvider;
            _meleeBehaviour.Initialize(getPlayerTransform, onSuccessfulAttack: () => _weaponAnimationProvider.MeleeAttack(CurrentWeapon.AnimationType));
            SetupWeapons(crossHairProvider);
            SetupInputBinding();
        }

        public void ScrollSwapWeapons(int direction)
        {
            foreach (var weapon in _weapons)
            {
                weapon.ActivateWeapon = false;
            }

            _weapons[_currentWeaponIndex].ActivateWeapon = true;
        }

        public void Reset()
        {
            foreach (var weapon in _weapons)
            {
                weapon.Reset();
            }
            
            foreach (var weaponBehaviour in _weaponBehaviours)
            {
                weaponBehaviour.Reset();
            }
            _currentWeaponIndex = 0;
            _weapons[_currentWeaponIndex].ActivateWeapon = true;
        }
        
        private void SetupWeapons(ICrossHairProvider crossHairProvider)
        {
            _weapons = new Weapon[_weaponBehaviours.Length];
            for(int i = 0; i < _weaponBehaviours.Length; i++)
            {
                _weaponBehaviours[i].Initialize(_inputSystem, _bulletSpawner, _wallHitVFXSpawner, _weaponAnimationProvider, crossHairProvider);
                _weapons[i] = _weaponBehaviours[i].WeaponViewModel;
                _weapons[i].ActivateWeapon = false;
            }

            _weapons[_currentWeaponIndex].ActivateWeapon = true;
        }

        private void SetupInputBinding()
        {
            _inputSystem.OnScrollWeaponChangeAction += OnScrollWeaponSwapAction;
            _inputSystem.OnDirectWeaponChangeAction += OnDirectWeaponSwapAction;
            _inputSystem.OnMeleeAction += OnMeleeAction;
        }
        
        private void UpdateCurrentWeaponIndex(bool isUpwards)
        {
            var indexLength = _weaponBehaviours.Length - 1;
            _currentWeaponIndex += isUpwards ? 1 : -1;
            
            if (_currentWeaponIndex > indexLength)
            {
                _currentWeaponIndex = 0;
            } 
            else if (_currentWeaponIndex < 0)
            {
                _currentWeaponIndex = indexLength;
            }
        }

        private void Awake()
        {
            _bulletSpawner = GetComponent<BulletSpawner>();
            _wallHitVFXSpawner = GetComponent<WallHitVFXSpawner>();
            _meleeBehaviour = GetComponent<MeleeWeaponBehaviour>();
            if (_weaponBehaviours.IsNullOrEmpty())
            {
                PanicHelper.Panic(new Exception("There are currently no WeaponBehaviour within the child of EquipmentBehaviour"));
            }
        }
        #region Input

        private void OnMeleeAction()
        {
            _meleeBehaviour.Melee();
        }

        private void OnScrollWeaponSwapAction(bool direction)
        {
            UpdateCurrentWeaponIndex(direction);
            _weaponAnimationProvider.SwapWeapon();
        }

        private void OnDirectWeaponSwapAction(int numberKey)
        {
            _currentWeaponIndex = numberKey - 1;
            _weaponAnimationProvider.SwapWeapon();
        }

        #endregion
    }
}