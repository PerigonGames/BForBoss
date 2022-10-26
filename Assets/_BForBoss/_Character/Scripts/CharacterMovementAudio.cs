using System;

namespace Perigon.Character
{
    public enum MovementSoundState
    {
        NotPlaying, Running, WallRunning
    }
    
    public class CharacterMovementAudio
    {
        private readonly ICharacterMovement _characterMovement = null;
        private MovementSoundState _state = MovementSoundState.NotPlaying;
        private float _minSpeed = 0;

        public event Action<MovementSoundState> OnSoundStateChange;

        public CharacterMovementAudio(ICharacterMovement characterMovement, float minSpeed)
        {
            _characterMovement = characterMovement;
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
            return _state == MovementSoundState.Running
                ? _characterMovement.SpeedMagnitude / _characterMovement.CharacterMaxSpeed
                : 0f;
        }
        
        private MovementSoundState GetNextStateFromNotPlaying()
        {
            if (_characterMovement.SpeedMagnitude < _minSpeed) 
                return MovementSoundState.NotPlaying;
            if (_characterMovement.IsWallRunning)
            {
                return MovementSoundState.WallRunning;
            }
            
            if (_characterMovement.IsWalking())
            {
                return MovementSoundState.Running;
            }
            return MovementSoundState.NotPlaying;
        }
        
        private MovementSoundState GetNextStateFromRunning()
        {
            bool isWalkingOrWallRunning = _characterMovement.IsWallRunning || _characterMovement.IsWalking();
            if (_characterMovement.SpeedMagnitude < _minSpeed || !isWalkingOrWallRunning)
            {
                return MovementSoundState.NotPlaying;
            }
            if (_characterMovement.IsWallRunning)
            {
                return MovementSoundState.WallRunning;
            }
            return MovementSoundState.Running;
        }
        
        private MovementSoundState GetNextStateFromWallRunning()
        {
            bool isWalkingOrWallRunning = _characterMovement.IsWallRunning || _characterMovement.IsWalking();
            if (_characterMovement.SpeedMagnitude < _minSpeed || !isWalkingOrWallRunning)
            {
                return MovementSoundState.NotPlaying;
            }
            if (!_characterMovement.IsWallRunning)
            {
                return MovementSoundState.Running;
            }
            return MovementSoundState.WallRunning;
        }
    }
}
