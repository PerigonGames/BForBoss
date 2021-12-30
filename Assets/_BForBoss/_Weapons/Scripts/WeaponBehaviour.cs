using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    [RequireComponent(typeof(BulletSpawner))]
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        private const float RAYCAST_DISTANCE_LIMIT = 50f;
        private readonly Vector3 CenterOfCameraPosition = new Vector3(0.5f, 0.5f, 0);
        [SerializeField] protected Transform _firePoint = null;
        [SerializeField] private CrosshairBehaviour _crosshair = null;
        [InlineEditor]
        [SerializeField] private WeaponScriptableObject _weaponScriptableObject;
        
        protected Weapon _weapon = null;

        private InputAction _fireInputAction = null;
        private InputAction _reloadInputAction = null;

        private Camera _mainCamera = null;
        private BulletSpawner _bulletSpawner;

        public Weapon WeaponViewModel => _weapon;

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

        protected abstract void OnFireInputAction(InputAction.CallbackContext context);
        protected abstract void Update();

        private void OnReloadInputAction(InputAction.CallbackContext context)
        {
            _weapon.ReloadWeaponIfPossible();
        }

        private void HandleOnFire(int numberOfBullets)
        {
            for (int i = 0; i < numberOfBullets; i++)
            {
                _bulletSpawner
                    .SpawnBullet(_weaponScriptableObject.TypeOfBullet)
                    .SetSpawnAndDirection(_firePoint.position, GetDirectionOfShot());
            }
        }

        private Vector3 GetDirectionOfShot()
        {
            var camRay = MainCamera.ViewportPointToRay(CenterOfCameraPosition);
            Vector3 targetPoint;
            if (Physics.Raycast(camRay, out var hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = camRay.GetPoint(RAYCAST_DISTANCE_LIMIT);
            }

            return _weapon.GetShootDirection(_firePoint.position, targetPoint);
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

        private void SetCrosshairImage()
        {
            if (_weapon != null)
            {
                _crosshair.SetCrosshairImage(_weapon.Crosshair);
            }
        }

        private void Awake()
        {
            _bulletSpawner = GetComponent<BulletSpawner>();
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
