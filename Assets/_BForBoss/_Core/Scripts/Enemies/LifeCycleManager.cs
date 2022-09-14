using UnityEngine;
using System;
using System.Linq;
using Perigon.Entities;
using Perigon.Weapons;

namespace BForBoss
{
    public class LifeCycleManager : MonoBehaviour
    {
        public Action<int> OnLivingEntityEliminated;
        private LifeCycleBehaviour[] _lifeCycleBehaviours = null;
        private int _totalEnemiesEliminated = 0;
        private Func<Vector3> _getPlayerPosition;
        private BulletSpawner _bulletSpawner;
        
        public int LivingEntities => _lifeCycleBehaviours.Count(life => life.IsAlive);
        
        public void Reset()
        {
            if (_lifeCycleBehaviours == null) 
            {
                return;
            }
            foreach (var lifeCycle in _lifeCycleBehaviours)
            {
                lifeCycle.Reset();
            }

            _totalEnemiesEliminated = 0;
            OnLivingEntityEliminated?.Invoke(_totalEnemiesEliminated);
        }

        public void Initialize(Func<Vector3> getPlayerPosition)
        {
            _getPlayerPosition = getPlayerPosition;
            _bulletSpawner = GetComponent<BulletSpawner>();
            
            if (_lifeCycleBehaviours == null)
            {
                return;
            }

            foreach (LifeCycleBehaviour behaviour in _lifeCycleBehaviours)
            {
                behaviour.Initialize(() =>
                {
                    _totalEnemiesEliminated++;
                    OnLivingEntityEliminated?.Invoke(_totalEnemiesEliminated);
                });
            }
        }

        public void AddEnemyBehaviourFromSpawner(EnemyBehaviour enemyBehaviour, Action<EnemyBehaviour> onReleaseToSpawner)
        {
            var floatingTarget = enemyBehaviour as FloatingTargetBehaviour;
            floatingTarget.Initialize(_getPlayerPosition, _bulletSpawner, () =>
            {
                _totalEnemiesEliminated++;
                OnLivingEntityEliminated?.Invoke(_totalEnemiesEliminated);
            }, onReleaseToSpawner);
        }

        private void Awake()
        {
            _lifeCycleBehaviours = FindObjectsOfType<LifeCycleBehaviour>();
        }
    }
}
