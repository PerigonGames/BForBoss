using System;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SimonSaysBlockBehaviour : MonoBehaviour, ISimonSaysBlock
    {
        public event Action<ISimonSaysBlock, SimonSaysColor> OnBlockCompleted;

        private SimonSaysColor _simonSaysColor = SimonSaysColor.None;
        private MeshRenderer _renderer;
        
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material.color = Color.grey;
        }
        
        public void SetColor(SimonSaysColor color)
        {
            _simonSaysColor = color;
        }

        public void Reset()
        {
            _simonSaysColor = SimonSaysColor.None;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (_simonSaysColor != SimonSaysColor.None)
            {
                OnBlockCompleted?.Invoke(this, _simonSaysColor);
            }
        }
    }
}
