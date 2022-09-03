using System;
using Perigon.Entities;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;
using UnityEngine.Pool;

namespace BForBoss
{
    [RequireComponent(typeof(BulletSpawner))]
    public class EnemyContainer: MonoBehaviour
    {
        [SerializeField] private EnemyBehaviour _enemyBehaviour;

        private IObjectPool<EnemyBehaviour> _enemyPool;
        private Func<Vector3> _targetDestination;
        private BulletSpawner _bulletSpawner;
        public EnemyBehaviour GetEnemy() => _enemyPool.Get();
        
        public void Initialize(Func<Vector3> targetDestination)
        {
            _targetDestination = targetDestination;
        }

        public void Reset()
        {
            if (_enemyPool != null)
            {
                _enemyPool.Clear();
            }
        }

        private void Awake()
        {
            _bulletSpawner = GetComponent<BulletSpawner>();
            if (_enemyBehaviour == null)
            {
                PanicHelper.Panic(new Exception("Missing EnemyBehaviour prefab from EnemyContainer"));
            }
            _enemyPool = new ObjectPool<EnemyBehaviour>(CreateEnemy, OnTakenFromPool, OnReturnedToPool);
        }

        private EnemyBehaviour CreateEnemy()
        {
            var enemy = Instantiate(_enemyBehaviour);
            if (enemy.TryGetComponent(out FloatingTargetBehaviour floatingEnemy))
            {
                floatingEnemy.Initialize(_targetDestination, _bulletSpawner);
            }

            return enemy;
        }
        
        private void OnTakenFromPool(EnemyBehaviour enemyBehaviour)
        {
            enemyBehaviour.Reset();
            enemyBehaviour.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(EnemyBehaviour enemyBehaviour)
        {
            enemyBehaviour.gameObject.SetActive(false);
        }
    }
}
