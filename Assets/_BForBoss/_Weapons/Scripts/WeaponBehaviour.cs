using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        private Camera _mainCamera = null;
        [SerializeField] private InputActionAsset _actions;
        [InlineEditor]
        [SerializeField] private WeaponScriptableObject _weaponScriptableObject;
        
        private bool _isFiring = false;
        private float _elapsedRateOfFire = 0;
        protected InputAction _fireInputAction { get; set; }
        protected IWeapon _weaponProperty;

        private bool CanShoot => _elapsedRateOfFire < 0;
        
        public void Initialize(IWeapon weaponProperty = null)
        {
            _weaponProperty = weaponProperty ?? _weaponScriptableObject;
        }

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

        private void Awake()
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
            
            _fireInputAction = _actions.FindAction("Fire");
            if (_fireInputAction != null)
            {
                _fireInputAction.started += OnFire;
                _fireInputAction.canceled += OnFire;
                _fireInputAction.Enable();
            }
        }

        private void OnFire(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isFiring = true;
            }
            
            if (context.canceled)
            {
                _isFiring = false;
                _elapsedRateOfFire = 0;
            }
        }

        protected virtual void Update()
        {
            if (!_isFiring)
            {
                return;
            }

            _elapsedRateOfFire -= Time.deltaTime;
            if (CanShoot)
            {
                _elapsedRateOfFire = _weaponProperty.RateOfFire;
                GenerateBullet(MainCamera.transform.position + MainCamera.transform.TransformDirection(Vector3.forward));
            }
        }

        private void GenerateBullet(Vector3 position)
        {
            var bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.GetComponent<Collider>().enabled = false;
            bullet.transform.position = position;
        }
    }
}
