using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public interface IFloatingEnemyAnimation
    {
        void SetShootingAnimation();
        void SetIdleAnimation();
        void SetMovementAnimation();
    }
    
    public class FloatingEnemyAnimationBehaviour : MonoBehaviour, IFloatingEnemyAnimation
    {
        private const string ANIMATION_DEATH_KEY = "Death";
        private const string ANIMATION_MOVEMENT_KEY = "Movement";
        private const string ANIMATION_SHOOTING_KEY = "Shooting";

        [Resolve] [SerializeField] private Animator _animator = null;
        [SerializeField] private GameObject _mainBody;
        [SerializeField] private GameObject _spawnEffect;
        [SerializeField] private float _spawnAnimationDuration = 1f;
        private float _elapsedSpawnAnimationDuration = 0;

        private Action _onSpawnComplete;
        
        public void Initialize(Action onSpawnComplete)
        {
            if (_animator == null)
            {
                PanicHelper.Panic(new Exception("Animator missing from Floating Enemy Animation Behaviour"));
            }

            _onSpawnComplete = onSpawnComplete;
            _elapsedSpawnAnimationDuration = _spawnAnimationDuration;
        }

        public void OnAnimationFixedUpdate()
        {
            _elapsedSpawnAnimationDuration -= Time.fixedDeltaTime;
            if (_elapsedSpawnAnimationDuration <= 0)
            {
                _mainBody.SetActive(true);
                _spawnEffect.SetActive(false);
                _onSpawnComplete?.Invoke();
            }
        }

        public void Reset()
        {
            _elapsedSpawnAnimationDuration = _spawnAnimationDuration;
            _mainBody.SetActive(false);
            _spawnEffect.SetActive(true);
        }

        public void SetMovementAnimation()
        {
            _animator.SetBool(ANIMATION_MOVEMENT_KEY, true);
            _animator.SetBool(ANIMATION_SHOOTING_KEY, false);
        }

        public void SetDeathAnimation()
        {
            _animator.SetTrigger(ANIMATION_DEATH_KEY);
        }

        public void SetIdleAnimation()
        {
            _animator.SetBool(ANIMATION_MOVEMENT_KEY, false);
            _animator.SetBool(ANIMATION_SHOOTING_KEY, false);
        }

        public void SetShootingAnimation()
        {
            _animator.SetBool(ANIMATION_MOVEMENT_KEY, false);
            _animator.SetBool(ANIMATION_SHOOTING_KEY, true);
        }
    }
}
