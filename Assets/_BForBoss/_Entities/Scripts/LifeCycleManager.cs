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
        
        public int LivingEntities => _lifeCycleBehaviours.Count(life => life.IsAlive);

        public void EliminateLivingEntity()
        {
            _totalEnemiesEliminated++;
            OnLivingEntityEliminated?.Invoke(_totalEnemiesEliminated);
        }
        
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

        private void Awake()
        {
            _lifeCycleBehaviours = FindObjectsOfType<LifeCycleBehaviour>();

            if (_lifeCycleBehaviours == null)
            {
                return;
            }

            foreach (LifeCycleBehaviour behaviour in _lifeCycleBehaviours)
            {
                behaviour.NotifyOnDeath(this);
            }
        }
    }
}
