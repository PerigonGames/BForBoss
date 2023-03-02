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
        private WeaponSO _weaponSo = null;

        protected Weapon _weapon = null;
        protected WeaponData _weaponData;
        
        protected float _timeSinceFire = 0f;
        
        private Camera _mainCamera = null;
        private BulletSpawner _bulletSpawner;
        private WallHitVFXSpawner _wallHitVFXSpawner;
        private IWeaponAnimationProvider _weaponAnimationProvider;
        private ICrossHairProvider _crossHairProvider;
        private PGInputSystem _inputSystem;

        public WeaponAnimationType AnimationType => _weaponData.AnimationType;
        
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
            _weapon = new Weapon(
                ammunitionAmount: _weaponData.AmmunitionAmount,
                rateOfFire: _weaponData.RateOfFire,
                reloadDuration: _weaponData.ReloadDuration,
                bulletSpread: _weaponData.BulletSpread);
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
        
        protected abstract void OnFireInputAction(bool isFiring);

        protected virtual void Update()
        {
            _weapon.DecrementElapsedTimeRateOfFire(Time.deltaTime, Time.timeScale);
            _weapon.ReloadWeaponCountDownIfNeeded(Time.deltaTime, Time.timeScale);
        }

        protected virtual void PlayFiringAudio()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_weaponData.WeaponShotAudio, transform.position);
        }

        protected virtual void HandleOnStartReloading()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_weaponData.WeaponReloadAudio, transform.position);
        }

        private void HandleReloadingState(bool isReloading)
        {
            if (isReloading)
            {
                HandleOnStartReloading();
            }
            else
            {
                _weaponAnimationProvider.ReloadingWeapon(false);
            }
        }
        
        protected virtual void Awake()
        {
            if (_muzzleFlash == null)
            {
                Debug.LogWarning("Missing VFX Visual Effect from this weapon");
            }
        }

        public void Activate(bool activate)
        {
            gameObject.SetActive(activate);
            _weaponAnimationProvider.ReloadingWeapon(false);        
        }

        private void BindWeapon()
        {
            _weapon.OnFireWeapon += HandleOnFire;
            _weapon.OnReloadingStateUpdate += HandleReloadingState;
        }

        private void SetCrossHairImage()
        {
            if (_weapon != null)
            {
                _crossHairProvider.SetCrossHairImage(_weaponData.Crosshair);
            }
        }
        
        private void OnBulletHitWall(Vector3 point, Vector3 pointNormal)
        {
            var wallHitVFX = _wallHitVFXSpawner.SpawnWallHitVFX();
            wallHitVFX.transform.SetPositionAndRotation(point, Quaternion.LookRotation(pointNormal));
            wallHitVFX.transform.Translate(0f, 0f, WALL_HIT_ZFIGHT_BUFFER, Space.Self);
            wallHitVFX.Spawn();
        }

        private void HandleOnFire()
        {
            // Shooting
            FireBullets();

            //VFX
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }
            
            // Animation
            _weaponAnimationProvider.WeaponFire(_weaponData.AnimationType);
            
            //Audio
            PlayFiringAudio();
        }

        private void FireBullets()
        {
            if (_weaponData.IsRayCastingWeapon)
            {
                FireRayCastBullets();
            }
            else
            {
                FireProjectiles();
            }
        }

        private void OnReloadInputAction()
        {
            _weapon.ReloadWeaponIfPossible();
            _weaponAnimationProvider.ReloadingWeapon(_weapon.IsReloading);
        }
        
        private void SetupPlayerInput()
        {
            _inputSystem.OnFireAction += OnFireInputAction;
            _inputSystem.OnReloadAction += OnReloadInputAction;
        }

        private void Start()
        {
            _weaponData = _weaponSo.MapToData();
        }

        private void OnEnable()
        {
            SetCrossHairImage();
        }

        private void OnDestroy()
        {
            if (_weapon != null)
            {
                _weapon.OnFireWeapon -= HandleOnFire;
                _weapon.OnReloadingStateUpdate -= HandleReloadingState;
                _weapon = null;
            }
        }
    }
}
