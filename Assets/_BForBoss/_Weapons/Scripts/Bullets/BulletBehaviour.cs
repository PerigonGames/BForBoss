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

        
        public Action<Vector3, Vector3> OnBulletHitWall { get; set; }

        public event Action OnBulletSpawn;
        public event Action<IBullet> OnBulletDeactivate;
        public event Action<IBullet, bool> OnBulletHitEntity;

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

        protected void Deactivate()
        {
            if(_pool == null)
            {
                Debug.LogError("Bullet was not initialized properly!");
                return;
            }
            
            OnBulletDeactivate?.Invoke(this);
            _pool.Reclaim(this);
        }

        protected void Update()
        {
            if(Vector3.Distance(transform.position, _startPosition) > _properties.MaxTravelDistance)
            {
                Deactivate();
            }
        }
    }
}
