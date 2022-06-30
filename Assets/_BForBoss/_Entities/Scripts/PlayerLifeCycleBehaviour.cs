using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Perigon.Utility;

namespace Perigon.Entities
{
    public class PlayerLifeCycleBehaviour : LifeCycleBehaviour
    {
        private const string HEALTH_PASS_EMISSION_KEY = "_EmissionStrength";
        [SerializeField] private CustomPassVolume _healthPassVolume = null;
        private Action _onDeathCallBack = null;
        private CustomPassVolumeWeightTool _customPassTool = null;
        
        public override void Initialize(Action onDeathCallback)
        {
            base.Initialize(onDeathCallback);
            _onDeathCallBack = onDeathCallback;
        }

        protected override void LifeCycleDamageTaken()
        {
            base.LifeCycleDamageTaken();
            _customPassTool.InstantDistortAndRevert(delayBeforeRevert: 1f);
        }

        protected override void LifeCycleFinished()
        {
            _onDeathCallBack?.Invoke();
        }

        private void Awake()
        {
            if (_healthPassVolume == null)
            {
                Debug.LogWarning("Missing Health Pass Volume VFX from PlayerLifeCycleBehaviour");
            }
            else
            {
                _customPassTool = new CustomPassVolumeWeightTool(_healthPassVolume, HEALTH_PASS_EMISSION_KEY);
            }
        }
    }
}
