using DG.Tweening;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Character
{
    public class PlayerSlowMotionBehaviour : MonoBehaviour
    {
        private const float DEFAULT_TIME_SCALE = 1.0f;
        
        [SerializeField, Range(0f,1f)]
        private float _targetTimeScale = 0.25f;

        [SerializeField] private float _tweenDuration = 1.0f;
        
        private bool _isSlowMotionActive = false;
        private float _fixedDeltaTime;
        private Sequence _timeScaleTween;

        private float CurrentTimeScale => Time.timeScale;
        
        private void Start()
        {
            _fixedDeltaTime = Time.fixedDeltaTime;
        }
        
        public void OnSlowMotion(bool isSlowingTime)
        {
            if (!_isSlowMotionActive && isSlowingTime)
            {
                StartSlowMotion();
            }
            else if (_isSlowMotionActive && !isSlowingTime)
            {
                StopSlowMotion();
            }
        }

        private void StartSlowMotion()
        {
            _isSlowMotionActive = true;
            SetupSlowMotionTweens(_targetTimeScale, VisualEffectsManager.Instance.Distort(HUDVisualEffect.SlowMotion));
        }

        private void StopSlowMotion()
        {
            _isSlowMotionActive = false;
            SetupSlowMotionTweens(DEFAULT_TIME_SCALE, VisualEffectsManager.Instance.Revert(HUDVisualEffect.SlowMotion));
        }

        private void SetupSlowMotionTweens(float targetVal, Tween vfxTween)
        {
            if (_timeScaleTween.IsActive())
            {
                _timeScaleTween.Kill();
            }
            _timeScaleTween = DOTween.Sequence();
            _timeScaleTween.Append(DOTween.To(() => CurrentTimeScale, 
                SetTimeScale, 
                targetVal,
                _tweenDuration));
            if (vfxTween != null)
            {
                _timeScaleTween.Join(vfxTween);
            }
            _timeScaleTween.timeScale = 1f;
            _timeScaleTween.Play();
        }

        private void SetTimeScale(float targetTimeScale)
        {
            Time.timeScale = targetTimeScale;
            Time.fixedDeltaTime = _fixedDeltaTime * targetTimeScale;
        }
    }
}
