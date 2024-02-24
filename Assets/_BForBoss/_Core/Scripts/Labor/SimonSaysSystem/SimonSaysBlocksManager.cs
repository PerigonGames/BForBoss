using System;
using System.Collections.Generic;
using Perigon.Utility;
using PerigonGames;
using Sirenix.OdinInspector;
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
        
        public void Initialize(Dictionary<SimonSaysColor, Color> colorMap)
        {
            _colorMap = colorMap;
        }

        [Button]
        public void DebugBlockColors()
        {
            SetBlockColors(new SimonSaysColorData[]
            {
                new SimonSaysColorData(SimonSaysColor.Blue, _colorMap[SimonSaysColor.Blue]),
                new SimonSaysColorData(SimonSaysColor.Red, _colorMap[SimonSaysColor.Red]),
                new SimonSaysColorData(SimonSaysColor.Purple, _colorMap[SimonSaysColor.Purple]),
            });
        }
        
        public void SetBlockColors(SimonSaysColorData[] sequence)
        {
            _blocks.ShuffleFisherYates();
            var blockIndex = 0;
            for (blockIndex = 0; blockIndex < sequence.Length; blockIndex++)
            {
                _blocks[blockIndex].SetColorData(sequence[blockIndex]);
            }

            var randomizer = new RandomUtility();
            for (var i = blockIndex; i < _blocks.Length; i++)
            {
                if (randomizer.CoinFlip())
                {
                    _blocks[i].SetColorData(new SimonSaysColorData(simonSaysColor: SimonSaysColor.None, color: _colorMap[SimonSaysColor.None]));
                }
                else
                {
                    var colorAmount = Enum.GetValues(typeof(SimonSaysColor)).Length;
                    var state = (SimonSaysColor) randomizer.NextInt(0, colorAmount - 1);
                    var data = new SimonSaysColorData(simonSaysColor: state, color: _colorMap[state]);
                    _blocks[i].SetColorData(data);
                }
            }
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
