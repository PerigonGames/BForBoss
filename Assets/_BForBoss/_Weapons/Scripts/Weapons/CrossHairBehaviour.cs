using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.Weapons
{
    public class CrossHairBehaviour : MonoBehaviour
    {
        [FoldoutGroup("CrossHair Configurations")]
        [SerializeField]
        private float _maxCrossHairOffsetFromCenter = 75f;
        [FoldoutGroup("CrossHair Configurations")] 
        [SerializeField]
        private float _expandingCrossHairSpeed = 50f;
        [FoldoutGroup("CrossHair Configurations")] 
        [SerializeField]
        private float _shrinkingCrossHairSpeed = 20f;
        
        private readonly Color KillHitMarkerColor = Color.red;
        private readonly Color HitMarkerColor = Color.white;

        [SerializeField] private RectTransform _crossHairRectTransform = null;
        
        [SerializeField] private Sprite _defaultCrossHair = null;

        [Title("HitMarkers")]
        [SerializeField] private float _hitMarkerStayOnScreenTime = 0.2f;
        [Resolve][SerializeField] private Image _hitMarker = null;

        private float _elapsedHitMarkerTime = 0;
        private float _elapsedKillMarkerTime = 0;
        private bool _shouldSetToMaxSize = false;    
        
        public void SetDefaultCrossHair()
        {
            //_crossHair.sprite = _defaultCrossHair;
        }

        public void SetMaximumSize()
        {
            _shouldSetToMaxSize = true;
        }

        public void ActivateHitMarker(bool isDead)
        {
            if (isDead)
            {
                _elapsedKillMarkerTime = _hitMarkerStayOnScreenTime;
            }
            
            _elapsedHitMarkerTime = _hitMarkerStayOnScreenTime;
        }

        private void Update()
        {
            OnShoot();
            HitMarkerUpdate();
        }

        private void OnShoot()
        {
            if (_shouldSetToMaxSize)
            {
                var width = Mathf.Lerp(_crossHairRectTransform.rect.width, _maxCrossHairOffsetFromCenter, Time.deltaTime * _expandingCrossHairSpeed);
                _crossHairRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                if (Math.Abs(_crossHairRectTransform.rect.width - _maxCrossHairOffsetFromCenter) < 0.01)
                {
                    _shouldSetToMaxSize = false;
                }
            }
            else
            {
                var width = Mathf.Lerp(_crossHairRectTransform.rect.width, 0, Time.deltaTime * _shrinkingCrossHairSpeed);
                _crossHairRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }
        }

        private void HitMarkerUpdate()
        {
            _elapsedHitMarkerTime -= Time.deltaTime;
            _elapsedKillMarkerTime -= Time.deltaTime;
            if (_elapsedKillMarkerTime > 0)
            {
                _hitMarker.color = KillHitMarkerColor;
            } 
            else if (_elapsedHitMarkerTime > 0)
            {
                _hitMarker.color = HitMarkerColor;
            }
            else
            {
                _hitMarker.color = Color.clear;
            }
        }
        
        private void Awake()
        {
            SetDefaultCrossHair();
            if (_defaultCrossHair == null)
            {
                PanicHelper.Panic(new Exception("Crosshair Behaviour missing sprite"));
            }

            if (_hitMarker == null)
            {
                PanicHelper.Panic(new Exception("Crosshair behaviour missing hit marker"));
            }
        }
    }
}
