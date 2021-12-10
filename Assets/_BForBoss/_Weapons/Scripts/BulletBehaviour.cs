using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class BulletBehaviour : MonoBehaviour, IBullet
    {
        [InlineEditor]
        [SerializeField] protected BulletPropertiesScriptableObject _properties;
        
        private ObjectPooler<BulletBehaviour> _pool = null;
        private Vector3 _startPosition;

        public ObjectPooler<BulletBehaviour> Pool
        {
            get => _pool;
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

        public event Action OnBulletSpawn;
        public event Action OnBulletDeactivate;

        public void SetSpawnAndDirection(Vector3 location, Vector3 normalizedDirection)
        {
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

        protected abstract void HandleCollision(Vector3 position);

        protected void Deactivate()
        {
            if(_pool == null)
            {
                Debug.LogError("Bullet was not initialized properly!");
                return;
            }
            
            OnBulletDeactivate?.Invoke();
            _pool.Reclaim(this);
        }

        protected void Update()
        {
            if(Vector3.Distance(transform.position, _startPosition) > _properties.MaxDistance)
            {
                Deactivate();
            }
        }
    }
}
