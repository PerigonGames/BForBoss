using System;
using UnityEngine;
using UnityEngine.AI;

namespace Perigon.Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentNavigationBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private float _stopDistanceBeforeReachingDestination = 5;
        private Func<Vector3> _destination = null;
        private NavMeshAgent _agent = null;
        private Action _onDestinationReached;

        public float StopDistanceBeforeReachingDestination => _stopDistanceBeforeReachingDestination;

        public void Initialize(Func<Vector3> navigationDestination, Action onDestinationReached)
        {
            _destination = navigationDestination;
            _onDestinationReached = onDestinationReached;
        }
        
        public void MovementUpdate()
        {
            if (_destination == null || !_agent.enabled)
            {
                return;
            }
            
            var destination = _destination();
                
            if (Vector3.Distance(transform.position, destination) > _stopDistanceBeforeReachingDestination)
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

        public void PauseNavigation()
        {
            if (_agent.enabled)
            {
                _agent.isStopped = true;
                _agent.enabled = false;
            }
        }
        
        public void ResumeNavigation()
        {
            if (!_agent.enabled)
            {
                _agent.enabled = true;
                if (_agent.isOnNavMesh)
                {
                    _agent.isStopped = false;
                }
            }
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = _stopDistanceBeforeReachingDestination;
        }
    }
}
