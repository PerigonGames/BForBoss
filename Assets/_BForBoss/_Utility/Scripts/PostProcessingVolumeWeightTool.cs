using DG.Tweening;
using UnityEngine.Rendering;

namespace Perigon.Utility
{
    public class PostProcessingVolumeWeightTool
    {
        private readonly Volume _postProcessingVolume = null;
        private readonly float _distortDuration = 0;
        private readonly float _revertDuration = 0;
        private readonly float _startValue = 0f;
        private readonly float _endValue = 0f;

        public PostProcessingVolumeWeightTool(Volume volume, float duration = 0.25f, float startValue = 0f, float endValue = 1f)
        {
            _postProcessingVolume = volume;
            _postProcessingVolume.weight = 0;
            _distortDuration = duration;
            _revertDuration = duration * 2;
            _startValue = startValue;
            _endValue = endValue;
        }

        public Tweener Distort()
        {
            return DOTween.To(intensity => _postProcessingVolume.weight = intensity, _startValue, _endValue, _distortDuration);
        }

        public Tweener Revert()
        {
            return DOTween.To(intensity => _postProcessingVolume.weight = intensity, _endValue, _startValue, _revertDuration);
        }

        public void InstantDistortAndRevert()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(Distort());
            sequence.Append(Revert());
        }
        
    }
}