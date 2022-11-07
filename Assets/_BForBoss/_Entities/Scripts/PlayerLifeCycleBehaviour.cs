using System;
using DG.Tweening;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Entities
{
    public class PlayerLifeCycleBehaviour : LifeCycleBehaviour
    {
        private Action _onEndGameCallback;
        private Action _onDeathCallback;

        [SerializeField] private FMODUnity.EventReference _healthSFX;
        
        public bool IsInvincible
        {
            get => _lifeCycle.IsInvincible;
            set => _lifeCycle.IsInvincible = value;
        }

        public bool IsFullHealth => _lifeCycle.IsFullHealth;

        public void Initialize(LifeCycle lifeCycle, Action onDeathCallback, Action onEndGameCallback)
        {
            base.Initialize(lifeCycle);
            _onDeathCallback = onDeathCallback;
            _onEndGameCallback = onEndGameCallback;
        }

        protected override void LifeCycleDamageTaken()
        {
            base.LifeCycleDamageTaken();
            VisualEffectsManager.Instance.DistortAndRevert(HUDVisualEffect.DamageTaken);
        }

        protected override void LifeCycleOnHeal()
        {
            VisualEffectsManager.Instance.DistortAndRevert(HUDVisualEffect.Heal);
            FMODUnity.RuntimeManager.PlayOneShot(_healthSFX, transform.position);
        }

        protected override void LifeCycleFinished()
        {
            _onDeathCallback?.Invoke();
            VisualEffectsManager.Instance.Distort(HUDVisualEffect.Death).OnComplete(() =>
            {
                _onEndGameCallback.Invoke();
            });
        }
    }
}
