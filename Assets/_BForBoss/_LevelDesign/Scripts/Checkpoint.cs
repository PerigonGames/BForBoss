using System;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
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
    }
}
