using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    [RequireComponent(typeof(BulletSpawner))]
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        private const float FutherestDistanceToRayCast = 50f;
        private readonly Vector3 CenterOfCameraPosition = new Vector3(0.5f, 0.5f, 0);
        [SerializeField] private InputActionAsset _actions;
        [SerializeField] protected Transform _firePoint = null;
        [SerializeField] private CrosshairBehaviour _crosshair = null;
        [InlineEditor]
        [SerializeField] private WeaponScriptableObject _weaponScriptableObject;

        protected Weapon _weapon = null;
        private InputAction FireInputAction { get; set; }
        private InputAction ReloadInputAction { get; set; }
        private Camera _mainCamera = null;
        private BulletSpawner _bulletSpawner;

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

        public void Initialize(IWeaponProperties properties = null)
        {
            _bulletSpawner = GetComponent<BulletSpawner>();
            var weaponProperty = properties ?? _weaponScriptableObject;
            _weapon = new Weapon(weaponProperty);
            BindWeapon();

            _crosshair.SetCrosshairImage(weaponProperty.Crosshair);
        }

        private void BindWeapon()
        {
            _weapon.OnFireWeapon += HandleOnFire;
        }

        protected abstract void OnFireInputAction(InputAction.CallbackContext context);
        protected abstract void Update();

        private void OnReloadInputAction(InputAction.CallbackContext context)
        {
            _weapon.IsReloading = true;
        }

        private void HandleOnFire(int numberOfBullets)
        {
            for (int i = 0; i < numberOfBullets; i++)
            {
                GenerateBullet(_firePoint.position, GetDirectionOfShot());
            }
        }

        private void GenerateBullet(Vector3 position, Vector3 fireDirection)
        {
            _bulletSpawner
                .SpawnBullet(_weaponScriptableObject.TypeOfBullet)
                .SetSpawnAndDirection(position, fireDirection);
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
                targetPoint = camRay.GetPoint(FutherestDistanceToRayCast);
            }

            return _weapon.GetShootDirection(_firePoint.position, targetPoint);
        }


        private void Start()
        {
            Initialize();
            SetupPlayerInput();
        }

        private void SetupPlayerInput()
        {
            if (_actions == null)
            {
                Debug.LogWarning("Input Action Asset is missing from weapons behaviour");
                return;
            }

            ReloadInputAction = _actions.FindAction("Reload");
            if (ReloadInputAction != null)
            {
                ReloadInputAction.started += OnReloadInputAction;
                ReloadInputAction.Enable();
            }

            FireInputAction = _actions.FindAction("Fire");
            if (FireInputAction != null)
            {
                FireInputAction.started += OnFireInputAction;
                FireInputAction.canceled += OnFireInputAction;
                FireInputAction.Enable();
            }
        }

        private void OnDisable()
        {
            FireInputAction.started -= OnFireInputAction;
            FireInputAction.canceled -= OnFireInputAction;
            FireInputAction.Disable();

            ReloadInputAction.started -= OnReloadInputAction;
            ReloadInputAction.Disable();

            _weapon.OnFireWeapon -= HandleOnFire;
        }
    }
}
