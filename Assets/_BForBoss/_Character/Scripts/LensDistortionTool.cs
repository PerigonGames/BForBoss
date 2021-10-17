using DG.Tweening;
using UnityEngine.Rendering.Universal;

namespace BForBoss
{
    /// <summary>
    /// Tool to distort the lens.
    /// WARNING/NOTE - Post Processing > Volume > LensDistortion - make sure intensity is turned on
    /// For some reason there's no built in function to enable this automatically. 
    /// </summary>
    public class LensDistortionTool
    {
        private const float Distortion_Amount = 0.3f;
        private readonly LensDistortion _lens = null;
        private readonly float _duration = 0;

        public LensDistortionTool(LensDistortion lens, float duration)
        {
            _lens = lens;
            _lens.active = true;            
            _lens.intensity.value = 0;
            _duration = duration;
        }

        public void Distort()
        {
            DOTween.To(intensity => _lens.intensity.value = intensity, 0, Distortion_Amount, _duration);
        }

        public void Revert()
        {
            DOTween.To(intensity => _lens.intensity.value = intensity, Distortion_Amount, 0, _duration * 2);
        }
    }
}