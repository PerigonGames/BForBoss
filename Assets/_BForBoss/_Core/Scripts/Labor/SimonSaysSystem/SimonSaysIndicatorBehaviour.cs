using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SimonSaysIndicatorBehaviour : MonoBehaviour
    {
        private MeshRenderer _renderer;
        private Color _color;
        private Dictionary<SimonSaysColor, Color> _colorMap;
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        public void Initialize(Dictionary<SimonSaysColor, Color> colorMap, Color color)
        {
            _colorMap = colorMap;
            _color = color;
        }

        public void Reset()
        {
            _renderer.material.color = _colorMap[SimonSaysColor.None];
        }

        public void SetColor()
        {
            _renderer.material.color = _color;
        }
    }
}
