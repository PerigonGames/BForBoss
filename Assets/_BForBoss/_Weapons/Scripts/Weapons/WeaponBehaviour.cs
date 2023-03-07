using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Perigon.Weapons
{
    public abstract partial class WeaponBehaviour : MonoBehaviour
    {
        private const float WALL_HIT_ZFIGHT_BUFFER = 0.01f;
        private const float RAYCAST_DISTANCE_LIMIT = 50f;
        private readonly Vector3 CenterOfCameraPosition = new Vector3(0.5f, 0.5f, 0);

        [SerializeField] protected Transform _firePoint = null;
        [SerializeField] private VisualEffect _muzzleFlash = null;
        [SerializeField] private LayerMask _rayCastBulletLayerMask;
        [InlineEditor]
        [SerializeField]
        private WeaponConfigurationSO _weaponConfigurationSo = null;
        
        protected Weapon _weapon = null;
        protected WeaponConfigurationData _weaponConfigurationData;
        
        protected float _timeSinceFire = 0f;
        
        private WeaponData _weaponData = default;
        private Camera _mainCamera = null;
        private BulletSpawner _bulletSpawner;
        private WallHitVFXSpawner _wallHitVFXSpawner;
        private IWeaponAnimationProvider _weaponAnimationProvider;
        private ICrossHairProvider _crossHairProvider;
        private PGInputSystem _inputSystem;

        public WeaponAnimationType AnimationType => _weaponConfigurationData.AnimationType;
        
        private Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }

                return _mainCamera;
            }
        }

        public void Initialize(
            PGInputSystem inputSystem,
            BulletSpawner bulletSpawner,
            WallHitVFXSpawner wallHitVFXSpawner,
            IWeaponAnimationProvider weaponAnimationProvider,
            ICrossHairProvider crossHairProvider)
        {
            _inputSystem = inputSystem;
            _bulletSpawner = bulletSpawner;
            _wallHitVFXSpawner = wallHitVFXSpawner;
            _weaponAnimationProvider = weaponAnimationProvider;
            _crossHairProvider = crossHairProvider;
            _weaponConfigurationData = _weaponConfigurationSo.MapToData();
            _weapon = new Weapon(
                rateOfFire: _weaponConfigurationData.RateOfFire,
                bulletSpread: _weaponConfigurationData.BulletSpread);
            BindWeapon();
            SetCrossHairImage();
            SetupPlayerInput();
        }

        public virtual void Reset()
        {
            _timeSinceFire = 0;
            enabled = false;
            gameObject.SetActive(false);
        }
        
        public void Activate(bool activate)
        {
            enabled = activate;
            gameObject.SetActive(activate);
            _weaponAnimationProvider.ReloadingWeapon(false);        
        }
        
        protected abstract void OnFireInputAction(bool isFiring);

        protected virtual void Update()
        {
            _weapon.DecrementElapsedTimeRateOfFire(Time.deltaTime, Time.timeScale);
        }

        protected virtual void PlayFiringAudio()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_weaponConfigurationData.WeaponShotAudio, transform.position);
        }

        protected virtual void Awake()
        {
            if (_muzzleFlash == null)
            {
                Debug.LogWarning("Missing VFX Visual Effect from this weapon");
            }
        }
        
        private void BindWeapon()
        {
            _weapon.OnWeaponEffectEmit += HandleEffect;
            _weapon.OnWeaponDataStateChange += HandleStateChange;
        }

        private void HandleStateChange(WeaponData data)
        {
            _weaponData = data;
        }

        private void HandleEffect(WeaponEffect effect)
        {
            switch (effect)
            {
                case WeaponEffect.FireWeapon:
                    FireWeapon();
                    break;
            }
        }

        private void SetCrossHairImage()
        {
            _crossHairProvider.SetCrossHairImage(_weaponConfigurationData.Crosshair);
        }
        
        private void OnBulletHitWall(Vector3 point, Vector3 pointNormal)
        {
            var wallHitVFX = _wallHitVFXSpawner.SpawnWallHitVFX();
            wallHitVFX.transform.SetPositionAndRotation(point, Quaternion.LookRotation(pointNormal));
            wallHitVFX.transform.Translate(0f, 0f, WALL_HIT_ZFIGHT_BUFFER, Space.Self);
            wallHitVFX.Spawn();
        }

        private void FireWeapon()
        {
            // Shooting
            FireBullets();

            //VFX
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }
            
            // Animation
            _weaponAnimationProvider.WeaponFire(_weaponConfigurationData.AnimationType);
            
            //Audio
            PlayFiringAudio();
        }

        private void FireBullets()
        {
            if (_weaponConfigurationData.IsRayCastingWeapon)
            {
                FireRayCastBullets();
            }
            else
            {
                FireProjectiles();
            }
        }

        private void SetupPlayerInput()
        {
            _inputSystem.OnFireAction += OnFireInputAction;
        }

        private void OnEnable()
        {
            if (_crossHairProvider != null)
            {
                SetCrossHairImage();
            }
        }

        private void OnDestroy()
        {
            if (_weapon != null)
            {
                _weapon.OnWeaponEffectEmit -= HandleEffect;
                _weapon.OnWeaponDataStateChange -= HandleStateChange;
                _weapon = null;
            }
        }
    }
}
