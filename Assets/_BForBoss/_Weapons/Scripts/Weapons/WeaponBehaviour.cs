using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

namespace Perigon.Weapons
{
    [RequireComponent(typeof(BulletSpawner))]
    public abstract partial class WeaponBehaviour : MonoBehaviour
    {
        private const float RAYCAST_DISTANCE_LIMIT = 50f;
        protected readonly Vector3 CenterOfCameraPosition = new Vector3(0.5f, 0.5f, 0);
        
        [SerializeField] protected Transform _firePoint = null;
        [SerializeField] protected CrosshairBehaviour _crosshair = null;
        [SerializeField] private VisualEffect _muzzleFlash = null;
        [InlineEditor] 
        [SerializeField]
        private WeaponScriptableObject _weaponScriptableObject = null;
        
        protected Weapon _weapon = null;
        protected bool _isFiring = false;
        protected float _timeSinceFire = 0f;
        
        private InputAction _fireInputAction = null;
        private InputAction _reloadInputAction = null;

        private Camera _mainCamera = null;
        protected BulletSpawner _bulletSpawner;

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
            InputAction fireInputAction,
            InputAction reloadInputAction,
            IWeaponProperties properties = null)
        {
            _fireInputAction = fireInputAction;
            _reloadInputAction = reloadInputAction;
            _weapon = new Weapon(properties ?? _weaponScriptableObject);
            BindWeapon();
            SetCrosshairImage();
        }

        private void HandleOnWeaponActivate(bool activate)
        {
            enabled = activate;
        }

        private void BindWeapon()
        {
            _weapon.OnFireWeapon += HandleOnFire;
            _weapon.OnSetWeaponActivate += HandleOnWeaponActivate;
        }

        private void SetCrosshairImage()
        {
            if (_weapon != null)
            {
                _crosshair.SetCrosshairImage(_weapon.Crosshair);
            }
        }
        
        protected abstract void OnFireInputAction(InputAction.CallbackContext context);
        protected abstract void Update();

        private void HandleOnFire(int numberOfBullets)
        {
            FireBullets(numberOfBullets);
            
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }
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

        private void OnReloadInputAction(InputAction.CallbackContext context)
        {
            _weapon.ReloadWeaponIfPossible();
        }


        
        protected Vector3 GetDirectionOfShot()
        {
            var camRay = MainCamera.ViewportPointToRay(CenterOfCameraPosition);
            Vector3 targetPoint;
            if (Physics.Raycast(camRay, out var hit, Mathf.Infinity, ~TagsAndLayers.Layers.TriggerArea))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = camRay.GetPoint(RAYCAST_DISTANCE_LIMIT);
            }

            return _weapon.GetShootDirection(_firePoint.position, targetPoint, _timeSinceFire);
        }
        
        private void SetupPlayerInput()
        {
            if (_fireInputAction != null)
            {
                _fireInputAction.started += OnFireInputAction;
                _fireInputAction.canceled += OnFireInputAction;
            }

            if (_reloadInputAction != null)
            {
                _reloadInputAction.started += OnReloadInputAction;
            }
        }
        
        private void Awake()
        {
            _bulletSpawner = GetComponent<BulletSpawner>();
            if (_muzzleFlash == null)
            {
                Debug.LogWarning("Missing VFX Visual Effect from this weapon");
            } 
        }

        private void OnEnable()
        {
            SetupPlayerInput();
            SetCrosshairImage();
        }

        private void OnDisable()
        {
            _fireInputAction.started -= OnFireInputAction;
            _fireInputAction.canceled -= OnFireInputAction;
            _reloadInputAction.started -= OnReloadInputAction;
        }

        private void OnDestroy()
        {
            if (_weapon != null)
            {
                _weapon.OnFireWeapon -= HandleOnFire;
                _weapon.OnSetWeaponActivate -= HandleOnWeaponActivate;
                _weapon = null;
            }
        }
    }
}
