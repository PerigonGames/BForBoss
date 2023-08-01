using DG.Tweening;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(DeathAreaBehaviour))]
    public class WipeOutWallBehaviour : MonoBehaviour
    {
        private Vector3 _originalSize;
        private float _activationDuration;
        private float _deactivateDuration;

        public void Initialize(float activateDuration = 0.5f, float deactivateDuration = 0.5f)
        {
            _activationDuration = activateDuration;
            _deactivateDuration = deactivateDuration;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            transform.DOScale(_originalSize, _activationDuration);
        }

        public void Deactivate()
        {
            transform
                .DOScaleZ(0, _deactivateDuration)
                .OnComplete(() => gameObject.SetActive(false));
        }
        
        private void Awake()
        {
            _originalSize = transform.localScale;
        }
    }
}
