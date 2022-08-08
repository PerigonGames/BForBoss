using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Perigon.Utility
{
    public enum HUDVisualEffect
    {
        DamageTaken,
        Heal,
        SlowMotion,
        Dash
    }
    public class VisualEffectsManager : MonoBehaviour
    {
        private const float HEALTH_VFX_DELAY_BEFORE_REVERT = 1f;
        private const string EMISSION_STRENGTH_KEY = "_EmissionStrength";

        private static VisualEffectsManager _instance = null;

        [Resolve][SerializeField] private CustomPassVolume _damageTakenPassVolume = null;
        [Resolve][SerializeField] private CustomPassVolume _healPassVolume = null;
        private CustomPassVolumeWeightTool _damageTakenVFXTool;
        private CustomPassVolumeWeightTool _healVFXTool;
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
                    Debug.LogWarning("Missing Dash VFX from VisualEffectsManager");
                    return null;
                case HUDVisualEffect.DamageTaken:
                    return _damageTakenVFXTool;
                case HUDVisualEffect.Heal:
                    return _healVFXTool;
                case HUDVisualEffect.SlowMotion:
                    Debug.LogWarning("Missing SlowMotion VFX from VisualEffectsManager");
                    return null;
            }

            return null;
        }
        
        public void Reset()
        {
            _damageTakenVFXTool.Reset();
            _healVFXTool.Reset();
        }

        private void OnDestroy()
        {
            Reset();
        }
    }
}
