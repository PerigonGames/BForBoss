using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Character
{
    public class PlayerSlowMotionBehaviour : MonoBehaviour
    {
        private const float DEFAULT_TIME_SCALE = 1.0f;
        
        [SerializeField, Range(0f,1f)]
        private float _targetTimeScale = 0.25f;

        [SerializeField] private float _tweenDuration = 1.0f;
        
        private InputAction _slowMotionInputAction;
        private bool _isSlowMotionActive = false;
        private float _fixedDeltaTime;
        private Sequence _timeScaleTween;

        public Func<Tween> OnSlowMotionStart { private get; set; }
        public Func<Tween> OnSlowMotionStopped { private get; set; }

        private float CurrentTimeScale => Time.timeScale;
        
        private void Start()
        {
            _fixedDeltaTime = Time.fixedDeltaTime;
        }

        public void SetupPlayerInput(InputAction slowMoInput)
        {
            _slowMotionInputAction = slowMoInput;
            _slowMotionInputAction.started += OnSlowMotion;
            _slowMotionInputAction.canceled += OnSlowMotion;
        }

        private void OnSlowMotion(InputAction.CallbackContext callbackContext)
        {
            if (!_isSlowMotionActive && callbackContext.started)
            {
                StartSlowMotion();
            }
            else if (_isSlowMotionActive && callbackContext.canceled)
            {
                StopSlowMotion();
            }
        }

        private void StartSlowMotion()
        {
            _isSlowMotionActive = true;
            SetupSlowMotionTweens(_targetTimeScale, OnSlowMotionStart?.Invoke());
        }

        private void StopSlowMotion()
        {
            _isSlowMotionActive = false;
            SetupSlowMotionTweens(DEFAULT_TIME_SCALE, OnSlowMotionStopped?.Invoke());
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
        
        public void OnOnEnable()
        {
            _slowMotionInputAction.Enable();
        }
        
        public void OnOnDisable()
        {
            _slowMotionInputAction.Disable();
        }
    }
}
