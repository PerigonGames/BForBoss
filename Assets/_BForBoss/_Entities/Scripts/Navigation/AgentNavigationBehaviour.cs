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
            _agent.destination = _destination();
            if (ReachedDestination())
            {
                if (IsLineOfSightBlocked())
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
        
        private bool IsLineOfSightBlocked()
        {
            var direction = _destination() - (transform.position + Vector3.up); 
            if (Physics.Raycast((transform.position + Vector3.up), direction.normalized, out var hitInfo))
            {
                Debug.DrawRay((transform.position + Vector3.up), direction.normalized, Color.red);
                if (hitInfo.collider.GetComponent<PlayerLifeCycleBehaviour>() != null)
                {
                    return false;
                }
            }

            return true;
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
