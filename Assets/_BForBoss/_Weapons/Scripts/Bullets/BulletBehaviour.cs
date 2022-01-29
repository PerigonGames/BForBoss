using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class BulletBehaviour : MonoBehaviour, IBullet
    {
        private const float WALL_HIT_ZFIGHT_BUFFER = 0.01f;
        
        [InlineEditor]
        [SerializeField] protected BulletPropertiesScriptableObject _properties;

        [SerializeField] private WallHitVFX _wallHitVFXPrefab;

        private ObjectPooler<BulletBehaviour> _pool = null;
        private Vector3 _startPosition;
        private IBulletProperties _bulletProperties;
        
        private event Action _onBulletSpawn;
        private event Action<IBullet> _onBulletDeactivate;
        private event Action<IBullet, bool> _onBulletHitEntity;

        private static ObjectPooler<WallHitVFX> _wallHitVFXObjectPool = null;

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

        event Action IBullet.OnBulletSpawn
        {
            add => this._onBulletSpawn += value;
            remove => this._onBulletSpawn -= value;
        }

        event Action<IBullet> IBullet.OnBulletDeactivate
        {
            add => _onBulletDeactivate += value;
            remove => _onBulletDeactivate -= value;
        }

        event Action<IBullet, bool> IBullet.OnBulletHitEntity
        {
            add => _onBulletHitEntity += value;
            remove => _onBulletHitEntity -= value;
        }

        protected IBulletProperties BulletProperties
        {
            get
            {
                if (_bulletProperties == null)
                {
                    _bulletProperties = _properties;
                }

                return _bulletProperties;
            }
        }

        void IBullet.SetSpawnAndDirection(Vector3 location, Vector3 normalizedDirection)
        {
            if (normalizedDirection == Vector3.zero)
            {
                Deactivate();
            }
            else
            {
                _startPosition = location;
                transform.SetPositionAndRotation(location, Quaternion.LookRotation(normalizedDirection));
                _onBulletSpawn?.Invoke();
            }
        }

        protected void Deactivate()
        {
            if(_pool == null)
            {
                Debug.LogError("Bullet was not initialized properly!");
                return;
            }
            
            _onBulletDeactivate?.Invoke(this);
            _pool.Reclaim(this);
        }

        protected void Update()
        {
            if(Vector3.Distance(transform.position, _startPosition) > BulletProperties.MaxDistance)
            {
                Deactivate();
            }
        }

        protected void SpawnWallHitPrefab(Vector3 position, Vector3 wallNormal)
        {
            if (_wallHitVFXPrefab == null)
            {
                Debug.LogWarning("No wall hit prefab set!");
                return;
            }
            
            _wallHitVFXObjectPool ??= new ObjectPooler<WallHitVFX>("WallHitVFX", 
                () =>
                {
                    var vfx = Instantiate(_wallHitVFXPrefab);
                    vfx.Initialize(_wallHitVFXObjectPool);
                    return vfx;
                },
                (vfx) => vfx.gameObject.SetActive(true),
                (vfx) =>
                {
                    vfx.Reset();
                    vfx.gameObject.SetActive(false);
                });
            
            var vfx = _wallHitVFXObjectPool.Get();
            vfx.transform.SetPositionAndRotation(position, Quaternion.LookRotation(wallNormal));
            vfx.transform.Translate(0f, 0f, WALL_HIT_ZFIGHT_BUFFER, Space.Self);
            vfx.Spawn(BulletProperties.BulletHoleTimeToLive);
        }
    }
}
