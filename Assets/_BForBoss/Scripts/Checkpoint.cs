using System;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        private bool _showDebugBox = false;
        [ShowIf("_showDebugBox")]
        [SerializeField]
        private Color _boxColor = Color.cyan;
        
        public event Action<Checkpoint> OnCheckpointActivation;
        
        private bool _hasBeenActivated = false;
        
        public void Reset()
        {
            _hasBeenActivated = false;
        }

        public void SetCheckpoint()
        {
            _hasBeenActivated = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_hasBeenActivated && other.CompareTag(Tags.Player))
            {
                OnCheckpointActivation?.Invoke(this);
            }
        }

        private void Awake()
        {
            if (_showDebugBox && Debug.isDebugBuild)
            {
                var cuboid = gameObject.AddComponent<Cuboid>();
                cuboid.Size = GetComponent<BoxCollider>().size;
                cuboid.Color = _boxColor;
            }
        }
    }
}
