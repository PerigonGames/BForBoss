using UnityEngine;
using System;
using System.Linq;

namespace Perigon.Entities
{
    public class LifeCycleManager : MonoBehaviour
    {
        public Action OnLivingEntitiesAmountChanged;
        private LifeCycleBehaviour[] _lifeCycleBehaviours = null;

        public int LivingEntities => _lifeCycleBehaviours.Count(life => life.IsAlive);

        public void LivingEntitiesAmountChanged()
        {
            OnLivingEntitiesAmountChanged?.Invoke();
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
        }

        private void Awake()
        {
            _lifeCycleBehaviours = FindObjectsOfType<LifeCycleBehaviour>();

            if (_lifeCycleBehaviours == null)
            {
                return;
            }

            foreach (LifeCycleBehaviour lifeCycle in _lifeCycleBehaviours)
            {
                lifeCycle.AddLifeCycleSubscriber(this);
            }
        }
    }
}
