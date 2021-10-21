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
        
        public event Action<Checkpoint> OnEnterArea;
        
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
                OnEnterArea?.Invoke(this);
            }
        }

        private void Awake()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_showDebugBox)
            {
                var cuboid = gameObject.AddComponent<Cuboid>();
                cuboid.Size = GetComponent<BoxCollider>().size;
                cuboid.Color = _boxColor;
            }
            #endif
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_showDebugBox)
            {
                Gizmos.color = _boxColor;
                Gizmos.DrawCube(transform.position, GetComponent<BoxCollider>().size);
            }
        }
        #endif
    }
}
