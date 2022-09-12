using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Perigon.Utility
{
    public enum HUDVisualEffect
    {
        DamageTaken,
        Heal,
        SlowMotion,
        Dash,
        Death
    }
    public class VisualEffectsManager : MonoBehaviour
    {
        private const float DAMAGE_TAKEN_VFX_DELAY_BEFORE_REVERT = 1f;
        private const string EMISSION_STRENGTH_KEY = "_EmissionStrength";

        private static VisualEffectsManager _instance = null;

        [Resolve][SerializeField] private CustomPassVolume _damageTakenPassVolume;
        [Resolve][SerializeField] private CustomPassVolume _healPassVolume;
        [Resolve][SerializeField] private Volume _slowMotionVolume;
        [Resolve][SerializeField] private Volume _dashVolume;
        [Resolve][SerializeField] private Volume _deathVolume;
        private IVolumeWeightTool _damageTakenVFXTool;
        private IVolumeWeightTool _healVFXTool;
        private IVolumeWeightTool _slowMotionVFXTool;
        private IVolumeWeightTool _dashVFXTool;
        private IVolumeWeightTool _deathVFXTool;
        private Camera _mainCamera = null;

        public static VisualEffectsManager Instance => _instance;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _instance = this;
            _damageTakenPassVolume.targetCamera = _mainCamera;
            _healPassVolume.targetCamera = _mainCamera;
            _damageTakenVFXTool = new CustomPassVolumeWeightTool(_damageTakenPassVolume, EMISSION_STRENGTH_KEY);
            _healVFXTool = new CustomPassVolumeWeightTool(_healPassVolume, EMISSION_STRENGTH_KEY);
            _slowMotionVFXTool = new PostProcessingVolumeWeightTool(_slowMotionVolume);
            _dashVFXTool = new PostProcessingVolumeWeightTool(_dashVolume);
            _deathVFXTool = new PostProcessingVolumeWeightTool(_deathVolume);
        }
        
        public void DistortAndRevert(HUDVisualEffect effect)
        {
            var pass = GetCustomPass(effect);

            float DelayBeforeRevert()
            {
                switch (effect)
                {
                    case HUDVisualEffect.DamageTaken:
                        return DAMAGE_TAKEN_VFX_DELAY_BEFORE_REVERT;
                    default:
                        return 0;
                }
            }
            pass?.InstantDistortAndRevert(delayBeforeRevert: DelayBeforeRevert());
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

        private IVolumeWeightTool GetCustomPass(HUDVisualEffect effect)
        {
            switch (effect)
            {
                case HUDVisualEffect.Dash:
                    return _dashVFXTool;
                case HUDVisualEffect.DamageTaken:
                    return _damageTakenVFXTool;
                case HUDVisualEffect.Heal:
                    return _healVFXTool;
                case HUDVisualEffect.SlowMotion:
                    return _slowMotionVFXTool;
                case HUDVisualEffect.Death:
                    return _deathVFXTool;
            }

            return null;
        }
        
        public void Reset()
        {
            _damageTakenVFXTool.Reset();
            _healVFXTool.Reset();
            _dashVFXTool.Reset();
            _slowMotionVFXTool.Reset();
            _deathVFXTool.Reset();
        }

        private void OnDestroy()
        {
            Reset();
        }
    }
}
