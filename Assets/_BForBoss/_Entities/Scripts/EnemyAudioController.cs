using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Perigon.Entities
{
    public interface IEnemyAudioController
    {
        public void PlayIdleAudio();
        public void PlayShootingAudio();
        public void PlayAimingAudio();
    }
    
    [RequireComponent(typeof(StudioEventEmitter))]
    public class EnemyAudioController : MonoBehaviour, IEnemyAudioController
    {
        [SerializeField] private EventReference _onHitAudio;
        [SerializeField] private EventReference _onDeathAudio;
        [SerializeField] private EventReference _idleAudio;
        [SerializeField] private EventReference _chargeAudio;
        [SerializeField] private EventReference _fireAudio;

        private ILifeCycle _enemyLifeCycle;

        private StudioEventEmitter _emitter;

        public void Initialize(ILifeCycle lifeCycle)
        {
            _enemyLifeCycle = lifeCycle;
            _enemyLifeCycle.OnDeath += OnDeath;
            _enemyLifeCycle.OnDamageTaken += OnDamageTaken;
        }
        
        public void PlayIdleAudio()
        {
            _emitter.Play();
        }

        public void PlayShootingAudio()
        {
            _emitter.Stop();
            RuntimeManager.PlayOneShot(_fireAudio, transform.position);
        }

        public void PlayAimingAudio()
        {
            _emitter.Stop();
            RuntimeManager.PlayOneShot(_fireAudio, transform.position);
        }

        private void OnDeath()
        {
            _emitter.Stop();
            RuntimeManager.PlayOneShot(_onDeathAudio, transform.position);
        }

        private void OnDamageTaken()
        {
            RuntimeManager.PlayOneShot(_onHitAudio, transform.position);
        }

        private void Awake()
        {
            _emitter = GetComponent<StudioEventEmitter>();
            _emitter.AllowFadeout = true;
            _emitter.EventReference = _idleAudio;
            PlayIdleAudio();
        }

        private void OnDestroy()
        {
            if (_enemyLifeCycle != null)
            {
                _enemyLifeCycle.OnDeath -= OnDeath;
                _enemyLifeCycle.OnDamageTaken -= OnDamageTaken;
            }
        }
    }
}
