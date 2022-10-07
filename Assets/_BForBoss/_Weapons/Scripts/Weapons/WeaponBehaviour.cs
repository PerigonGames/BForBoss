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
        protected readonly Vector3 CenterOfCameraPosition = new Vector3(0.5f, 0.5f, 0);

        [SerializeField] protected Transform _firePoint = null;
        [SerializeField] private VisualEffect _muzzleFlash = null;
        [SerializeField] private LayerMask _rayCastBulletLayerMask;
        [InlineEditor]
        [SerializeField]
        private WeaponScriptableObject _weaponScriptableObject = null;

        protected Weapon _weapon = null;
        protected bool _isFiring = false;
        protected float _timeSinceFire = 0f;
        
        private Camera _mainCamera = null;
        private BulletSpawner _bulletSpawner;
        private WallHitVFXSpawner _wallHitVFXSpawner;
        private IWeaponAnimationProvider _weaponAnimationProvider;
        private ICrossHairProvider _crossHairProvider;
        private PGInputSystem _inputSystem;
        
        public Weapon WeaponViewModel => _weapon;

        protected Camera MainCamera
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
            ICrossHairProvider crossHairProvider,
            IWeaponProperties properties = null)
        {
            _inputSystem = inputSystem;
            _bulletSpawner = bulletSpawner;
            _wallHitVFXSpawner = wallHitVFXSpawner;
            _weaponAnimationProvider = weaponAnimationProvider;
            _crossHairProvider = crossHairProvider;
            _weapon = new Weapon(properties ?? _weaponScriptableObject);
            BindWeapon();
            SetCrosshairImage();
            SetupPlayerInput();
        }

        private void HandleOnWeaponActivate(bool activate)
        {
            enabled = activate;
            gameObject.SetActive(activate);
        }

        private void HandleOnStopReloading()
        {
            _weaponAnimationProvider.ReloadingWeapon(false);
        }

        private void BindWeapon()
        {
            _weapon.OnFireWeapon += HandleOnFire;
            _weapon.OnSetWeaponActivate += HandleOnWeaponActivate;
            _weapon.OnStopReloading += HandleOnStopReloading;
        }

        private void SetCrosshairImage()
        {
            if (_weapon != null)
            {
                _crossHairProvider.SetCrossHairImage(_weapon.Crosshair);
            }
        }

        protected abstract void OnFireInputAction(bool isFiring);
        protected abstract void Update();
        
        private void OnBulletHitWall(Vector3 point, Vector3 pointNormal)
        {
            var wallHitVFX = _wallHitVFXSpawner.SpawnWallHitVFX();
            wallHitVFX.transform.SetPositionAndRotation(point, Quaternion.LookRotation(pointNormal));
            wallHitVFX.transform.Translate(0f, 0f, WALL_HIT_ZFIGHT_BUFFER, Space.Self);
            wallHitVFX.Spawn();
        }

        private void HandleOnFire(int numberOfBullets)
        {
            FireBullets(numberOfBullets);

            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }
            
            _weaponAnimationProvider.WeaponFire(_weapon.AnimationType);
            FMODUnity.RuntimeManager.PlayOneShot(_weapon.ShotAudio, transform.position);
        }

        private void FireBullets(int numberOfBullets)
        {
            if (_weapon.IsRayCastingWeapon)
            {
                FireRayCastBullets(numberOfBullets);
            }
            else
            {
                FireProjectiles(numberOfBullets);
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

        private void Awake()
        {
            if (_muzzleFlash == null)
            {
                Debug.LogWarning("Missing VFX Visual Effect from this weapon");
            }
        }

        private void OnEnable()
        {
            SetCrosshairImage();
        }

        private void OnDestroy()
        {
            if (_weapon != null)
            {
                _weapon.OnFireWeapon -= HandleOnFire;
                _weapon.OnSetWeaponActivate -= HandleOnWeaponActivate;
                _weapon.OnStopReloading -= HandleOnStopReloading;
                _weapon = null;
            }
        }
    }
}
