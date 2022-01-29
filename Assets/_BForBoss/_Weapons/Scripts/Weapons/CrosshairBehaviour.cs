using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.Weapons
{
    public class CrosshairBehaviour : MonoBehaviour
    {
        private readonly Color KillHitMarkerColor = Color.red;
        private readonly Color HitMarkerColor = Color.white;
        
        [SerializeField] private Sprite _defaultCrosshair = null;
        [Resolve][SerializeField] private Image _crosshair = null;

        [Title("HitMarkers")]
        [SerializeField] private float _hitMarkerStayOnScreenTime = 0.2f;
        [Resolve][SerializeField] private Image _hitMarker = null;

        private float _elapsedHitMarkerTime = 0;
        private float _elapsedKillMarkerTime = 0;
            
        public void SetDefaultCrosshair()
        {
            _crosshair.sprite = _defaultCrosshair;
        }

        public void SetCrosshairImage(Sprite image)
        {
            _crosshair.sprite = image;
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
            SetDefaultCrosshair();
        }

        private void OnValidate()
        {
            if (_defaultCrosshair == null)
            {
                Debug.LogWarning("Crosshair Behaviour missing sprite");
            }

            if (_hitMarker == null)
            {
                Debug.LogWarning("Crosshair behaviour missing hit marker");
            }
            
            if (_crosshair == null)
            {
                Debug.LogWarning("Crosshair behaviour missing crosshair");
            }
        }
    }
}
