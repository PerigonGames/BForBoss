using System;
using Sirenix.Utilities;
using UnityEngine;

namespace Perigon.Entities
{
    public class EnemyNavigationManager : MonoBehaviour
    {
        private AgentNavigationBehaviour[] _navMeshAgents = { };

        public void Initialize(Func<Vector3> getPlayerPosition)
        {
            foreach (var agent in _navMeshAgents)
            {
                agent.Initialize(getPlayerPosition);
            }
        }
        
        private void Awake()
        {
            _navMeshAgents = FindObjectsOfType<AgentNavigationBehaviour>();
            if (_navMeshAgents.IsNullOrEmpty())
            {
                Debug.LogWarning("There are no NavMeshAgents found in this scene");
            }
        }
    }
}