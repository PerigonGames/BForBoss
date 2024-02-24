using System;
using System.Collections.Generic;
using BForBoss.Labor;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class SimonSaysLabor: ILabor
    {
        private ISimonSaysBlock[] _blocks;
        private readonly int _sequenceLength; 
        private SimonSaysColorData[] _sequenceOfColors;
        private Dictionary<SimonSaysColor, Color> _colorMap;

        public SimonSaysColorData[] SequenceOfColors
        {
            get
            {
                if (_sequenceOfColors == null)
                {
                    _sequenceOfColors = BuildRandomSequence();
                }

                return _sequenceOfColors;
            }
        }

        public event OnLaborCompletedEventHandler OnLaborCompleted;

        public SimonSaysLabor(
            ISimonSaysBlock[] blocks,
            Dictionary<SimonSaysColor, Color> colorMap,
            int sequenceLength)
        {
            _blocks = blocks;
            _sequenceLength = sequenceLength;
            _colorMap = colorMap;
        }
        
        public void Activate()
        {
            BlockCheck(SequenceOfColors);
            //TODO - Set random blocks to activate and set to sequence of colors
        }

        private void BlockCheck(SimonSaysColorData[] sequenceOfColors)
        {
            if (sequenceOfColors.Length <= _blocks.Length)
            {
                PanicHelper.Panic(new Exception($"Number of blocks is less than or equal to the sequence of colors generated.\nBLocks: {_blocks.Length}.\nSequence Generated: {sequenceOfColors.Length}"));
            }
        }

        public void Reset()
        {
            _sequenceOfColors = BuildRandomSequence();
        }
        
        private SimonSaysColorData[] BuildRandomSequence()
        {
            var randomizer = new RandomUtility();
            SimonSaysColorData[] sequence = new SimonSaysColorData[_sequenceLength];
            var colorAmount = Enum.GetValues(typeof(SimonSaysColor)).Length;
            for (int i = 0; i < _sequenceLength; i++)
            {
                var state = (SimonSaysColor) randomizer.NextInt(0, colorAmount - 1);
                sequence[i] = new SimonSaysColorData { 
                    SimonSaysColor = state, 
                    Color = _colorMap[state]
                };  
            }
            return sequence;
        }
    }
}
