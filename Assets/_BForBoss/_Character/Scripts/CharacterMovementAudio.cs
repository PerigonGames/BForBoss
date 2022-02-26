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
            PlayStateAudio(_state, oldState);
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

        private MovementSoundState GetNextStateFromNotPlaying()
        {
            if (_player.Speed < _minSpeed) 
                return MovementSoundState.NotPlaying;
            if (_player.IsWallRunning())
            {
                return MovementSoundState.WallRunning;
            }
            else if (_player.IsWalking())
            {
                return MovementSoundState.Running;
            }
            return MovementSoundState.NotPlaying;
        }
        
        private MovementSoundState GetNextStateFromRunning()
        {
            bool isWalkingOrWallRunning = _player.IsWallRunning() || _player.IsWalking();
            if (_player.Speed < _minSpeed || !isWalkingOrWallRunning)
            {
                return MovementSoundState.NotPlaying;
            }
            if (_player.IsWallRunning())
            {
                return MovementSoundState.WallRunning;
            }
            return MovementSoundState.Running;
        }
        
        private MovementSoundState GetNextStateFromWallRunning()
        {
            bool isWalkingOrWallRunning = _player.IsWallRunning() || _player.IsWalking();
            if (_player.Speed < _minSpeed || !isWalkingOrWallRunning)
            {
                return MovementSoundState.NotPlaying;
            }
            if (!_player.IsWallRunning())
            {
                return MovementSoundState.Running;
            }
            return MovementSoundState.WallRunning;
        }

        private void PlayStateAudio(MovementSoundState state, MovementSoundState oldState)
        {
            if (oldState == state) return;
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
            }
        }

        private enum MovementSoundState
        {
            NotPlaying, Running, WallRunning
        }
    }
}
