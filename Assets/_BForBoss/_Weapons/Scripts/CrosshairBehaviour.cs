using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.Weapons
{
    public class CrosshairBehaviour : MonoBehaviour
    {
        [SerializeField] private Sprite _defaultCrosshair = null;
        private Image _crosshair = null;
        
        private void Awake()
        {
            _crosshair = GetComponentInChildren<Image>();
            SetDefaultCrosshair();
        }

        public void SetDefaultCrosshair()
        {
            _crosshair.sprite = _defaultCrosshair;
        }

        [Button]
        public void SetCrosshairImage(Sprite image)
        {
            _crosshair.sprite = image;
        }
    }
}
