using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Perigon.Utility
{
    public class CustomPassVolumeWeightTool: IVolumeWeightTool
    {
        private readonly Material _material;
        private readonly string _materialKey;
        private readonly float _distortDuration;
        private readonly float _revertDuration;
        private readonly float _startValue;
        private readonly float _endValue;

        public CustomPassVolumeWeightTool(CustomPassVolume customPassVolume, string materialKey, float duration = 0.25f, float startValue = 0f, float endValue = 1f)
        {
            foreach (var pass in customPassVolume.customPasses)
            {
                if (pass is FullScreenCustomPass f)
                {
#if UNITY_EDITOR
                    var mat = new Material(f.fullscreenPassMaterial);
                    f.fullscreenPassMaterial = mat;
                    _material = mat;
#else
                    _material = f.fullscreenPassMaterial;
#endif
                }
            }
            
            if (_material == null)
            {
                Debug.LogWarning("CustomPassVolume unable to find FullScreenCustomPass");
            }

            _materialKey = materialKey;
            _distortDuration = duration;
            _revertDuration = duration * 2;
            _startValue = startValue;
            _endValue = endValue;
        }
        
        public Tweener Distort()
        {
            return DOTween.To(intensity => _material.SetFloat(_materialKey, intensity), _startValue, _endValue, _distortDuration).SetUpdate(true);
        }

        public Tweener Revert()
        {
            return DOTween.To(intensity => _material.SetFloat(_materialKey, intensity), _endValue, _startValue, _revertDuration).SetUpdate(true);
        }

        public void Reset()
        {
            _material.SetFloat(_materialKey, _startValue);
        }

        public void InstantDistortAndRevert(float delayBeforeRevert = 0)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(Distort());
            sequence.Append(Revert().SetDelay(delayBeforeRevert));
        }
    }
}
