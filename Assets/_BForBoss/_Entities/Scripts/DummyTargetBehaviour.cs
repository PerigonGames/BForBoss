using DG.Tweening;
using UnityEngine;

namespace Perigon.Entities
{
    public class DummyTargetBehaviour : LifeCycleBehaviour
    {
        private const float DEATH_ANIMATION_DURATION = 5f;
        private const string IS_DEAD = "IsDead";
        private const string HIT = "Hit";
        private const float MAX_DISSOLVE = 1f;
        private readonly int DEATH_ID = Animator.StringToHash(IS_DEAD);
        private readonly int HIT_ID = Animator.StringToHash(HIT);
        private readonly int DISSOLVE_ID = Shader.PropertyToID("_Dissolve");
        
        
        
        private Animator _animator;
        private Renderer _renderer;
        
        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponentInChildren<Animator>();
            _renderer = GetComponentInChildren<Renderer>();
        }

        protected override void LifeCycleFinished()
        {
            _animator.SetBool(DEATH_ID, true);
            var tween = _renderer.material.DOFloat(MAX_DISSOLVE, DISSOLVE_ID, DEATH_ANIMATION_DURATION * 2);
            tween.OnComplete(() => Destroy(gameObject));
            tween.Play();
        }

        private void TriggerHitAnimation()
        {
            _animator.SetTrigger(HIT_ID);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _lifeCycle.OnDamageTaken += TriggerHitAnimation;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            _lifeCycle.OnDamageTaken -= TriggerHitAnimation;
        }
    }
}
