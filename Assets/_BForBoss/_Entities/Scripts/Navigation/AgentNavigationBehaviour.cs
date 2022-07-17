using System;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Perigon.Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentNavigationBehaviour : MonoBehaviour
    {
        private Func<Vector3> _destination = null;
        private NavMeshAgent _agent = null;
        private Action _onDestinationReached;
        
        public void Initialize(Func<Vector3> navigationDestination, Action onDestinationReached)
        {
            _destination = navigationDestination;
            _onDestinationReached = onDestinationReached;
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
            if (_destination != null)
            {
                var destination = _destination();
                if (Vector3.Distance(transform.position, destination) > _agent.stoppingDistance * 2)
                {
                    _agent.isStopped = false;
                    _agent.destination = destination;
                }
                else
                {
                    _agent.isStopped = true;
                    _onDestinationReached?.Invoke();
                }
            }
        }
    }
}
