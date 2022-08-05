using System;
using UnityEngine;
using Perigon.Utility;

namespace Perigon.Entities
{
    public class PlayerLifeCycleBehaviour : LifeCycleBehaviour
    {
        [SerializeField] private PlayerHealthViewBehaviour _healthBarView = null;
        private VisualEffectsManager _visualEffectsManager = null;
        private Action _onDeathCallBack = null;

        public override void Initialize(Action onDeathCallback)
        {
            base.Initialize(onDeathCallback);
            _healthBarView.Initialize(_lifeCycle);
            _onDeathCallBack = onDeathCallback;
        }

        protected override void LifeCycleDamageTaken()
        {
            base.LifeCycleDamageTaken();
            _visualEffectsManager.DistortAndRevert(HUDVisualEffect.Health);
        }

        protected override void LifeCycleFinished()
        {
            _onDeathCallBack.Invoke();
        }

        private void Awake()
        {
            _visualEffectsManager = FindObjectOfType<VisualEffectsManager>();

            if (_visualEffectsManager == null)
            {
                Debug.LogWarning("Missing Visual effects manager from PlayerLifeCycleBehaviour");
            }
            
            if (_healthBarView == null)
            {
                Debug.LogWarning("Missing Health Bar View from PlayerLifeCycleBehaviour");
            }
        }
    }
}
