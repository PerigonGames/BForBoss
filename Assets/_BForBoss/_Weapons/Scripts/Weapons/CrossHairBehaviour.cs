using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.Weapons
{
    public interface ICrossHairProvider
    {
        void SetCrossHairImage(Sprite sprite);
        void ActivateHitMarker(bool isDead);
        void SetDefaultCrossHair();
    }
    public class CrossHairBehaviour : MonoBehaviour, ICrossHairProvider
    {
        private readonly Color KillHitMarkerColor = Color.red;
        private readonly Color HitMarkerColor = Color.white;
        
        [SerializeField] private Sprite _defaultCrossHair = null;
        [Resolve][SerializeField] private Image _crossHair = null;

        [Title("HitMarkers")]
        [SerializeField] private float _hitMarkerStayOnScreenTime = 0.2f;
        [Resolve][SerializeField] private Image _hitMarker = null;

        private float _elapsedHitMarkerTime = 0;
        private float _elapsedKillMarkerTime = 0;
            
        public void SetDefaultCrossHair()
        {
            _crossHair.sprite = _defaultCrossHair;
        }

        public void SetCrossHairImage(Sprite image)
        {
            _crossHair.sprite = image;
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
        }

        private void OnValidate()
        {
            if (_defaultCrossHair == null)
            {
                PanicHelper.Panic(new Exception("Crosshair Behaviour missing sprite"));
            }

            if (_hitMarker == null)
            {
                PanicHelper.Panic(new Exception("Crosshair behaviour missing hit marker"));
            }
            
            if (_crossHair == null)
            {
                PanicHelper.Panic(new Exception("Crosshair behaviour missing crosshair"));
            }
        }
    }
}
