using System;
using DG.Tweening;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Entities
{
    public class PlayerLifeCycleBehaviour : LifeCycleBehaviour
    {
        [SerializeField] private PlayerHealthViewBehaviour _healthBarView = null;
        private Action _onEndGameCallback;
        private Action _onDeathCallback;
        
        public bool IsInvincible
        {
            get => _lifeCycle.IsInvincible;
            set => _lifeCycle.IsInvincible = value;
        }

        public void Initialize(Action onDeathCallback, Action onEndGameCallback)
        {
            base.Initialize();
            _healthBarView?.Initialize(_lifeCycle);
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
        }

        protected override void LifeCycleFinished()
        {
            _onDeathCallback?.Invoke();
            VisualEffectsManager.Instance.Distort(HUDVisualEffect.Death).OnComplete(() =>
            {
                _onEndGameCallback.Invoke();
            });
        }

        private void Awake()
        {
            if (_healthBarView == null)
            {
                Debug.LogWarning("Missing Health Bar View from PlayerLifeCycleBehaviour");
            }
        }
    }
}
