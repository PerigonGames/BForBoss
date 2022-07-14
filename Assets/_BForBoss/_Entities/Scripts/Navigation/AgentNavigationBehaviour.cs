using System;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Perigon.Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentNavigationBehaviour : MonoBehaviour, IKnockback
    {
        private Func<Vector3> Destination = null;
        private NavMeshAgent _agent = null;
        
        public void Initialize(Func<Vector3> navigationDestination)
        {
            Destination = navigationDestination;
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null)
            {
                PanicHelper.Panic(new Exception("AgentNavigationBehaviour is missing a NavMeshAgent"));
            }
        }

        private void Update()
        {
            if (Destination != null)
            {
                _agent.destination = Destination();
            }
        }

        public void ApplyKnockback(float force, Vector3 originPosition)
        {
            var direction = transform.position - originPosition;
            _agent.Move(direction * force);
        }
    }
}
