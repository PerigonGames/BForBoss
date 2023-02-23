using System;

namespace BForBoss
{
    public enum MovementSoundState
    {
        NotPlaying, Running, WallRunning
    }

    public class CharacterMovementAudio
    {
        private readonly IPlayerMovementStates _playerMovementStates = null;
        private MovementSoundState _state = MovementSoundState.NotPlaying;
        private readonly float _minSpeed = 0;

        public event Action<MovementSoundState> OnSoundStateChange;

        public CharacterMovementAudio(IPlayerMovementStates playerMovementStates, float minSpeed)
        {
            _playerMovementStates = playerMovementStates;
            _minSpeed = minSpeed;
        }

        public void OnUpdate()
        {
            var oldState = _state;
            switch (_state)
            {
                case MovementSoundState.NotPlaying:
                    _state = GetNextStateFromNotPlaying();
                    break;
                case MovementSoundState.Running:
                    _state = GetNextStateFromRunning();
                    break;
                case MovementSoundState.WallRunning:
                    _state = GetNextStateFromWallRunning();
                    break;
            }

            if (oldState != _state)
            {
                OnSoundStateChange?.Invoke(_state);
            }
        }

        public float GetPlayerSpeedNormalized()
        {
            if (_state == MovementSoundState.Running)
            {
                return _playerMovementStates.GetNormalizedSpeed();
            }

            return 0;
        }
        
        private MovementSoundState GetNextStateFromNotPlaying()
        {
            if (_playerMovementStates.IsLowerThanSpeed(_minSpeed))
            {
                return MovementSoundState.NotPlaying;
            }
            
            if (_playerMovementStates.IsWallRunning)
            {
                return MovementSoundState.WallRunning;
            }
            
            if (_playerMovementStates.IsRunning)
            {
                return MovementSoundState.Running;
            }
            return MovementSoundState.NotPlaying;
        }
        
        private MovementSoundState GetNextStateFromRunning()
        {
            if (_playerMovementStates.IsLowerThanSpeed(_minSpeed) || 
                (!_playerMovementStates.IsWallRunning && !_playerMovementStates.IsRunning))
            {
                return MovementSoundState.NotPlaying;
            }
            if (_playerMovementStates.IsWallRunning)
            {
                return MovementSoundState.WallRunning;
            }
            return MovementSoundState.Running;
        }
        
        private MovementSoundState GetNextStateFromWallRunning()
        {
            if (_playerMovementStates.IsLowerThanSpeed(_minSpeed) || 
                (!_playerMovementStates.IsWallRunning && !_playerMovementStates.IsRunning))
            {
                return MovementSoundState.NotPlaying;
            }
            if (!_playerMovementStates.IsWallRunning)
            {
                return MovementSoundState.Running;
            }
            return MovementSoundState.WallRunning;
        }
    }
}
