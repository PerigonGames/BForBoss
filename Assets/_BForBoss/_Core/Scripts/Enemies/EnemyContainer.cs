using System;
using System.Collections.Generic;
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
        private List<EnemyBehaviour> _activeEnemies = new List<EnemyBehaviour>();


        public EnemyBehaviour GetEnemy() => _enemyPool.Get();
        
        public void Initialize(Func<Vector3> targetDestination)
        {
            _targetDestination = targetDestination;
        }

        public void Reset()
        {
            _enemyPool.Clear();
            _activeEnemies.Clear();
            _bulletSpawner.Reset();
        }

        public void CleanUp()
        {
            foreach (var enemy in _activeEnemies)
            {
                Destroy(enemy.gameObject);
            }
            _activeEnemies.Clear();
        }

        private void Awake()
        {
            _bulletSpawner = GetComponent<BulletSpawner>();
            if (_enemyBehaviour == null)
            {
                PanicHelper.Panic(new Exception("Missing EnemyBehaviour prefab from EnemyContainer"));
            }
            _enemyPool = new ObjectPool<EnemyBehaviour>(
                CreateEnemy, 
                OnTakenFromPool, 
                OnReturnedToPool, 
                OnDestroyEnemy);
        }

        private EnemyBehaviour CreateEnemy()
        {
            var enemy = Instantiate(_enemyBehaviour);
            enemy.Pool = _enemyPool;
            if (enemy.TryGetComponent(out FloatingTargetBehaviour floatingEnemy))
            {
                floatingEnemy.Initialize(_targetDestination, _bulletSpawner);
            }
            
            return enemy;
        }
        
        private void OnTakenFromPool(EnemyBehaviour enemyBehaviour)
        {
            _activeEnemies.Add(enemyBehaviour);
            enemyBehaviour.Reset();
            enemyBehaviour.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(EnemyBehaviour enemyBehaviour)
        {
            _activeEnemies.Remove(enemyBehaviour);
            enemyBehaviour.gameObject.SetActive(false);
        }

        private void OnDestroyEnemy(EnemyBehaviour enemyBehaviour)
        {
            Destroy(enemyBehaviour.gameObject);
        }
    }
}
