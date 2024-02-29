using System.Collections.Generic;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class SimonSaysBlocksManager : MonoBehaviour
    {
        private ISimonSaysBlock[] _blocks;
        public ISimonSaysBlock[] Blocks => _blocks;

        private Dictionary<SimonSaysColor, Color> _colorMap;

        private void Awake()
        {
            _blocks = GetComponentsInChildren<ISimonSaysBlock>();
            this.PanicIfNullOrEmptyList(_blocks, nameof(_blocks));
        }

        public void Reset()
        {
            foreach (var block in _blocks)
            {
                block.Reset();
            }
        }
    }
}
