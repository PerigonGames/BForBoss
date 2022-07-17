using System;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Perigon.Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentNavigationBehaviour : MonoBehaviour
    {
        private Func<Vector3> Destination = null;
        private NavMeshAgent _agent = null;
        private Action OnDestinationReached;
        
        public void Initialize(Func<Vector3> navigationDestination, Action onDestinationReached)
        {
            Destination = navigationDestination;
            OnDestinationReached = onDestinationReached;
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null)
            {
                PanicHelper.Panic(new Exception("AgentNavigationBehaviour is missing a NavMeshAgent"));
            }
        }

        public void MovementUpdate()
        {
            if (Destination != null)
            {
                var destination = Destination();
                if (Vector3.Distance(transform.position, destination) > _agent.stoppingDistance * 2)
                {
                    _agent.isStopped = false;
                    _agent.destination = destination;
                }
                else
                {
                    _agent.isStopped = true;
                    OnDestinationReached?.Invoke();
                }
            }
        }
    }
}
