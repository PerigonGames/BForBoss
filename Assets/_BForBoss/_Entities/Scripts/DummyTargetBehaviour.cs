using System;
using DG.Tweening;
using UnityEngine;

namespace Perigon.Entities
{
    public class DummyTargetBehaviour : LifeCycleBehaviour
    {
        [SerializeField] private HealthbarViewBehaviour _healthbar;
        private const string IS_DEAD = "IsDead";
        private const string HIT = "Hit";
        private const float MAX_DISSOLVE = 1f;
        private readonly int DEATH_ID = Animator.StringToHash(IS_DEAD);
        private readonly int HIT_ID = Animator.StringToHash(HIT);
        private readonly int DISSOLVE_ID = Shader.PropertyToID("_Dissolve");
        
        [SerializeField]
        private float _dissolveVFXDuration = 7.5f;
        
        private Animator _animator;
        private Renderer _renderer;
        private Tween _deathTween;

        public override void Initialize(Action onDeathCallback)
        {
            base.Initialize(onDeathCallback);
            _animator = GetComponentInChildren<Animator>();
            _renderer = GetComponentInChildren<Renderer>();
            if (_healthbar != null)
            {
                _healthbar.Initialize(_lifeCycle);
            }
            _lifeCycle.OnDamageTaken += TriggerHitAnimation;
        }
        
        protected override void LifeCycleFinished()
        {
            _animator.SetBool(DEATH_ID, true); 
            _deathTween = _renderer.material.DOFloat(MAX_DISSOLVE, DISSOLVE_ID, _dissolveVFXDuration)
             .OnComplete(() => gameObject.SetActive(false));
        }

        private void TriggerHitAnimation()
        {
            _animator.SetTrigger(HIT_ID);
        }

        public override void Reset()
        {
            if (_deathTween.IsActive())
            {
                _deathTween.Kill();
            }
            _animator.SetBool(DEATH_ID, false); 
            base.Reset();
            _lifeCycle.OnDamageTaken += TriggerHitAnimation;
            gameObject.SetActive(true);
            _renderer.material.SetFloat(DISSOLVE_ID, 0);
            _healthbar.Reset();
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _lifeCycle.OnDamageTaken -= TriggerHitAnimation;
        }
        
        private void Awake()
        {
            if (_healthbar == null)
            {
                Debug.LogWarning("A dummyTargetBehaviour is missing a health bar");
            }
        }
    }
}
