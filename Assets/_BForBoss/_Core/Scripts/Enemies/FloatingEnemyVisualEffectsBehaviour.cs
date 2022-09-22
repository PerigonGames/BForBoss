using System;
using Perigon.Utility;
using Perigon.VFX;
using PerigonGames;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Rigidbody))]
    public class FloatingEnemyVisualEffectsBehaviour : MonoBehaviour
    {
        private IRandomUtility _randomUtility;
        
        [Title("Spawn Effect Properties")]
        [SerializeField] private GameObject _mainBody;
        [SerializeField] private GameObject _spawnEffect;
        [SerializeField] private float _spawnAnimationDuration = 1f;
        private float _elapsedSpawnAnimationDuration = 0;

        [Title("Death Effect Properties")] 
        [Resolve] [SerializeField] private TimedVFXEffect _explosionEffect;
        [SerializeField] private float _deathRagDollForce = 3f;
        [SerializeField] private float _deathFallMaxDuration = 2f;
        private Rigidbody _rigidbody;
        private float _elapsedFallTime = 1f;

        private Action _onSpawnVisualsComplete;
        private Action _onDeathVisualsComplete;

        public void Initialize(Action onSpawnVisualsComplete, Action onDeathVisualsComplete)
        {
            _onSpawnVisualsComplete = onSpawnVisualsComplete;
            _onDeathVisualsComplete = onDeathVisualsComplete;
            _elapsedSpawnAnimationDuration = _spawnAnimationDuration;
            _randomUtility = new RandomUtility(gameObject.GetInstanceID());
        }
        
        public void OnSpawningFixedUpdate()
        {
            _elapsedSpawnAnimationDuration -= Time.fixedDeltaTime;
            if (_elapsedSpawnAnimationDuration <= 0)
            {
                _mainBody.SetActive(true);
                _spawnEffect.SetActive(false);
                _onSpawnVisualsComplete?.Invoke();
            }
        }

        public void OnDeathFixedUpdate()
        {
            if (_elapsedFallTime < 0)
            {
                return;
            }

            _elapsedFallTime -= Time.fixedDeltaTime;
            if (_elapsedFallTime <= 0)
            {
                _mainBody.SetActive(false);
                _explosionEffect.StartEffect();
            }
        }

        public void StartDeathVisual()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.AddTorque(new Vector3(RandomDirection(), RandomDirection(), RandomDirection()));
            _rigidbody.AddForce(new Vector3(RandomDirection(), RandomDirection(), RandomDirection()), ForceMode.Impulse);
        }

        private float RandomDirection()
        {
            var direction =  _randomUtility.CoinFlip() ? 1 : -1;
            return direction * (float)_randomUtility.NextDouble() * _deathRagDollForce;
        }

        public void Reset()
        {
            _elapsedFallTime = _deathFallMaxDuration * (float) _randomUtility.NextDouble();
            _elapsedSpawnAnimationDuration = _spawnAnimationDuration;
            _mainBody.SetActive(false);
            _spawnEffect.SetActive(true);
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            if (_mainBody == null)
            {
                PanicHelper.Panic(new Exception("_mainBody missing from FloatingEnemyVisualEffectsBehaviour"));
            }
            
            if (_spawnEffect == null)
            {
                PanicHelper.Panic(new Exception("_spawnEffect missing from FloatingEnemyVisualEffectsBehaviour"));
            }

            if (_explosionEffect == null)
            {
                PanicHelper.Panic(new Exception("_explosionEffect missing from FloatingEnemyVisualEffectsBehaviour"));
            }

            _explosionEffect.OnEffectStop += OnExplosionEffectStopped;
        }

        private void OnDestroy()
        {
            _explosionEffect.OnEffectStop -= OnExplosionEffectStopped;
        }

        private void OnExplosionEffectStopped()
        {
            _onDeathVisualsComplete?.Invoke();
        }
    }
}
