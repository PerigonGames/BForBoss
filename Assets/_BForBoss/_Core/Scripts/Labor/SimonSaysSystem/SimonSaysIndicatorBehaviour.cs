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
            _renderer.material.color = Color.grey;
        }

        public void Initialize(Color color)
        {
            _color = color;
        }

        public void Reset()
        {
            _renderer.material.color = Color.grey;
        }

        public void SetColor()
        {
            _renderer.material.color = _color;
        }
    }
}
