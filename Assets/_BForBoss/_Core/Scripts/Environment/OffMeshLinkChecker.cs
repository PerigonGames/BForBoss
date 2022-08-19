using UnityEngine;
using UnityEngine.AI;

namespace BForBoss
{
    public class OffMeshLinkChecker : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent = null;
        [SerializeField, Tooltip("How much should the cost of a mesh link multiply by when in use?")]
        private float _multiplier = 1000f;

        private OffMeshLink _link = null;
        private float _oldCost;

        private void Update()
        {
            if (_agent.currentOffMeshLinkData.valid)
            {
                AcquireOffMeshLink();
            }
            else
            {
                ReleaseOffMeshLink();
            }
        }

        private void AcquireOffMeshLink()
        {
            if (_link != null)
            {
                return;
            }
            
            _link = _agent.currentOffMeshLinkData.offMeshLink;
            _oldCost = _link.costOverride;
            _link.costOverride = _oldCost <= 0.0f ? (_multiplier * 1) : _oldCost * _multiplier;
        }

        private void ReleaseOffMeshLink()
        {
            if (_link == null)
            {
                return;
            }

            _link.costOverride = _oldCost;
            _link = null;
        }

        private void OnDestroy()
        {
            ReleaseOffMeshLink();
        }

        private void OnValidate()
        {
            if (_agent == null)
            {
                Debug.LogWarning("agent missing from OffMeshLinkChecker", this);
            }
        }
    }
}
