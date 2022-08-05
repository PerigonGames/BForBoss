using System;
using DG.Tweening;
using Perigon.Utility;
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
        
        private VisualEffectsManager _visualEffectsManager;
        private InputAction _slowMotionInputAction;
        private bool _isSlowMotionActive = false;
        private float _fixedDeltaTime;
        private Sequence _timeScaleTween;

        public float CurrentTimeScale => Time.timeScale;

        private void Awake()
        {
            _visualEffectsManager = FindObjectOfType<VisualEffectsManager>();
            if (_visualEffectsManager == null)
            {
                Debug.LogWarning("Missing Visual effects manager from PlayerLifeCycleBehaviour");
            }
        }

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
            SetupSlowMotionTweens(_targetTimeScale, tool => tool.Distort(HUDVisualEffect.SlowMotion));
        }

        private void StopSlowMotion()
        {
            _isSlowMotionActive = false;
            SetupSlowMotionTweens(DEFAULT_TIME_SCALE, tool => tool.Revert(HUDVisualEffect.SlowMotion));
        }

        private void SetupSlowMotionTweens(float targetVal, Func<VisualEffectsManager, Tweener> postProcessingFunc)
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
            if (_visualEffectsManager != null)
            {
                _timeScaleTween.Join(postProcessingFunc(_visualEffectsManager));
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
