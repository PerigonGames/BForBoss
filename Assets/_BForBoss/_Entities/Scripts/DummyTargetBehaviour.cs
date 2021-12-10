using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Entities
{
    public class DummyTargetBehaviour : LifeCycleBehaviour
    {
        private const float DEATH_ANIMATION_DURATION = 5f;
        private const string IS_DEAD = "IsDead";
        private const string HIT = "Hit";
        private readonly int DEATH_ID = Animator.StringToHash(IS_DEAD);
        private readonly int HIT_ID = Animator.StringToHash(HIT);
        
        private Animator _animator;
        
        
        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponentInChildren<Animator>();
        }

        protected override void LifeCycleFinished()
        {
            _animator.SetBool(DEATH_ID, true);
            Destroy(gameObject, DEATH_ANIMATION_DURATION);
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
