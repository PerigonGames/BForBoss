using System;
using Perigon.Entities;
using UnityEngine.Pool;

namespace BForBoss
{
    public class EnemyContainer
    {
        private ObjectPool<EnemyBehaviour> _enemyPool;

        private EnemyBehaviourManager _enemyBehaviourManager;
        private Action _onRelease;

        public EnemyContainer(EnemyBehaviour enemyAgent, EnemyBehaviourManager enemyBehaviourManager, Action onRelease)
        {
            _enemyBehaviourManager = enemyBehaviourManager;
            _onRelease = onRelease;
            
            if (_enemyPool == null)
            {
                SetupPool(enemyAgent);
            }
        }

        public EnemyBehaviour Get()
        {
            return _enemyPool.Get();
        }

        public void Reset()
        {
            if (_enemyPool != null)
            {
                _enemyPool.Clear();
            }
        }

        private void SetupPool(EnemyBehaviour enemyAgentToPool)
        {
            _enemyPool = new ObjectPool<EnemyBehaviour>(() => GenerateEnemy(enemyAgentToPool),
                (enemy) =>
                {
                    enemy.gameObject.SetActive(true);
                },
                (enemy) =>
                {
                    enemy.gameObject.SetActive(false);
                    _onRelease?.Invoke();
                });
        }

        //public ObjectPool<EnemyBehaviour> EnemyPool => _enemyPool;

        private void Release(EnemyBehaviour behaviour)
        {
            _enemyPool.Release(behaviour);
        }

        private EnemyBehaviour GenerateEnemy(EnemyBehaviour enemyAgent)
        {
            FloatingTargetBehaviour floatingEnemy = enemyAgent as FloatingTargetBehaviour;
            
            if (floatingEnemy != null)
            {
                floatingEnemy.Initialize(_enemyBehaviourManager.GetPlayerPosition, _enemyBehaviourManager.BulletSpawner, null, Release);
            
                return floatingEnemy;
            }
            
            return null;
            //return enemyAgent;
        }
    }
}
