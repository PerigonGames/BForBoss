using System;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
        public event Action<Checkpoint> OnCheckpointActivation;

        private bool _isActiveCheckpoint = false;
        private bool _hasAlreadyBeenActivated = false;

        public bool IsActiveCheckpoint
        {
            set => _isActiveCheckpoint = value;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActiveCheckpoint && !_hasAlreadyBeenActivated && other.CompareTag("Player"))
            {
                OnCheckpointActivation?.Invoke(this);
                //Since only needs to trigger once
                OnCheckpointActivation = null;
            }
        }
    }
}
