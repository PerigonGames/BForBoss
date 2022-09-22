using DG.Tweening;
using UnityEngine;

namespace Perigon.Entities
{
    public class DummyTargetBehaviour : LifeCycleBehaviour
    {
        [SerializeField] private HealthBarViewBehaviour _healthBar;
        private const float MAX_DISSOLVE = 1f;
        private readonly int DEATH_ID = Animator.StringToHash("IsDead");
        private readonly int HIT_ID = Animator.StringToHash("Hit");
        private readonly int DISSOLVE_ID = Shader.PropertyToID("_Dissolve");
        
        [SerializeField]
        private float _dissolveVFXDuration = 7.5f;
        
        private Animator _animator;
        private Renderer _renderer;
        private Tween _deathTween;

        public override void Initialize()
        {
            base.Initialize();
            _animator = GetComponentInChildren<Animator>();
            _renderer = GetComponentInChildren<Renderer>();
            if (_healthBar != null)
            {
                _healthBar.Initialize(_lifeCycle);
            }
            _lifeCycle.OnDamageTaken += TriggerHitAnimation;
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
            _healthBar.Reset();
        }

        public override void CleanUp()
        {
            base.CleanUp();
            _lifeCycle.OnDamageTaken -= TriggerHitAnimation;
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

        
        private void Awake()
        {
            if (_healthBar == null)
            {
                Debug.LogWarning("A dummyTargetBehaviour is missing a health bar");
            }
        }
    }
}
