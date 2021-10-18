using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BForBoss
{
    /// <summary>
    /// Tool to distort the lens.
    /// WARNING/NOTE - Post Processing > Volume > LensDistortion - make sure intensity is turned on
    /// For some reason there's no built in function to enable this automatically. 
    /// </summary>
    public class PostProcessingVolumeWeightTool
    {
        private readonly Volume _postProcessingVolume = null;
        private readonly float _distortDuration = 0;
        private readonly float _revertDuration = 0;

        public PostProcessingVolumeWeightTool(Volume volume, float duration)
        {
            _postProcessingVolume = volume;
            _postProcessingVolume.weight = 0;
            _distortDuration = duration;
            _revertDuration = duration * 2;
        }

        public void Distort()
        {
            DOTween.To(intensity => _postProcessingVolume.weight = intensity, 0, 1, _distortDuration);
        }

        public void Revert()
        {
            DOTween.To(intensity => _postProcessingVolume.weight = intensity, 1, 0, _revertDuration);
        }
    }
}