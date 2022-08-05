using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Perigon.Utility
{
    public enum HUDVisualEffect
    {
        Health,
        SlowMotion,
        Dash
    }
    public class VisualEffectsManager : MonoBehaviour
    {
        private const float HEALTH_VFX_DELAY_BEFORE_REVERT = 1f;
        private const string HEALTH_PASS_EMISSION_KEY = "_EmissionStrength";

        [Resolve][SerializeField] private CustomPassVolume _healthPassVolume = null;
        private CustomPassVolumeWeightTool _healthVFXTool;

        public void Initialize()
        {
            _healthVFXTool = new CustomPassVolumeWeightTool(_healthPassVolume, HEALTH_PASS_EMISSION_KEY);
        }
        
        public void DistortAndRevert(HUDVisualEffect effect)
        {
            var pass = GetCustomPass(effect);
            pass?.InstantDistortAndRevert(delayBeforeRevert: HEALTH_VFX_DELAY_BEFORE_REVERT);
        }

        public Tweener Distort(HUDVisualEffect effect)
        {
            var pass = GetCustomPass(effect);
            return pass?.Distort();
        }
        
        public Tweener Revert(HUDVisualEffect effect)
        {
            var pass = GetCustomPass(effect);
            return pass?.Revert();
        }

        private CustomPassVolumeWeightTool GetCustomPass(HUDVisualEffect effect)
        {
            switch (effect)
            {
                case HUDVisualEffect.Dash:
                    return null;
                case HUDVisualEffect.Health:
                    return _healthVFXTool;
                case HUDVisualEffect.SlowMotion:
                    return null;
            }

            return null;
        }
        
        public void Reset()
        {
            _healthVFXTool.Reset();
        }

        private void OnDestroy()
        {
            Reset();
        }
    }
}
