using UnityEngine;
using System.Linq;

namespace Perigon.Entities
{
    public class LifeCycleManager : MonoBehaviour
    {
        private LifeCycleBehaviour[] _lifeCycleBehaviours = null;

        public int LivingEntities => _lifeCycleBehaviours.Count(life => life.IsAlive);

        public void Initialize()
        {
            _lifeCycleBehaviours = FindObjectsOfType<LifeCycleBehaviour>();
        }

        public void Reset()
        {
            if (_lifeCycleBehaviours == null) return;
            foreach (var lifeCycle in _lifeCycleBehaviours)
            {
                lifeCycle.Reset();
            }
        }
    }
}
