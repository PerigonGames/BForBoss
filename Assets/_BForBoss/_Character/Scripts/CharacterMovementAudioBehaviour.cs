using UnityEngine;
using FMODUnity;

namespace Perigon.Character
{
    
    public class CharacterMovementAudioBehaviour : MonoBehaviour
    {
        [SerializeField] private EventReference _dashAudio;
        [SerializeField] private EventReference _slideAudio;
        [SerializeField] private EventReference _jumpAudio;

        [SerializeField] private StudioEventEmitter _runningAudio;
        [SerializeField] private StudioEventEmitter _wallrunAudio;

        [SerializeField] private float _minSpeed = 0.1f;

        private PlayerMovementBehaviour _playerMovementBehaviour;
        private CharacterMovementAudio _movementAudio = null;

        private void Awake()
        {
            _playerMovementBehaviour = GetComponent<PlayerMovementBehaviour>();
            _movementAudio = new CharacterMovementAudio(_playerMovementBehaviour, _minSpeed);
            _movementAudio.OnSoundStateChange += MovementAudioOnOnSoundStateChange;
        }

        private void OnDestroy()
        {
            if (_movementAudio != null)
            {
                _movementAudio.OnSoundStateChange -= MovementAudioOnOnSoundStateChange;
            }
        }

        private void MovementAudioOnOnSoundStateChange(MovementSoundState state)
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
            }
        }

        private void Update()
        {
            _movementAudio.OnUpdate();
        }

        private void OnEnable()
        {
            if (_playerMovementBehaviour != null)
            {
                _playerMovementBehaviour.Jumped += PlayJumpSound;
                _playerMovementBehaviour.OnDashActivated += PlayOnDashSound;
                _playerMovementBehaviour.Slid += PlaySlideSound;
            }
        }
        
        private void OnDisable()
        {
            if (_playerMovementBehaviour != null)
            {
                _playerMovementBehaviour.Jumped -= PlayJumpSound;
                _playerMovementBehaviour.OnDashActivated -= PlayOnDashSound;
                _playerMovementBehaviour.Slid -= PlaySlideSound;
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
        
        private void PlayOnDashSound()
        {
            RuntimeManager.PlayOneShot(_dashAudio, transform.position);
        }
    }
}
