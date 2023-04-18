using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class BulletBehaviour : MonoBehaviour, IBullet
    {
        private ObjectPooler<BulletBehaviour> _pool = null;

        [InlineEditor]
        [SerializeField] protected BulletPropertiesScriptableObject _properties;
        protected Vector3 _startPosition;
        protected bool _isActive = false;

        public Transform HomingTarget { get; set; }

        public ObjectPooler<BulletBehaviour> Pool
        {
            set
            {
                if (_pool == null)
                    _pool = value;
                else
                {
                    Debug.LogError("Bullet is getting initialized with a pool twice!");
                }
            }
        }
        
        public LayerMask Mask { protected get; set; }
        public Action<Vector3, Vector3> OnBulletHitWall { get; set; }

        public event Action OnBulletSpawn;
        public event Action<IBullet> OnBulletDeactivate;
        public event Action<IBullet, bool> OnBulletHitEntity;

        public void SetSpawnAndDirection(Vector3 location, Vector3 normalizedDirection)
        {
            _isActive = true;
            if (normalizedDirection == Vector3.zero)
            {
                Deactivate();
            }
            else
            {
                _startPosition = location;
                transform.SetPositionAndRotation(location, Quaternion.LookRotation(normalizedDirection));
                OnBulletSpawn?.Invoke();
            }
        }

        protected void Deactivate()
        {
            _isActive = false;
            if(_pool == null)
            {
                Debug.LogError("Bullet was not initialized properly! Destroying GameObject");
                Destroy(gameObject);
                return;
            }
        
            OnBulletDeactivate?.Invoke(this);
            _pool.Reclaim(this);
        }

        protected virtual void Update()
        {
            if(Vector3.Distance(transform.position, _startPosition) > _properties.MaxTravelDistance)
            {
                Deactivate();
            }
        }
    }
}
