using System;
using System.Collections;
using System.Collections.Generic;
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
        private Tween _timeScaleTween;

        public float CurrentTimeScale => Time.timeScale;

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
            }else if (_isSlowMotionActive && callbackContext.canceled)
            {
                StopSlowMotion();
            }
        }

        private void StartSlowMotion()
        {
            _isSlowMotionActive = true;
            if (_timeScaleTween.IsActive())
            {
                _timeScaleTween.Kill();
            }

            _timeScaleTween = DOTween.To(() => CurrentTimeScale, 
                SetTimeScale, 
                _targetTimeScale,
                _tweenDuration);
        }

        private void StopSlowMotion()
        {
            _isSlowMotionActive = false;
            if (_timeScaleTween.IsActive())
            {
                _timeScaleTween.Kill();
            }

            _timeScaleTween = DOTween.To(() => CurrentTimeScale, 
                SetTimeScale, 
                DEFAULT_TIME_SCALE,
                _tweenDuration);
        }

        private void SetTimeScale(float targetTimeScale)
        {
            Time.timeScale = targetTimeScale;
            Time.fixedDeltaTime = _fixedDeltaTime * targetTimeScale;
        }
    }
}
