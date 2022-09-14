using System;
using UnityEngine;
using UnityEngine.AI;

namespace BForBoss
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentNavigationBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private float _stopDistanceBeforeReachingDestination = 5;
        private Func<Vector3> _destination;
        private NavMeshAgent _agent;
        private Action _onDestinationReached;
        private IsLineOfSightBlocked _isLineOfSightBlocked;

        public void Initialize(Func<Vector3> navigationDestination, IsLineOfSightBlocked isLineOfSightBlocked, Action onDestinationReached)
        {
            _destination = navigationDestination;
            _onDestinationReached = onDestinationReached;
            _isLineOfSightBlocked = isLineOfSightBlocked;
        }
        
        public void MovementUpdate()
        {
            if (_destination == null || !_agent.enabled)
            {
                return;
            }
            _agent.destination = _destination();
            if (ReachedDestination())
            {
                if (_isLineOfSightBlocked.Execute())
                {
                    _agent.isStopped = false;
                }
                else
                {
                    _agent.isStopped = true;
                    _onDestinationReached?.Invoke();
                }
            }
            else
            {
                _agent.isStopped = false;
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

        private bool ReachedDestination()
        {
            if (_agent.remainingDistance > 0)
            {
                return _agent.remainingDistance < _stopDistanceBeforeReachingDestination;
            }

            return false;
        }
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, _stopDistanceBeforeReachingDestination);
        }
    }
}


