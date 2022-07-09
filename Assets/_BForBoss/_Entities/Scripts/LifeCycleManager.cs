using UnityEngine;
using System;
using System.Linq;
using Perigon.Weapons;
using Sirenix.Utilities;

namespace Perigon.Entities
{
    [RequireComponent(typeof(BulletSpawner))]
    public class LifeCycleManager : MonoBehaviour
    {
        public Action<int> OnLivingEntityEliminated;
        private LifeCycleBehaviour[] _lifeCycleBehaviours = null;
        private FloatingTargetBehaviour[] _floatingTargetBehaviours = null;
        private int _totalEnemiesEliminated = 0;
        private BulletSpawner _bulletSpawner = null;
        
        public int LivingEntities => _lifeCycleBehaviours.Count(life => life.IsAlive);

        public void Initialize(Func<Transform> getPlayerPosition)
        {
            _floatingTargetBehaviours.ForEach(target => target.Initialize(getPlayerPosition, _bulletSpawner));
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
            _floatingTargetBehaviours = FindObjectsOfType<FloatingTargetBehaviour>();
            _bulletSpawner = GetComponent<BulletSpawner>();
            _lifeCycleBehaviours = FindObjectsOfType<LifeCycleBehaviour>();

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
    }
}
