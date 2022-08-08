using System;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Entities
{
    public class PlayerLifeCycleBehaviour : LifeCycleBehaviour
    {
        [SerializeField] private PlayerHealthViewBehaviour _healthBarView = null;
        private Action _onDeathCallBack = null;

        public override void Initialize(Action onDeathCallback)
        {
            base.Initialize(onDeathCallback);
            _healthBarView?.Initialize(_lifeCycle);
            _onDeathCallBack = onDeathCallback;
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
            _onDeathCallBack.Invoke();
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
