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

        private FirstPersonPlayer _player;
        private CharacterMovementAudio _movementAudio = null;

        private void Awake()
        {
            _player = GetComponent<FirstPersonPlayer>();
            _movementAudio = new CharacterMovementAudio(_player, _minSpeed);
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
    }
}
