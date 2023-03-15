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
    public class EquipmentBehaviour : MonoBehaviour
    {
        [SerializeField] private WeaponBehaviour[] _weaponBehaviours = null;
        private BulletSpawner _bulletSpawner;
        private WallHitVFXSpawner _wallHitVFXSpawner;
        private MeleeWeaponBehaviour _meleeBehaviour = null;
        private IWeaponAnimationProvider _weaponAnimationProvider;
        private PGInputSystem _inputSystem;

        private int _currentWeaponIndex = 0;
        private WeaponBehaviour CurrentWeapon => _weaponBehaviours[_currentWeaponIndex];
        
        public void Initialize(
            IGetPlayerTransform getPlayerTransform, 
            PGInputSystem inputSystem, 
            IWeaponAnimationProvider weaponAnimationProvider, 
            CrossHairBehaviour crossHairProvider,
            IShootingCases shootingCases)
        {
            _inputSystem = inputSystem;
            _weaponAnimationProvider = weaponAnimationProvider;
            _meleeBehaviour.Initialize(getPlayerTransform, onSuccessfulAttack: () => _weaponAnimationProvider.MeleeAttack(CurrentWeapon.AnimationType));
            SetupWeapons(crossHairProvider, shootingCases);
            SetupInputBinding();
        }

        public void ScrollSwapWeapons(int direction)
        {
            if (_weaponBehaviours.Length > 1)
            {
                foreach (var weapon in _weaponBehaviours)
                {
                    weapon.Activate(false);
                }

                _weaponBehaviours[_currentWeaponIndex].Activate(true);
            }
        }

        public void Reset()
        {
            foreach (var weaponBehaviour in _weaponBehaviours)
            {
                weaponBehaviour.Reset();
            }
            _currentWeaponIndex = 0;
            _weaponBehaviours[_currentWeaponIndex].Activate(true);
        }
        
        private void SetupWeapons(CrossHairBehaviour crossHairProvider, IShootingCases shootingCases)
        {
            for(int i = 0; i < _weaponBehaviours.Length; i++)
            {
                _weaponBehaviours[i].Initialize(_inputSystem, _bulletSpawner, _wallHitVFXSpawner, _weaponAnimationProvider, crossHairProvider, shootingCases);
                _weaponBehaviours[i].Activate(false);
            }

            _weaponBehaviours[_currentWeaponIndex].Activate(true);
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
            if (_weaponBehaviours.Length > 1)
            {
                UpdateCurrentWeaponIndex(direction);
                _weaponAnimationProvider.SwapWeapon();
            }
        }

        private void OnDirectWeaponSwapAction(int numberKey)
        {
            if (_weaponBehaviours.Length > 1)
            {
                _currentWeaponIndex = numberKey - 1;
                _weaponAnimationProvider.SwapWeapon();
            }
        }

        #endregion
    }
}