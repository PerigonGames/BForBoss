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
        private Queue<SimonSaysColor> _queue = new Queue<SimonSaysColor>();

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
            Subscribe();
        }

        private void Subscribe()
        {
            foreach (var block in _blocks)
            {
                block.OnBlockCompleted += HandleOnBlockTouched;
            }
        }

        private void HandleOnBlockTouched(ISimonSaysBlock block, SimonSaysColor color)
        {
            var expectedColor = _queue.Dequeue();
            block.Reset();
            var didSucceed = expectedColor == color;
            if (didSucceed && _queue.Count == 0)
            {
                OnLaborCompleted?.Invoke(this, new OnLaborCompletedArgs(didSucceed: true));
            }
            else if (!didSucceed)
            {
                OnLaborCompleted?.Invoke(this, new OnLaborCompletedArgs(didSucceed: false));
            }
        }

        public void Activate()
        {
            BlockCheck(SequenceOfColors);
            foreach (var element in _sequenceOfColors)
            {
                _queue.Enqueue(element.SimonSaysColor);
            }
        }

        private void BlockCheck(SimonSaysColorData[] sequenceOfColors)
        {
            if (sequenceOfColors.Length > _blocks.Length)
            {
                PanicHelper.Panic(new Exception($"Number of blocks is less than or equal to the sequence of colors generated.\nBLocks: {_blocks.Length}.\nSequence Generated: {sequenceOfColors.Length}"));
            }
        }

        public void Reset()
        {
            _sequenceOfColors = BuildRandomSequence();
            _queue.Clear();
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
