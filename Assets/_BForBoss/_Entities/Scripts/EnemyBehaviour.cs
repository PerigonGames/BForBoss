using System;
using UnityEngine;

namespace Perigon.Entities
{
    [RequireComponent(typeof(AgentNavigationBehaviour))]
    public abstract class EnemyBehaviour : LifeCycleBehaviour
    {
        protected Action<EnemyBehaviour> _onReleaseToSpawner = null;
            
        protected abstract override void LifeCycleFinished();

        public virtual void Initialize(Func<Vector3> getPlayerPosition, Action onDeathCallback,
            Action<EnemyBehaviour> onReleaseToSpawner = null)
        {
            AgentNavigationBehaviour agentNavigationBehaviour = GetComponent<AgentNavigationBehaviour>();
            
            agentNavigationBehaviour.Initialize(getPlayerPosition, () => { });
            Initialize(onDeathCallback);
            _onReleaseToSpawner = onReleaseToSpawner;
        }
    }
}
