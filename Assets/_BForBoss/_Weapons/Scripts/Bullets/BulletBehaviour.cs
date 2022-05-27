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

        protected void SpawnWallHitPrefab(Vector3 position, Vector3 wallNormal)
        {
            _wallHitVFXObjectPool ??= new ObjectPooler<WallHitVFX>(
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
            vfx.Spawn(_properties.BulletHoleTimeToLive);
        }

        private void OnValidate()
        {
            if (_wallHitVFXPrefab == null)
            {
                PanicHelper.Panic(new Exception("No wall hit prefab set!"));
            }
        }
    }
}
