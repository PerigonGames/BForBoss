using System;
using UnityEngine;
using FMODUnity;

namespace Perigon.Character
{
    public class CharacterMovementAudio : MonoBehaviour
    {
        [SerializeField] private EventReference _dashAudio;
        [SerializeField] private EventReference _slideAudio;
        [SerializeField] private EventReference _jumpAudio;

        [SerializeField] private StudioEventEmitter _runningAudio;
        [SerializeField] private StudioEventEmitter _wallrunAudio;

        [SerializeField] private float _minSpeed = 0.1f;

        private FirstPersonPlayer _player;

        private MovementSoundState _state;
        
        private void Awake()
        {
            _player = GetComponent<FirstPersonPlayer>();
        }

        private void Update()
        {
            switch (_state)
            {
                case MovementSoundState.NotPlaying:
                    _state = CheckNotPlaying();
                    break;
                case MovementSoundState.Running:
                    _state = CheckRunning();
                    break;
                case MovementSoundState.WallRunning:
                    _state = CheckWallRunning();
                    break;
                default:
                    break;
            }
        }

        private void OnEnable()
        {
            if (_player != null)
            {
                _player.Jumped += PlayJumpSound;
                _player.Dashed += PlayDashSound;
                _player.Slid += PlaySlideSound;
            }
        }
        
        private void OnDisable()
        {
            if (_player != null)
            {
                _player.Jumped -= PlayJumpSound;
                _player.Dashed -= PlayDashSound;
                _player.Slid -= PlaySlideSound;
            }
        }

        private void PlayJumpSound()
        {
            RuntimeManager.PlayOneShot(_jumpAudio, transform.position);
        }
        
        private void PlaySlideSound()
        {
            RuntimeManager.PlayOneShot(_slideAudio, transform.position);
        }
        
        private void PlayDashSound()
        {
            RuntimeManager.PlayOneShot(_dashAudio, transform.position);
        }

        private MovementSoundState CheckNotPlaying()
        {
            if (_player.Speed < _minSpeed) 
                return MovementSoundState.NotPlaying;
            if (_player.IsWallRunning())
            {
                var newState = MovementSoundState.WallRunning;
                PlayState(newState);
                return newState;
            }
            else if (_player.IsWalking())
            {
                var newState = MovementSoundState.Running;
                PlayState(newState);
                return newState;
            }
            return MovementSoundState.NotPlaying;
            }
        
        private MovementSoundState CheckRunning()
        {
            bool isWalkingOrWallRunning = _player.IsWallRunning() || _player.IsWalking();
            if (_player.Speed < _minSpeed || !isWalkingOrWallRunning)
            {
                var newState = MovementSoundState.NotPlaying;
                PlayState(newState);
                return newState;
            }
            if (_player.IsWallRunning())
            {
                var newState = MovementSoundState.WallRunning;
                PlayState(newState);
                return newState;
            }
            return MovementSoundState.Running;
        }
        
        private MovementSoundState CheckWallRunning()
        {
            bool isWalkingOrWallRunning = _player.IsWallRunning() || _player.IsWalking();
            if (_player.Speed < _minSpeed || !isWalkingOrWallRunning)
            {
                var newState = MovementSoundState.NotPlaying;
                PlayState(newState);
                return newState;
            }
            if (!_player.IsWallRunning())
            {
                var newState = MovementSoundState.Running;
                PlayState(newState);
                return newState;
            }
            return MovementSoundState.WallRunning;
        }

        private void PlayState(MovementSoundState state)
        {
            switch (state)
            {
                case MovementSoundState.NotPlaying:
                    _wallrunAudio.Stop();
                    _runningAudio.Stop();
                    break;
                case MovementSoundState.Running:
                    _wallrunAudio.Stop();
                    _runningAudio.Play();
                    break;
                case MovementSoundState.WallRunning:
                    _runningAudio.Stop();
                    _wallrunAudio.Play();
                    break;
                default:
                    break;
            }
        }

        private enum MovementSoundState
        {
            NotPlaying, Running, WallRunning
        }
    }
}
