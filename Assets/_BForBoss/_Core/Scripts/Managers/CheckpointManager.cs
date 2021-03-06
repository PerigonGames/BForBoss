using Perigon.Analytics;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class CheckpointManager : MonoBehaviour
    {
        [SerializeField] private Checkpoint _spawnPoint = null;
        [SerializeField] private Checkpoint[] _checkpoints = null;
        [SerializeField] private Checkpoint _endPoint = null;
        private readonly BForBossAnalytics _analytics = BForBossAnalytics.Instance;
        private TimeManagerViewModel _timeManagerViewModel = null;
        private Checkpoint _activeCheckpoint = null;
        
        public Vector3 CheckpointPosition => _activeCheckpoint == null ? _spawnPoint.transform.position : _activeCheckpoint.transform.position;
        public Quaternion CheckpointRotation => _activeCheckpoint == null ? _spawnPoint.transform.rotation : _activeCheckpoint.transform.rotation;

        public void Initialize(TimeManagerViewModel timeManagerViewModel)
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

                checkpoint.OnEnterArea += SetNewCheckpoint;
            }

            _endPoint.OnEnterArea += OnEnteredLastPoint;
            _timeManagerViewModel = timeManagerViewModel;
        }

        public void Reset()
        {
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                checkpoint.Reset();
            }

            _activeCheckpoint = null;
        }

        private void SetNewCheckpoint(Checkpoint checkpoint)
        {
            _activeCheckpoint = checkpoint;
            _activeCheckpoint.SetCheckpoint();
            
            _analytics.LogCheckpointEvent(gameObject.scene.name, _timeManagerViewModel.CurrentGameTime, _activeCheckpoint.name);
        }

        private void OnEnteredLastPoint(Checkpoint _)
        {
            var stateManager = StateManager.Instance;
            if (stateManager.GetState() == State.Play)
            {
                stateManager.SetState(State.EndRace);
            }
        }
        
        private void OnDestroy()
        {
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                checkpoint.OnEnterArea -= SetNewCheckpoint;
            }
        }
    }
}
