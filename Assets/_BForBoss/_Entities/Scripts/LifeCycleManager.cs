using UnityEngine;
using System;
using System.Linq;

namespace Perigon.Entities
{
    public class LifeCycleManager : MonoBehaviour
    {
        public Action<int> OnLivingEntityEliminated;
        private LifeCycleBehaviour[] _lifeCycleBehaviours = null;
        private int _totalEnemiesEliminated = 0;
        private Func<Vector3> _getPlayerPosition;
        
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
            enemyBehaviour.Initialize(_getPlayerPosition, () =>
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
