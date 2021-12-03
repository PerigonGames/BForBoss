using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {

        [SerializeField] private InputActionAsset _actions;
        [SerializeField] protected Transform _firePoint = null;
        [InlineEditor]
        [SerializeField] private WeaponScriptableObject _weaponScriptableObject;
       
        private InputAction _fireInputAction { get; set; }
        private Camera _mainCamera = null;
        
        protected float _elapsedRateOfFire = 0;
 
        protected IWeapon _weaponProperty;

        protected bool CanShoot => _elapsedRateOfFire < 0;
        
        public void Initialize(IWeapon weaponProperty = null)
        {
            _weaponProperty = weaponProperty ?? _weaponScriptableObject;
        }
        
        protected abstract void OnFire(InputAction.CallbackContext context);
        protected abstract void Update();
        
        protected void Fire()
        {
            _elapsedRateOfFire = _weaponProperty.RateOfFire;
            GenerateBullet(_firePoint.position);
        }
        
        protected void GenerateBullet(Vector3 position)
        {
            var bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.GetComponent<Collider>().enabled = false;
            bullet.transform.position = position;
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

    }
}
