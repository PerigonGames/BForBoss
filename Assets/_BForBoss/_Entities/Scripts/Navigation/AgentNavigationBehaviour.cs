using System;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.AI;
using Perigon.Weapons;

namespace Perigon.Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentNavigationBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private float _stopDistanceBeforeReachingDestination = 5;
        private Func<Vector3> Destination = null;
        private NavMeshAgent _agent = null;
        private Action OnDestinationReached;

        public void Initialize(Func<Vector3> navigationDestination, Action onDestinationReached)
        {
            Destination = navigationDestination;
            OnDestinationReached = onDestinationReached;
        }
        
        public void MovementUpdate()
        {
            if (Destination != null)
            {
                var destination = Destination();
                
                if (Vector3.Distance(transform.position, destination) > _stopDistanceBeforeReachingDestination)
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
            if (_agent == null)
            {
                //Do we need this? the component has a RequireComponent flag on it already
                PanicHelper.Panic(new Exception("AgentNavigationBehaviour is missing a NavMeshAgent"));
            }
            _agent.stoppingDistance = _stopDistanceBeforeReachingDestination;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireSphere(transform.position, _stopDistanceBeforeReachingDestination);
        }
    }
}
