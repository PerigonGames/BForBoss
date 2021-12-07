using System;
using Perigon.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract class BulletBehaviour : MonoBehaviour, IBullet
    {
        protected IBulletProperties _properties;
        private ObjectPooler<BulletBehaviour> _pool = null;

        private Vector3 _startPosition;

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

        public void Initialize(IBulletProperties properties, ObjectPooler<BulletBehaviour> pool)
        {
            _properties = properties;
            _pool = pool;
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
