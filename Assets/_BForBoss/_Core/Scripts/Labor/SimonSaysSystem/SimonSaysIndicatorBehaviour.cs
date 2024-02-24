using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SimonSaysIndicatorBehaviour : MonoBehaviour
    {
        private MeshRenderer _renderer;
        private Color _color;
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        public void Initialize(Color color)
        {
            _color = color;
        }

        public void Reset()
        {
            _renderer.material.color = SimonSaysUtility.DefaultNoneColor;
        }

        public void SetColor()
        {
            _renderer.material.color = _color;
        }
    }
}
