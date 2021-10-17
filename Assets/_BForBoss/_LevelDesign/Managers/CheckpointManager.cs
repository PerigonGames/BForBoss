using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class CheckpointManager : MonoBehaviour
    {
        [SerializeField] private Checkpoint[] _checkpoints;
        private Checkpoint _activeCheckpoint = null;
        private Vector3 _initialCheckpointPosition =  Vector3.zero;


        public Vector3 CurrentCheckpoint => _activeCheckpoint == null ? _initialCheckpointPosition : _activeCheckpoint.transform.position;

        public void Initialize()
        {
            if (_checkpoints.IsNullOrEmpty())
            {
                Debug.LogError("No Checkpoints found in the CheckPointManager!");
            }

            for (int i = 0; i < _checkpoints.Length; i++)
            {
                Checkpoint checkpoint = _checkpoints[i];

                if (checkpoint == null)
                {
                    Debug.LogError("There are null Checkpoints associated with the CheckpointManager");
                }

                checkpoint.OnCheckpointActivation += SetNewCheckpoint;

                if (i == 0)
                {
                    _activeCheckpoint = checkpoint;
                    _activeCheckpoint.IsActiveCheckpoint = true;
                    _initialCheckpointPosition = _activeCheckpoint.transform.position;
                }
            }
        }

        public void Reset()
        {
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                checkpoint.Reset();
            }
        }

        private void SetNewCheckpoint(Checkpoint checkpoint)
        {
            _activeCheckpoint.IsActiveCheckpoint = false;
            _activeCheckpoint = checkpoint;
            _activeCheckpoint.IsActiveCheckpoint = true;
        }


        private void OnDestroy()
        {
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                checkpoint.OnCheckpointActivation -= SetNewCheckpoint;
            }
        }
    }
}
