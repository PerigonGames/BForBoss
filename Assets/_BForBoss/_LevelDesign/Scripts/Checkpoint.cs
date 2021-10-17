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

        public void Reset()
        {
            _isActiveCheckpoint = false;
            _hasAlreadyBeenActivated = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActiveCheckpoint && !_hasAlreadyBeenActivated && other.CompareTag(Tags.Player))
            {
                OnCheckpointActivation?.Invoke(this);
                _hasAlreadyBeenActivated = true;
            }
        }
    }
}
