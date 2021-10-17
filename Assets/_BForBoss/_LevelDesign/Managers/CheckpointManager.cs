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

        //Todo: Pass Player position (as starting Checkpoint) when player Manager is finalized
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
                    _activeCheckpoint.SetCheckpoint();
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
            _activeCheckpoint = checkpoint;
            _activeCheckpoint.SetCheckpoint();
        }


        private void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.Space].wasPressedThisFrame)
            {
                Debug.Log($"Current Checkpoint Position : {CurrentCheckpoint}");
            }
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
